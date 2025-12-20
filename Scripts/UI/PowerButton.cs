using Godot;
using System;

public partial class PowerButton : Button
{
    public PowerData Power { get; private set; }

    [Export] public Label PowerNameLabel;
    [Export] public Label PowerDescriptionLabel;
    [Export] public TextureRect PowerIcon;

    public event Action<PowerButton> PowerSelected;

    public void Setup(PowerData power)
    {
        Power = power;
        PowerNameLabel.Text = Power.DisplayName;
        PowerDescriptionLabel.Text = Power.Description;
        PowerIcon.Texture = Power.Icon;
        PowerIcon.ExpandMode = TextureRect.ExpandModeEnum.KeepSize;
        PowerIcon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        Pressed += () =>
        {
            PowerSelected?.Invoke(this);
        };
    }
}
