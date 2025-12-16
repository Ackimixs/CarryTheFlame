using Godot;
using System;

public partial class PowerChoiceUI : Control
{
	[Export] public PackedScene PowerButtonScene;
	[Export] public NodePath PowerManagerPath;

	private PowerManager _powerManager;

	public override void _Ready()
	{
		_powerManager = GetNode<PowerManager>(PowerManagerPath);
		Generate();
	}

	private void Generate()
	{
		var container = GetNode<HBoxContainer>("HBoxContainer");
		var powers = _powerManager.GetRandomPowers(3);

		foreach (var power in powers)
		{
			var btn = PowerButtonScene.Instantiate<PowerButton>();
			btn.Setup(power);
			btn.PowerSelected += OnPowerSelected;
			container.AddChild(btn);
		}
	}

	private void OnPowerSelected(PowerData power)
	{
		GD.Print($"Chosen: {power.DisplayName}");
		Hide();
	}
}
