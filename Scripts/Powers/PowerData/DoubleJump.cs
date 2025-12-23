using Godot;
using System;

[GlobalClass]
public partial class DoubleJump : PowerData
{

    [Export] public int ExtraJumps = 1;

    public override void Apply(Player player)
    {
        base.Apply(player);

        player.AddJump(ExtraJumps);
    }

    public override void Remove(Player player)
    {
        base.Remove(player);

        player.AddJump(-ExtraJumps);
    }
}
