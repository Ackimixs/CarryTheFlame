using Godot;
using System;

public partial class PowerChoiceUI : Control
{
	[Export] public PackedScene PowerButtonScene;
	[Export] public PackedScene ActivePowerScene;
	[Export] public RoundManager RoundManager;
	[Export] public HBoxContainer NewPowerContainer;
	[Export] public HBoxContainer ActivePowerContainer;

	private PowerManager _powerManager;

	public override void _Ready()
	{
		_powerManager = GetNode<PowerManager>("%PowerManager");
		Generate();
	}

	public void Generate()
	{
		foreach (Node child in NewPowerContainer.GetChildren())
		{
			child.QueueFree();
		}

		var powers = _powerManager.GetRandomPowers(3);

		foreach (var power in powers)
		{
			var btn = PowerButtonScene.Instantiate<PowerButton>();
			btn.Setup(power);
			btn.PowerSelected += OnPowerSelected;
			NewPowerContainer.AddChild(btn);
		}
	}

	private void OnPowerSelected(PowerData power)
	{
		GD.Print($"Chosen: {power.DisplayName}");
		Hide();
		_powerManager.AddSelectedPower(power);
		RoundManager.PowerChosen();

		var activePowerUi = ActivePowerScene.Instantiate<ActivePowerUI>();
		activePowerUi.Setup(power);
		ActivePowerContainer.AddChild(activePowerUi);
	}
}
