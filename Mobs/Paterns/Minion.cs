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

	public override void _Ready()
	{
		animationTree = GetNode<AnimationTree>("%AnimationTree");
		navigationAgent = GetNode<NavigationAgent3D>("%NavigationAgent3D");
		player = GetNode<Player>("%Player");
		CurrentState = State.Idle;
		SetPhysicsProcess(false);
		NavigationServer3D.MapChanged += (_) => SetPhysicsProcess(true);
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
		switch (CurrentState)
		{
			case State.Idle:
				if (IsPlayerInNavigationRegion())
				{
					CurrentState = State.LookingForPlayer;
				}
				break;
			case State.LookingForPlayer:
				// We will code the LookForPlayer function later
				LookForPlayer();
				if (!IsPlayerInNavigationRegion())
				{
					CurrentState = State.Idle;
				}
				break;
		}
	}
	
	private void LookForPlayer()
	{
		Vector3 direction = GlobalPosition.DirectionTo(navigationAgent.GetNextPathPosition());
		Vector3 playerDirection = GlobalPosition.DirectionTo(player.Position);
		RotateY(Basis.Z.SignedAngleTo(playerDirection, Vector3.Up));

		Velocity = direction * speed;
		MoveAndSlide();
	}
	
	public void _OnTimerTimeout()
	{
		navigationAgent.TargetPosition = player.GlobalPosition;
	}
}
