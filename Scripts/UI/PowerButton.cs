using Godot;
using System;

public partial class PowerButton : Button
{
    public PowerData Power { get; private set; }
    public event Action<PowerData> PowerSelected;

    public void Setup(PowerData power)
    {
        Power = power;
        Text = power.DisplayName;
        Icon = power.Icon;
        Pressed += () => PowerSelected?.Invoke(Power);
    }
}
