using Godot;
using System;

[GlobalClass]
public partial class HealthPower : PowerData
{

	[Export] public float HealthMultiplier = 1.2f;

	public override void Apply(Player player)
	{
		base.Apply(player);

		player.SetBaseHealth(player.GetBaseHealth() * HealthMultiplier);
	}

	public override void Remove(Player player)
	{
		base.Remove(player);

		player.SetBaseHealth(player.GetBaseHealth() / HealthMultiplier);
	}
}
