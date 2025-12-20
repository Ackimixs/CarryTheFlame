using Godot;
using System;

public partial class PowerButton : Button
{
    public PowerData Power { get; private set; }

    [Export] public Label PowerNameLabel;
    [Export] public Label PowerDescriptionLabel;
    [Export] public TextureRect PowerIcon;
    [Export] public NinePatchRect PowerBackground;

    public event Action<PowerButton> PowerSelected;

    public void Setup(PowerData power)
    {
        Power = power;
        PowerNameLabel.Text = Power.DisplayName;
        PowerDescriptionLabel.Text = Power.Description;
        PowerIcon.Texture = Power.Icon;
        PowerBackground.SelfModulate = PowerData.RarityToColor(Power.Rarity);
        Pressed += () =>
        {
            PowerSelected?.Invoke(this);
        };
    }
}
