using Godot;
using System;

[GlobalClass]
public partial class DoubleJump : PowerData
{
    public override void Apply(Player player)
    {
        base.Apply(player);

        player.canDoubleJump = true;
        GD.Print($"{DisplayName} applied: Player can now double jump.");
    }

    public override void Remove(Player player)
    {
        base.Remove(player);

        player.canDoubleJump = false;
        GD.Print($"{DisplayName} removed: Player can no longer double jump.");
    }
}
