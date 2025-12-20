using Godot;
using System;
using System.Collections.Generic;

public partial class RoundManager : Node
{
	[Export] private PowerChoiceUI PowerChoiceUI;

	[Export] private Player _player;

	[Export] private Timer _endRoundTimer;

	[Export] private Marker3D _teleportPoint;

	private int _roundNumber = 1;

	[Export] private int nbEnemy = 2;

	[Export] private Label _timerLabel;

	[Export] private AnimationPlayer _animationPlayer;
	[Export] private Label _roundEndLabel;

	[Export] private ColorRect _backgroundRect;

	private int remainingEnemies = 0;

	[Export] private Godot.Collections.Array<MobSpawner> _mobSpawners;

	private RandomNumberGenerator _rng = new RandomNumberGenerator();

	private List<Minion> _activeMobs = new List<Minion>();

	public override void _Ready()
	{
		StartRound();
	}

	public override void _Process(double delta)
	{
		UpdateTimerUI();
	}

	public void EndRound()
	{
		_roundNumber++;
		if (_roundNumber % 5 == 0)
		{
			PowerManager.Instance.MaxActivePowers += 1;
		}

		if (_animationPlayer != null)
		{
			_animationPlayer.Play("TransitionRound");
			_animationPlayer.AnimationFinished += OnTransitionFinished;
		}
		else
		{
			ShowPowerUi();
		}
	}

	public void ShowPowerUi()
	{
		_player.ShowCursor();
		PowerChoiceUI.Generate();
		PowerChoiceUI.Show();
	}

	public void PowerChosen()
	{
		nbEnemy += 2;

		StartRound();
	}

	public void _OnFireEndRoundTimeout()
	{
		_roundEndLabel.SetText("Vous n'avez pas survécu a cette journée ...\nLes enemies non tués reviendront demain !");
		_backgroundRect.SetVisible(true);

		KillAllRemainingMobs();
		EndRound();
	}

	private void KillAllRemainingMobs()
	{
		foreach (var mob in _activeMobs.ToArray())
		{
			if (IsInstanceValid(mob))
			{
				mob.QueueFree();
			}
		}
		_activeMobs.Clear();
	}

	public void StartRound()
	{
		TeleportPlayer();

		_player.HideCursor();
		PowerChoiceUI.Hide();
		_endRoundTimer.Start();
		_backgroundRect.SetVisible(false);

		for (int i = 0; i < nbEnemy + remainingEnemies; i++)
		{
			int randomIndex = _rng.RandiRange(0, _mobSpawners.Count - 1);
			var spawner = _mobSpawners[randomIndex];
			Minion mob = spawner.SpawnMob();
			_activeMobs.Add(mob);
			mob.OnKilled += OnEnemyKilled;
		}

		remainingEnemies = nbEnemy;
	}

	private void OnEnemyKilled(Minion minion)
	{
		if (_activeMobs.Contains(minion))
		{
			_activeMobs.Remove(minion);
		}

		remainingEnemies--;
		if (remainingEnemies <= 0)
		{
			_roundEndLabel.SetText("Vous avez survécu a cette journée ...\nRendez-vous à l'aube !");
			_backgroundRect.SetVisible(true);

			EndRound();
			_endRoundTimer.Stop();
		}
	}

	private void TeleportPlayer()
	{
		_player.GlobalPosition = _teleportPoint.GlobalPosition;

		_player.Velocity = Vector3.Zero;

		_player.GlobalRotation = _teleportPoint.GlobalRotation;
	}

	private void UpdateTimerUI()
	{
		if (_endRoundTimer == null || _timerLabel == null) return;

		double timeLeft = _endRoundTimer.TimeLeft;

		int minutes = (int)(timeLeft / 60);
		int seconds = (int)(timeLeft % 60);

		int milliseconds = (int)((timeLeft - Math.Floor(timeLeft)) * 1000);

		_timerLabel.Text = string.Format("{0:D2}:{1:D2}:{2:D3}", minutes, seconds, milliseconds);
	}

	private void OnTransitionFinished(StringName animName)
	{
		if (animName == "TransitionRound")
		{
			_animationPlayer.AnimationFinished -= OnTransitionFinished;
			ShowPowerUi();
		}
	}
}
