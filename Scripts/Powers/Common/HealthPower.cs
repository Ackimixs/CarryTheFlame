using Godot;
using System;

[GlobalClass]
public partial class HealthPower : PowerData
{

	[Export] public int HealthBonus = 8;

	public override void Apply(Player player)
	{
		base.Apply(player);

		player.AddHealth(HealthBonus);
	}

	public override void Remove(Player player)
	{
		base.Remove(player);

		player.AddHealth(-HealthBonus);
	}
}
