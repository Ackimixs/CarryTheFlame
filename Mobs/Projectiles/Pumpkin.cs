using Godot;
using System;

public partial class Pumpkin : Area3D
{
    [Export]
    private float speed = 35f;

    [Export]
    private float maxDistance = 100f;
    private float travelledDistance;
    
    [Export]
    private int damage = 2;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition -= -GlobalTransform.Basis.Z * speed * (float)delta;
        travelledDistance += speed * (float)delta;

		
        if (travelledDistance > maxDistance)
        {
            QueueFree();
        }
    }
    
    private void OnBodyEntered(Node body)
    {
        if (body is Player player)
        {
            player.TakeDamage(damage);
        }
    }
}
