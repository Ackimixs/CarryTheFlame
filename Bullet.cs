using Godot;
using System;

public partial class Bullet : Area3D
{
	[Export]
	private float speed = 40f;

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


}
