using Godot;
using System;

public partial class Hitbox : Area3D
{
    [Export] public int Damage = 1;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }
    
    private void OnAreaEntered(Area3D area)
    {
        if (area is Bullet bullet)
        {
            Mobs mob = GetParentOrNull<Mobs>();
            mob.TakeDamage(Damage);
            if (!mob.player.powerManager.HasPower<PiercingBulletsPowers>())
            {
                bullet.QueueFree();
            }
        }
    }
}
