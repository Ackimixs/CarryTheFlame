using Godot;
using System;

public partial class Weapon : Node3D
{
	[Export] public float DamageMultiplier = 1;

	[Export] protected AnimationPlayer animPlayer;
	[Export] public string WeaponName = "Weapon";
	[Export] public AudioStreamPlayer3D AudioPlayer;

	protected Player _player;

	public bool IsEquipped = false;

	public override void _Ready()
	{
		AudioPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
		Hide();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("shoot") && IsEquipped)
		{
			AudioPlayer.Play();
			HandleShoot();
		}
	}

	public void SetPlayer(Player player)
	{
		_player = player;
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
