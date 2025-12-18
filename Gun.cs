using Godot;
using System;

public partial class Gun : Node3D
{
	[ExportGroup("Type d'Arme")]
	[Export] private bool isMelee = false; // Coche cette case pour tes armes de mêlée

	[ExportGroup("Munitions")]
	[Export] private PackedScene bulletScene;
	[Export] private int maxAmmo = 10;
	private int currentAmmo;
	private bool isReloading = false;

	private Marker3D marker;
	private AnimationPlayer animPlayer;

	public override void _Ready()
	{
		currentAmmo = maxAmmo;
		marker = GetNode<Marker3D>("%Marker3D");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsVisibleInTree()) return;

		// --- LOGIQUE DE TIR / ATTAQUE ---
		if (Input.IsActionJustPressed("shoot") && !isReloading)
		{
			if (isMelee)
			{
				AttackMelee(); // Les armes de mêlée n'ont pas besoin de munitions
			}
			else if (currentAmmo > 0)
			{
				Shoot(); // Les armes à distance utilisent le système actuel
			}
			else
			{
				GD.Print("Chargeur vide ! Appuyez sur R pour recharger.");
			}
		}

		// --- LOGIQUE DE RECHARGE ---
		// On ignore totalement la recharge si c'est une arme de mêlée
		if (!isMelee && Input.IsActionJustPressed("reload") && !isReloading && currentAmmo < maxAmmo)
		{
			Reload();
		}
	}

	private void Shoot()
	{
		currentAmmo--;
		GD.Print("Munitions : " + currentAmmo + "/" + maxAmmo);

		PlayAnimation("local/shoot");

		if (bulletScene != null)
		{
			Bullet bullet = bulletScene.Instantiate<Bullet>();
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
