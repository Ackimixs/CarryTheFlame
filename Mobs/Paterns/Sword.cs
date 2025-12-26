using Godot;
using System;

public partial class Sword : Area3D
{
    [Export]
    private int damage = 2;

    private bool _canDealDamage = false;
    private bool _hasHit = false;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (!_canDealDamage)
            return;

        if (body is Player player)
        {
            player.TakeDamage(damage);
            _hasHit = false;
        }
    }

    public void EnableDamage()
    {
        _canDealDamage = true;
        _hasHit = false;
    }
    public void DisableDamage()
    {
        _canDealDamage = false;
    }
}
