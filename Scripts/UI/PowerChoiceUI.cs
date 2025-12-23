using Godot;
using System;

public partial class PowerChoiceUI : Control
{
	[Export] public PackedScene PowerButtonScene;
	[Export] public PackedScene ActivePowerScene;
	[Export] public RoundManager RoundManager;
	[Export] public HBoxContainer NewPowerContainer;
	[Export] public HBoxContainer ActivePowerContainer;
	[Export] public Label HaveToChangePowerLabel;
	[Export] public Button SkipButton;

	private PowerManager _powerManager;
	private PowerButton selectedPowerButton;

	public override void _Ready()
	{
		_powerManager = GetNode<PowerManager>("%PowerManager");
		Generate();
		HaveToChangePowerLabel.Visible = false;

		foreach (Node child in NewPowerContainer.GetChildren())
		{
			child.QueueFree();
		}

		foreach (Node child in ActivePowerContainer.GetChildren())
		{
			child.QueueFree();
		}
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

		foreach (Node child in ActivePowerContainer.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var power in _powerManager._activePowers)
		{
			var activePowerUi = ActivePowerScene.Instantiate<ActivePowerUI>();
			activePowerUi.Setup(power);
			ActivePowerContainer.AddChild(activePowerUi);
			activePowerUi.PowerSelected += OnActivePowerSelected;
		}
	}

	private void OnPowerSelected(PowerButton powerButton)
	{
		var power = powerButton.Power;
		selectedPowerButton = powerButton;
		GD.Print($"Chosen: {power.DisplayName}");
		if (!_powerManager.AddSelectedPower(power))
		{
			GD.Print("Failed to add power to player.");
			HaveToChangePowerLabel.Visible = true;
			return;
		}
		RoundManager.PowerChosen();
		Hide();

		var activePowerUi = ActivePowerScene.Instantiate<ActivePowerUI>();
		activePowerUi.Setup(power);
		ActivePowerContainer.AddChild(activePowerUi);
		activePowerUi.PowerSelected += OnActivePowerSelected;

		selectedPowerButton = null;
		HaveToChangePowerLabel.Visible = false;
	}

	private void OnActivePowerSelected(ActivePowerUI activePowerUi)
	{
		if (selectedPowerButton != null)
		{
			GD.Print($"Replacing {activePowerUi.Power.DisplayName} with {selectedPowerButton.Power.DisplayName}");
			_powerManager.ReplacePower(activePowerUi.Power, selectedPowerButton.Power);

			RoundManager.PowerChosen();
			Hide();

			activePowerUi.QueueFree();

			var newActivePowerUi = ActivePowerScene.Instantiate<ActivePowerUI>();
			newActivePowerUi.Setup(selectedPowerButton.Power);
			ActivePowerContainer.AddChild(newActivePowerUi);
			newActivePowerUi.PowerSelected += OnActivePowerSelected;

			selectedPowerButton = null;
			HaveToChangePowerLabel.Visible = false;
		}
	}

	private void _OnSkipButtonPressed()
	{
		RoundManager.PowerChosen();
		Hide();
	}
}
