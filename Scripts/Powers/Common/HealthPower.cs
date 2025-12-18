using Godot;
using System;

[GlobalClass]
public partial class HealthPower : PowerData
{
	public override void Apply(Player player)
	{
		base.Apply(player);

		player.SetSpeed(16);
		GD.Print($"{DisplayName} applied: Player has now more health.");
	}

	public override void Remove(Player player)
	{
		base.Remove(player);

		player.SetSpeed(8);
		GD.Print($"{DisplayName} removed: Player has no longer more health.");
	}
}
