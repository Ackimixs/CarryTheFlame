using Godot;

public partial class PowerData : Resource
{
    [Export] public string Id;
    [Export] public string DisplayName;
    [Export] public string Description;
    [Export] public Texture2D Icon;
    [Export] public PowerRarity Rarity;

    [Export] public bool isStackable = false;

    public virtual void Apply(Player player)
    {
        GD.Print($"Apply power: {DisplayName}");
    }

    public virtual void Remove(Player player)
    {
        GD.Print($"Remove power: {DisplayName}");
    }

    public static Color RarityToColor(PowerRarity rarity)
    {
        return rarity switch
        {
            PowerRarity.Common => Colors.DarkGreen,
            PowerRarity.Rare => Colors.Blue,
            PowerRarity.Elite => Colors.Purple,
            PowerRarity.Legendary => Colors.Orange,
            _ => Colors.DarkGreen,
        };
    }
}