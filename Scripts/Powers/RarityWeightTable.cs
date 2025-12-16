using Godot;
using System.Collections.Generic;

public partial class RarityWeightTable : Resource
{
    [Export] public float Common = 60f;
    [Export] public float Rare = 25f;
    [Export] public float Elite = 10f;
    [Export] public float Legendary = 5f;

    public float GetWeight(PowerRarity rarity)
    {
        return rarity switch
        {
            PowerRarity.Common => Common,
            PowerRarity.Rare => Rare,
            PowerRarity.Elite => Elite,
            PowerRarity.Legendary => Legendary,
            _ => 0f
        };
    }

    public float TotalWeight =>
        Common + Rare + Elite + Legendary;
}
