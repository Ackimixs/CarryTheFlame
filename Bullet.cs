using Godot;
using System;

public partial class Bullet : Area3D
{
	[Export]
	private float speed = 35f;

	[Export]
	private float maxDistance = 100f;
	private float travelledDistance;

	public override void _PhysicsProcess(double delta)
	{
		Position += Transform.Basis.Z * speed * (float)delta;
		travelledDistance += speed * (float)delta;

		
		if (travelledDistance > maxDistance)
		{
			QueueFree();
		}
	}

	private void OnBodyEntered(Node body)
	{
		if (body is TargetStatue statue)
		{
			statue.OnHit();
		}
		QueueFree(); 
	}
}
