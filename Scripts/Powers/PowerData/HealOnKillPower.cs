using Godot;
using System;

[GlobalClass]
public partial class HealOnKillPower : PowerData
{

    [Export] public int HealthBonus = 4;

    public override void Apply(Player player)
    {
        base.Apply(player);

        player.AddHealOnKill(HealthBonus);
    }

    public override void Remove(Player player)
    {
        base.Remove(player);

        player.AddHealOnKill(-HealthBonus);
    }
}