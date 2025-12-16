using Godot;

public partial class PowerData : Resource
{
    [Export] public string Id;
    [Export] public string DisplayName;
    [Export] public string Description;
    [Export] public Texture2D Icon;
    [Export] public PowerRarity Rarity;

    public virtual void Apply(Node player)
    {
        GD.Print($"Apply power: {DisplayName}");
    }
}
