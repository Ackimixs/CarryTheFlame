using Godot;
using System;

public partial class LongRangeWeapon : Weapon
{
	[ExportGroup("Munitions")]
	[Export] protected PackedScene bulletScene;
	[Export] protected int maxAmmo = 10;
	protected int currentAmmo;
	protected bool isReloading = false;

	[Export] protected Marker3D marker;

	public override void _Ready()
	{
		base._Ready();

		currentAmmo = maxAmmo;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Input.IsActionJustPressed("reload"))
		{
			Reload();
		}
	}

	protected override void HandleShoot()
	{
		base.HandleShoot();
		Shoot();
	}

	protected void Shoot()
	{
		if (currentAmmo <= 0)
		{
			return;
		}

		currentAmmo--;
		GD.Print("Munitions : " + currentAmmo + "/" + maxAmmo);

		PlayAnimation("local/shoot");

		if (bulletScene != null)
		{
			var bullet = bulletScene.Instantiate<Bullet>();
			GetTree().CurrentScene.AddChild(bullet);
			bullet.GlobalTransform = marker.GlobalTransform;
			bullet.SetDamage(_player.GetDamage() * DamageMultiplier);
		}
	}

	protected async void Reload()
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
