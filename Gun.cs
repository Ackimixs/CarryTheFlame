using Godot;
using System;

public partial class Gun : MeshInstance3D
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
		marker.AddChild(bullet);
		bullet.Position = marker.GlobalPosition;
		bullet.Basis = marker.GlobalBasis;
	  }
	}
}
