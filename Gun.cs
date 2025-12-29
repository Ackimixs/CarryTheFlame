using Godot;
using System;

public partial class Gun : Node3D
{
	[ExportGroup("Type d'Arme")]
	[Export] private bool isMelee = false;
	[Export] private bool isBow = false; // Nouvelle option pour l'arc

	[ExportGroup("Munitions")]
	[Export] private PackedScene bulletScene; // Ici, glisse ton prefab de flèche (Arrow)
	[Export] private int maxAmmo = 10;
	private int currentAmmo;
	private bool isReloading = false;

	// Références aux composants
	private Marker3D marker;
	private AnimationPlayer animPlayer;
	private MeshInstance3D arrow;

	public override void _Ready()
	{
		currentAmmo = maxAmmo;
		marker = GetNode<Marker3D>("%Marker3D");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		// Si c'est un arc, on cherche le mesh de la flèche et on le cache
		if (isBow)
		{
			arrow = GetNode<MeshInstance3D>("%arrow"); // Nom à adapter selon ton nœud
			if (arrow != null) arrow.Hide();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsVisibleInTree()) return;

		// --- LOGIQUE DE TIR (ARC) ---
		if (isBow && !isReloading && currentAmmo > 0)
		{
			HandleBowLogic();
		}
		// --- LOGIQUE DE TIR (FEU / MÊLÉE) ---
		else if (!isBow && Input.IsActionJustPressed("shoot") && !isReloading)
		{
			if (isMelee) AttackMelee();
			else if (currentAmmo > 0) Shoot();
		}

		// Recharge (uniquement si pas en train de charger l'arc)
		if (!isMelee && Input.IsActionJustPressed("reload") && !isReloading && currentAmmo < maxAmmo)
		{
			Reload();
		}
	}

	private void HandleBowLogic()
	{
		// 1. On commence à bander l'arc
		if (Input.IsActionJustPressed("shoot"))
		{
			if (arrow != null) arrow.Show();
			PlayAnimation("local/charge");
		}

		// 2. On relâche la flèche
		if (Input.IsActionJustReleased("shoot"))
		{
			if (arrow != null) arrow.Hide();
			Shoot();
		}
	}

	private void Shoot()
	{
		currentAmmo--;
		GD.Print("Munitions : " + currentAmmo + "/" + maxAmmo);

		PlayAnimation("local/shoot");

		if (bulletScene != null)
		{
			// Ici, bulletScene contiendra ton prefab "Arrow" qui avance tout seul
			var bullet = bulletScene.Instantiate<Node3D>();
			GetTree().CurrentScene.AddChild(bullet);
			bullet.GlobalTransform = marker.GlobalTransform;
		}
	}

	private void AttackMelee()
	{
		GD.Print("Coup de mêlée !");
		PlayAnimation("local/shoot"); // On réutilise ton animation de mouvement pour l'attaque
		
		// Ici, tu pourrais ajouter une détection de collision (Raycast) pour les dégâts au corps à corps
	}

	private async void Reload()
	{
		if (animPlayer != null && animPlayer.HasAnimation("local/reload"))
		{
			isReloading = true;
			GD.Print("Recharge...");
			animPlayer.Play("local/reload");
			await ToSignal(animPlayer, "animation_finished");
			currentAmmo = maxAmmo;
			isReloading = false;
			GD.Print("Recharge terminée !");
		}
	}

	// Petite fonction utilitaire pour éviter de répéter le code des animations
	private void PlayAnimation(string animName)
	{
		if (animPlayer != null && animPlayer.HasAnimation(animName))
		{
			animPlayer.Stop();
			animPlayer.Play(animName);
		}
	}
}
