using Godot;
using System;

public partial class ActivePowerUI : Button
{
    public PowerData Power { get; private set; }

    [Export] public Label PowerNameLabel;
    [Export] public TextureRect PowerIcon;
    [Export] public NinePatchRect PowerBackground;

    public event Action<ActivePowerUI> PowerSelected;

    public void Setup(PowerData power)
    {
        Power = power;
        PowerNameLabel.Text = Power.DisplayName;
        PowerIcon.Texture = Power.Icon;
        PowerBackground.SelfModulate = PowerData.RarityToColor(Power.Rarity);
        Pressed += () =>
        {
            PowerSelected?.Invoke(this);
        };
    }
}
