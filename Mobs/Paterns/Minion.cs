using Godot;
using System;

public partial class Minion : CharacterBody3D
{
	[Export]
	private int Health = 3;
	[Export]
	private float speed = 10f;
	[Export]
	private float DetectionRange = 20.0f;
	[Export]
	private float AttackRange = 2.0f;
	[Export]
	private float AttackCooldown = 1.2f;
	[Export]
	private int AttackDamage = 1;
	private double _attackTimer = 0.0;
	
	private AnimationTree animationTree;
	private NavigationAgent3D  navigationAgent ;
	private Player player;
	private State CurrentState;

	[Signal]
	public delegate void OnKilledEventHandler(Minion minion);

	private bool _everSeenPlayer = false;

	public override void _Ready()
	{
		animationTree = GetNode<AnimationTree>("%AnimationTree");
		animationTree.Active = true;
		navigationAgent = GetNode<NavigationAgent3D>("%NavigationAgent3D");
		player = GetTree().GetFirstNodeInGroup("Player") as Player;
		if (player == null)
		{
			GD.PrintErr("Erreur : Joueur non trouvé dans le groupe 'Player' !");
		}
		CurrentState = State.Idle;
	}

	public void TakeDamage(int damages)
	{
		if (Health <= 0)
			return;

		Health -= damages;
		animationTree.Set("parameters/Get_Hit/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
		
		if (Health <= 0)
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
		_attackTimer -= delta;
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
					animationTree.Set("parameters/Idle_Walking_Transition/transition_request", "Walking");
					CurrentState = State.LookingForPlayer;

				}
				break;
			case State.LookingForPlayer:
				float distanceToPlayer = GlobalPosition.DistanceTo(player.GlobalPosition);
				if (!IsPlayerInNavigationRegion())
				{
					animationTree.Set("parameters/Idle_Walking_Transition/transition_request", "Idle");
					CurrentState = State.Idle;
					Velocity = Vector3.Zero;
				}
				else if (distanceToPlayer <= AttackRange && _attackTimer <= 0.0)
				{
					AttackPlayer();
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
	
	private void AttackPlayer()
	{
		Velocity = Vector3.Zero;

		Vector3 dir = GlobalPosition.DirectionTo(player.GlobalPosition);
		dir.Y = 0;
		if (dir.Length() > 0.1f)
			LookAt(GlobalPosition - dir, Vector3.Up);

		animationTree.Set("parameters/Is_Attacking/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);

		_attackTimer = AttackCooldown;

		player.TakeDamage(AttackDamage);
	}
	
	public void _OnTimerTimeout()
	{
		navigationAgent.TargetPosition = player.GlobalPosition;
	}
}
