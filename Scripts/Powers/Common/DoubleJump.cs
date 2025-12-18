using Godot;
using System;

[GlobalClass]
public partial class DoubleJump : PowerData
{
    public override void Apply(Player player)
    {
        base.Apply(player);

        player.AddJump(1);
        GD.Print($"{DisplayName} applied: Player can now double jump.");
    }

    public override void Remove(Player player)
    {
        base.Remove(player);

        player.RemoveJump(1);
        GD.Print($"{DisplayName} removed: Player can no longer double jump.");
    }
}
