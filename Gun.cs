using Godot;
using System;

public partial class Gun : Node3D
{
	
	[Export]
	private PackedScene bulletScene;
	private Marker3D marker;
	private AnimationPlayer animPlayer;
	public override void _Ready()
	{
		marker = GetNode<Marker3D>("%Marker3D");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}
	private void Shoot()
	{
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
	public override void _PhysicsProcess(double delta)
	{
		if (IsVisibleInTree() && Input.IsActionJustPressed("shoot"))
		{
			Shoot();
		}
	}
}
