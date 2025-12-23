using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class RoundManager : Node
{
	[Export] private PowerChoiceUI PowerChoiceUI;

	[Export] public Player _player;

	[Export] private Timer _endRoundTimer;
	[Export] private int _defaultRoundDuration = 180;

	[Export] private Marker3D _teleportPoint;

	public int _roundNumber = 1;

	[Export] private int nbEnemy = 2;
	private int lastEnemyCount;

	[Export] private Label _timerLabel;

	[Export] private AnimationPlayer _animationPlayerEndRound;
	[Export] private Label _roundEndLabel;
	[Export] private Control _roundEndControl;

	[Export] private AnimationPlayer _animationPlayerMapHazardUI;
	[Export] private Control _mapHazardUIControl;

	[Export] private ColorRect _backgroundRect;

	[Export] public Label RoundLabel;

	[Export] public MapHazardManager MapHazardManager;

	private int remainingEnemies = 0;

	[Export] private Godot.Collections.Array<MobSpawner> _mobSpawners;

	private RandomNumberGenerator _rng = new RandomNumberGenerator();

	private List<Minion> _activeMobs = new List<Minion>();

	[Export] private Godot.Collections.Dictionary<int, float> hazardWeights;

	private float mobScaleMultiplier = 1.0f;

	public bool isDNKRound;

	public override void _Ready()
	{
		StartRound();
		_roundEndControl.SetVisible(false);
		_mapHazardUIControl.SetVisible(false);
		_endRoundTimer.WaitTime = _defaultRoundDuration;
		_rng.Randomize();
	}

	public override void _Process(double delta)
	{
		UpdateTimerUI();
	}

	public void EndRound()
	{
		_player.SetHealth(_player.GetBaseHealth());

		if (_roundNumber % 5 == 0)
		{
			PowerManager.Instance.AddMaxActivePowers(1);
		}
		_roundNumber++;

		if (_animationPlayerEndRound != null)
		{
			_animationPlayerEndRound.Play("TransitionRound");
			_animationPlayerEndRound.AnimationFinished += OnRoundEndTransitionFinished;
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
		if (isDNKRound)
		{
			_roundEndLabel.SetText("Vous avez survécu à cette journée ...\nLes enemies non tués ne reviendront PAS demain !");
		}
		else
		{
			_roundEndLabel.SetText("Vous n'avez pas survécu à cette journée ...\nLes enemies non tués reviendront demain !");
		}
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
		_backgroundRect.SetVisible(false);
		RoundLabel.SetText("Journée " + _roundNumber);

		MapHazardManager.DeactivateAllHazards();

		if (_roundNumber >= 5 && _rng.RandiRange(0, 20) < _roundNumber)
		{
			MapHazardManager.ActivateRandomHazard(GetHazardWeightedRandomNumber());

			if (_animationPlayerMapHazardUI != null)
			{
				_animationPlayerMapHazardUI.Play("MapHazardUI");
				_animationPlayerMapHazardUI.AnimationFinished += OnMapHazardUITransitionFinished;
			}
		}
		else
		{
			SpawnEnemies();
		}
	}

	public void SpawnEnemies()
	{
		for (int i = 0; i < nbEnemy + remainingEnemies; i++)
		{
			int randomIndex = _rng.RandiRange(0, _mobSpawners.Count - 1);
			var spawner = _mobSpawners[randomIndex];
			Minion mob = spawner.SpawnMob();
			mob.SetScale(mob.Scale * mobScaleMultiplier);
			_activeMobs.Add(mob);
			mob.OnKilled += OnEnemyKilled;
		}

		remainingEnemies = nbEnemy;

		_endRoundTimer.Start();
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

	private void OnRoundEndTransitionFinished(StringName animName)
	{
		if (animName == "TransitionRound")
		{
			_animationPlayerEndRound.AnimationFinished -= OnRoundEndTransitionFinished;
			ShowPowerUi();
		}
	}

	private void OnMapHazardUITransitionFinished(StringName animName)
	{
		if (animName == "MapHazardUI")
		{
			_animationPlayerMapHazardUI.AnimationFinished -= OnMapHazardUITransitionFinished;
			SpawnEnemies();
		}
	}

	private int GetHazardWeightedRandomNumber()
	{
		float totalWeight = hazardWeights.Values.Sum();
		float roll = (float)_rng.Randf() * totalWeight;

		foreach (var kv in hazardWeights)
		{
			roll -= kv.Value;
			if (roll <= 0f)
				return kv.Key;
		}

		return hazardWeights.Keys.First();
	}

	public void SetMobScaleMultiplier(float scaleMultiplier)
	{
		mobScaleMultiplier = scaleMultiplier;
	}

	// DO NOT KILL Round
	public void StartDNKRound(int roundDuration, int enemyCount)
	{
		isDNKRound = true;
		lastEnemyCount = nbEnemy;
		nbEnemy = enemyCount;
		_endRoundTimer.WaitTime = roundDuration;
	}

	public void ResetDNKRound()
	{
		isDNKRound = false;
		nbEnemy = lastEnemyCount + (nbEnemy - remainingEnemies) + 2; // TODO change the 2 with the nb of enemies increase per round
		remainingEnemies = 0;
		_endRoundTimer.WaitTime = _defaultRoundDuration;
	}
}
