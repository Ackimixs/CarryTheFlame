using Godot;
using System;

public partial class ActivePowerUI : Button
{
    public PowerData Power { get; private set; }

    [Export] public Label PowerNameLabel;
    [Export] public TextureRect PowerIcon;

    public event Action<ActivePowerUI> PowerSelected;

    public void Setup(PowerData powerData)
    {
        Power = powerData;
        PowerNameLabel.Text = Power.DisplayName;
        PowerIcon.Texture = Power.Icon;
        Pressed += () =>
        {
            GD.Print($"Power Button Pressed: {Power.DisplayName}");
            PowerSelected?.Invoke(this);
        };
    }
}
