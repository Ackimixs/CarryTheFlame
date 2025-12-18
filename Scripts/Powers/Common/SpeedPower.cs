using Godot;
using System;

[GlobalClass]
public partial class SpeedPower : PowerData

{
	[Export] public float SpeedMultiplier = 2.0f;
	[Export] public float SpeedBase = 8.0f;

	public override void Apply(Player player)
	{
		base.Apply(player);

		player.SetSpeed(SpeedBase * SpeedMultiplier);
		GD.Print($"{DisplayName} applied: Player has now more speed.");
	}

	public override void Remove(Player player)
	{
		base.Remove(player);

		player.SetSpeed(SpeedBase);
		GD.Print($"{DisplayName} removed: Player has no longer more speed.");
	}
}
