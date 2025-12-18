using Godot;
using System;

[GlobalClass]
public partial class DamagePower : PowerData
{
	public override void Apply(Player player)
	{
		base.Apply(player);

		player.SetSpeed(16);
		GD.Print($"{DisplayName} applied: Player has now more damage.");
	}

	public override void Remove(Player player)
	{
		base.Remove(player);

		player.SetSpeed(8);
		GD.Print($"{DisplayName} removed: Player has no longer more damage.");
	}
}
