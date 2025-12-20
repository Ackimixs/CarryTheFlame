using Godot;
using System;

public partial class Minion : CharacterBody3D
{
	[Export]
	private int Health = 3;
	[Export]
	private float speed = 10f;
	private AnimationTree animationTree;
	private NavigationAgent3D  navigationAgent ;
	private Player player;
	private State CurrentState;

	[Export] private float DetectionRange = 20.0f;

	[Signal] public delegate void OnKilledEventHandler(Minion minion);

	private bool _everSeenPlayer = false;

	public override void _Ready()
	{
		animationTree = GetNode<AnimationTree>("%AnimationTree");
		navigationAgent = GetNode<NavigationAgent3D>("%NavigationAgent3D");
		player = GetTree().GetFirstNodeInGroup("Player") as Player;
		if (player == null)
		{
			GD.PrintErr("Erreur : Joueur non trouvé dans le groupe 'Player' !");
		}
		CurrentState = State.Idle;
	}

	public void TakeDamage()
	{
		if (Health <= 0)
		{
			return;
		}

		Health -= 1;
		animationTree.Set("parameters/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);

		if (Health == 0)
		{
			EmitSignal(SignalName.OnKilled, this);
			QueueFree();
		}
	}

	public enum State
	{
		Idle,
		LookingForPlayer,
	}
	
	private bool IsPlayerInNavigationRegion()
	{
		var closestPoint = NavigationServer3D.MapGetClosestPoint(navigationAgent.GetNavigationMap(), player.GlobalPosition);
		return Mathf.IsZeroApprox(closestPoint.X - player.GlobalPosition.X) && Mathf.IsZeroApprox(closestPoint.Z - player.GlobalPosition.Z);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_everSeenPlayer)
		{
			float distanceToPlayer = GlobalPosition.DistanceTo(player.GlobalPosition);
			if (distanceToPlayer <= DetectionRange && IsPlayerInNavigationRegion())
			{
				_everSeenPlayer = true;
			}
			else
			{
				return;
			}
		}

		switch (CurrentState)
		{
			case State.Idle:
				if (IsPlayerInNavigationRegion())
				{
					CurrentState = State.LookingForPlayer;
				}
				break;
			case State.LookingForPlayer:
				if (!IsPlayerInNavigationRegion())
				{
					CurrentState = State.Idle;
					Velocity = Vector3.Zero;
				}
				else
				{
					LookForPlayer();
				}
				break;
		}
	}
	
	private void LookForPlayer()
	{
		navigationAgent.TargetPosition = player.GlobalPosition;

		Vector3 nextPathPos = navigationAgent.GetNextPathPosition();
		Vector3 direction = GlobalPosition.DirectionTo(nextPathPos);

		Vector3 playerDirection = GlobalPosition.DirectionTo(player.GlobalPosition);
		playerDirection.Y = 0;

		if (playerDirection.Length() > 0.1f)
		{
			LookAt(GlobalPosition - playerDirection, Vector3.Up);
		}

		Velocity = direction * speed;
		MoveAndSlide();
	}
	
	public void _OnTimerTimeout()
	{
		navigationAgent.TargetPosition = player.GlobalPosition;
	}
}
