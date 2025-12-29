using Godot;
using System;

public partial class Pumpkin : Area3D
{
    [Export]
    private float speed = 5f;
    [Export]
    private float maxDistance = 100f;
    [Export]
    private int damage = 1;
    [Export]
    private float turnSpeed = 6f;
    private int health = 1;
    private Player target;
    private float travelledDistance;
    private Vector3 currentDirection;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        AreaEntered += OnAreaEntered;
    }
    
    public void Initialize(Player player)
    {
        target = player;

        currentDirection = GlobalPosition.DirectionTo(player.GlobalPosition);
        currentDirection.Y = 0;
        currentDirection = currentDirection.Normalized();

        LookAt(GlobalPosition + currentDirection, Vector3.Up);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (target == null || !IsInstanceValid(target))
        {
            QueueFree();
            return;
        }

        // Desired direction (toward player)
        Vector3 desiredDirection = GlobalPosition.DirectionTo(target.GlobalPosition);
        desiredDirection.Y = 0;
        desiredDirection = desiredDirection.Normalized();

        // Smooth turning
        currentDirection = currentDirection.Lerp(
            desiredDirection,
            turnSpeed * (float)delta
        ).Normalized();

        // Move
        Vector3 movement = currentDirection * speed * (float)delta;
        GlobalPosition += movement;
        travelledDistance += movement.Length();

        // Rotate to face movement
        LookAt(GlobalPosition + currentDirection, Vector3.Up);

        if (travelledDistance >= maxDistance)
            QueueFree();
    }
    
    private void OnBodyEntered(Node body)
    {
        if (body is Player player)
        {
            player.TakeDamage(damage);
            QueueFree(); 
        }
    }
    private void OnAreaEntered(Area3D area)
    {
        if (area is Bullet bullet)
        {
            QueueFree();
            bullet.QueueFree();
        }
    }
}
