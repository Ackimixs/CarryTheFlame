using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PowerManager : Node
{
	[Export] public Godot.Collections.Array<PowerData> AllPowers;
	[Export] public RarityWeightTable WeightTable;

	public override void _Ready()
	{
		/*foreach (var power in AllPowers)
			GD.Print(power.DisplayName);*/
	}

	public List<PowerData> GetRandomPowers(int count)
	{
		List<PowerData> result = new();
		int safety = 50;

		while (result.Count < count && safety-- > 0)
		{
			var power = GetRandomPower();
			if (power != null && !result.Contains(power))
				result.Add(power);
		}

		return result;
	}

	private PowerData GetRandomPower()
	{
		PowerRarity rarity = RollRarity();
		var pool = AllPowers.Where(p => p.Rarity == rarity).ToList();
		if (pool.Count == 0) return null;
		return pool[(int)(GD.Randf() * pool.Count)];
	}

	private PowerRarity RollRarity()
	{
		float roll = GD.Randf() * WeightTable.TotalWeight;
		float acc = 0f;

		acc += WeightTable.Common;
		if (roll <= acc) return PowerRarity.Common;

		acc += WeightTable.Rare;
		if (roll <= acc) return PowerRarity.Rare;

		acc += WeightTable.Elite;
		if (roll <= acc) return PowerRarity.Elite;

		return PowerRarity.Legendary;
	}
}
