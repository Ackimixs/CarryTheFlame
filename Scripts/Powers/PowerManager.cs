using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PowerManager : Node
{
	[Export] public Godot.Collections.Array<PowerData> AllPowers;
	[Export] public Godot.Collections.Dictionary<int, RarityWeightTable> WeightTables;

	private RarityWeightTable ActiveWeightTable
	{
		get
		{
			int round = RoundManager._roundNumber;

			int bestKey = int.MinValue;
			RarityWeightTable bestTable = null;

			foreach (int key in WeightTables.Keys)
			{
				if (key <= round && key > bestKey)
				{
					bestKey = key;
					bestTable = WeightTables[key];
				}
			}

			return bestTable ?? WeightTables.Values.First();
		}
	}

	[Export] public Player Player;

	public static PowerManager Instance { get; private set; }

	private List<PowerData> _activePowers = new();
	public int MaxActivePowers = 3;

	[Export] public Label ActivePowerLabel;
	[Export] private RoundManager RoundManager;

	public override void _Ready()
	{
		Instance = this;

		foreach (var power in AllPowers)
		{
			GD.Print(power.DisplayName);
		}
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
		float roll = GD.Randf() * ActiveWeightTable.TotalWeight;
		float acc = 0f;

		acc += ActiveWeightTable.Common;
		if (roll <= acc) return PowerRarity.Common;

		acc += ActiveWeightTable.Rare;
		if (roll <= acc) return PowerRarity.Rare;

		acc += ActiveWeightTable.Elite;
		if (roll <= acc) return PowerRarity.Elite;

		return PowerRarity.Legendary;
	}

	public bool AddSelectedPower(PowerData power)
	{
		if (_activePowers.Count >= MaxActivePowers)
		{
			GD.Print("Cannot add more powers, max reached.");
			return false;
		}

		_activePowers.Add(power);
		GD.Print("Active Powers: " + _activePowers.Count);
		if (!power.isStackable)
		{
			AllPowers.Remove(power);
		}
		power.Apply(Player);

		ActivePowerLabel.SetText("Pouvoir Actif(s) : " + _activePowers.Count + "/" + MaxActivePowers);

		return true;
	}

	public void RemovePower(PowerData power)
	{
		if (_activePowers.Contains(power))
		{
			_activePowers.Remove(power);
			if (!power.isStackable)
			{
				AllPowers.Add(power);
			}
			power.Remove(Player);
		}

		ActivePowerLabel.SetText("Pouvoir Actif(s) : " + _activePowers.Count + "/" + MaxActivePowers);
	}

	public void ReplacePower(PowerData oldPower, PowerData newPower)
	{
		RemovePower(oldPower);
		AddSelectedPower(newPower);
	}

	public void AddMaxActivePowers(int nb)
	{
		MaxActivePowers += nb;

		ActivePowerLabel.SetText("Pouvoir Actif(s) : " + _activePowers.Count + "/" + MaxActivePowers);
	}

	public bool HasPiercingBullets()
	{
		return _activePowers.Any(p => p is PiercingBulletsPowers);
	}
}
