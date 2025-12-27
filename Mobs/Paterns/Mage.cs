using Godot;
using System;

public partial class Mage : Mobs
{
    private AnimationTree animationTree;
	private NavigationAgent3D  navigationAgent ;
	private State CurrentState;

	private bool _everSeenPlayer = false;
	
	[Export]
	private PackedScene PumpkinScene;
	private Marker3D shootPoint;

	public override void _Ready()
	{
		Health = 3;
		speed = 5f;
		DetectionRange = 25.0f;
		AttackRange = 10f;
		AttackCooldown = 10f;
		AttackDamage = 1;
		_attackTimer = 0.0;
		
		animationTree = GetNode<AnimationTree>("%AnimationTree");
		animationTree.Active = true;
		
		navigationAgent = GetNode<NavigationAgent3D>("%NavigationAgent3D");
		navigationAgent.TargetDesiredDistance = AttackRange - 0.2f;
		player = GetTree().GetFirstNodeInGroup("Player") as Player;
		if (player == null)
		{
			GD.PrintErr("Erreur : Joueur non trouvé dans le groupe 'Player' !");
		}
		CurrentState = State.Idle;
		shootPoint = GetNode<Marker3D>("ShootPoint");
	}

	public override void TakeDamage(int damages)
	{
		animationTree.Set("parameters/Get_Hit/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
		base.TakeDamage(damages);
	}

	public enum State
	{
		Idle,
		Moving,
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
					CurrentState = State.Moving;

				}
				break;
			case State.Moving:
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
		
		if (navigationAgent.IsNavigationFinished())
		{
			Velocity = Vector3.Zero;
			return;
		}

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
		Shoot();
		_attackTimer = AttackCooldown;

	}
	
	private void Shoot()
	{
		if (PumpkinScene == null || player == null)
			return;
		
		Pumpkin projectile = PumpkinScene.Instantiate<Pumpkin>();

		// Set position
		projectile.GlobalTransform = shootPoint.GlobalTransform;

		// Aim toward player
		Vector3 direction = (player.GlobalPosition - shootPoint.GlobalPosition).Normalized();
		projectile.LookAt(shootPoint.GlobalPosition + direction, Vector3.Up);

		// Add to scene (important!)
		GetTree().CurrentScene.AddChild(projectile);
	}
	
	public void _OnTimerTimeout()
	{
		navigationAgent.TargetPosition = player.GlobalPosition;
	}
}
