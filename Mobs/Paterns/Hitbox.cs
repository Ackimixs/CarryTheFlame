using Godot;
using System;

public partial class Hitbox : Area3D
{
    [Export] public float DamageMultiplier = 1;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }
    
    private void OnAreaEntered(Area3D area)
    {
        if (area is Bullet bullet)
        {
            Mobs mob = GetParentOrNull<Mobs>();
            mob.TakeDamage(bullet.damage * DamageMultiplier);
            if (!mob.player.powerManager.HasPower<PiercingBulletsPowers>())
            {
                bullet.QueueFree();
            }
        }
    }
}
