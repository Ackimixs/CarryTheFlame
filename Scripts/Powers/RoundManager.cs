using Godot;
using System;

public partial class RoundManager : Node
{
	[Export] private PowerChoiceUI PowerChoiceUI;

	[Export] private Player _player;

	public override void _Ready()
	{
		// EndRound();
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
	}

	public void _OnFireEndRoundTimeout()
	{
		EndRound();
	}
}
