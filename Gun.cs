using Godot;
using System;

public partial class Gun : Node3D
{
	
	[Export]
	private PackedScene bulletScene;
	private Marker3D marker;

	public override void _Ready()
	{
	  marker = GetNode<Marker3D>("%Marker3D");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("shoot"))
		{
			Bullet bullet = bulletScene.Instantiate<Bullet>();
			GetTree().CurrentScene.AddChild(bullet);
			bullet.GlobalTransform = marker.GlobalTransform;
		}
	}
}
