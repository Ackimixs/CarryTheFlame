using Godot;
using System;

public partial class Weapon : Node3D
{
	[Export] public int Damage = 1;

	[Export] protected AnimationPlayer animPlayer;
	[Export] public string WeaponName = "Weapon";

	public bool IsEquipped = false;

	public override void _Ready()
	{
		Hide();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("shoot") && IsEquipped)
		{
			HandleShoot();
		}
	}

	protected virtual void HandleShoot()
	{
	}

	protected void PlayAnimation(string animName)
	{
		if (animPlayer != null && animPlayer.HasAnimation(animName))
		{
			animPlayer.Stop();
			animPlayer.Play(animName);
		}
	}
}
