using Godot;
using System;

public partial class Gun : Node3D
{
	[Export] private PackedScene bulletScene;
	private Marker3D marker;
	private AnimationPlayer animPlayer;
	
	[Export] private int maxAmmo = 10;
	private int currentAmmo;
	private bool isReloading = false;

	public override void _Ready()
	{
		currentAmmo = maxAmmo;
		marker = GetNode<Marker3D>("%Marker3D");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsVisibleInTree()) return;

		if (Input.IsActionJustPressed("shoot") && !isReloading)
		{
			if (currentAmmo > 0)
			{
				Shoot();
			}
			else
			{
				GD.Print("Chargeur vide ! Appuyez sur R pour recharger.");
			}
		}

		// Recharge : seulement si le chargeur n'est pas déjà plein
		if (Input.IsActionJustPressed("reload") && !isReloading && currentAmmo < maxAmmo)
		{
			Reload();
		}
	}

	private void Shoot()
	{
		currentAmmo--;
		GD.Print("Munitions : " + currentAmmo + "/" + maxAmmo);

		if (animPlayer != null && animPlayer.HasAnimation("local/shoot"))
		{
			animPlayer.Stop();
			animPlayer.Play("local/shoot");
		}

		if (bulletScene != null)
		{
			Bullet bullet = bulletScene.Instantiate<Bullet>();
			GetTree().CurrentScene.AddChild(bullet);
			bullet.GlobalTransform = marker.GlobalTransform;
		}
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
}
