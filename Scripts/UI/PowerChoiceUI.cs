using Godot;
using System;

public partial class PowerChoiceUI : Control
{
	[Export] public PackedScene PowerButtonScene;
	[Export] public RoundManager RoundManager;

	private PowerManager _powerManager;

	public override void _Ready()
	{
		_powerManager = GetNode<PowerManager>("%PowerManager");
		Generate();
	}

	public void Generate()
	{

		var container = GetNode<HBoxContainer>("HBoxContainer");
		foreach (Node child in container.GetChildren())
		{
			child.QueueFree();
		}

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
		_powerManager.AddSelectedPower(power);
		RoundManager.PowerChosen();
	}
}
