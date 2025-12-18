using Godot;
using System;

public partial class RoundManager : Node
{
	[Export] private PowerChoiceUI PowerChoiceUI;

	[Export] private Player _player;

	[Export] private Timer _endRoundTimer;

	public override void _Ready()
	{
		// EndRound();
		PowerChoiceUI.Hide();
		_endRoundTimer.Start();
	}

	public void EndRound()
	{
		_player.ShowCursor();
		// GetTree().Paused = true;*
		PowerChoiceUI.Generate();
		PowerChoiceUI.Show();
	}

	public void PowerChosen()
	{
		_player.HideCursor();
		PowerChoiceUI.Hide();
		// GetTree().Paused = false;
		// StartNextRound();
		_endRoundTimer.Start();
	}

	public void _OnFireEndRoundTimeout()
	{
		EndRound();
	}
}
