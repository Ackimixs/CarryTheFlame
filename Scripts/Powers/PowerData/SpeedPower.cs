using Godot;
using System;

[GlobalClass]
public partial class SpeedPower : PowerData
{
	[Export] public float SpeedMultiplier = 2.0f;

	public override void Apply(Player player)
	{
		base.Apply(player);

		player.AddSpeed(player.GetBaseSpeed() * (SpeedMultiplier - 1));
	}

	public override void Remove(Player player)
	{
		base.Remove(player);

		player.AddSpeed(-player.GetBaseSpeed() * (SpeedMultiplier - 1));
	}
}
