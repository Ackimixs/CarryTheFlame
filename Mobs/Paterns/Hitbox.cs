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
            GD.Print(Damage.ToString());
            Minion minion = GetParent<Minion>();
            minion.TakeDamage(Damage);
            if (!minion.player.powerManager.HasPower<PiercingBulletsPowers>())
            {
                bullet.QueueFree();
            }
        }
    }
}
