using Godot;
using System;

[GlobalClass]
public partial class DamagePower : PowerData
{
	[Export] public float DamageMultipler = 2.0f;

	public override void Apply(Player player)
	{
		base.Apply(player);

		player.AddDamage(player.GetBaseDamage() * (DamageMultipler - 1));
	}

	public override void Remove(Player player)
	{
		base.Remove(player);

		player.AddDamage(-player.GetBaseDamage() * (DamageMultipler - 1));
	}
}
