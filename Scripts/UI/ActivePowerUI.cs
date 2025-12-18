using Godot;
using System;

public partial class ActivePowerUI : Control
{
    private PowerData power;

    [Export] public Label PowerNameLabel;
    [Export] public TextureRect PowerIcon;

    public void Setup(PowerData powerData)
    {
        power = powerData;
        PowerNameLabel.Text = power.DisplayName;
        PowerIcon.Texture = power.Icon;
    }
}
