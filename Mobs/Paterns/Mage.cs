using Godot;
using System;

public partial class Mage : Mobs
{
	[Export]
	private PackedScene PumpkinScene;
	private Marker3D shootPoint;

	public override void _Ready()
	{
		Health = 10;
		speed = 6f;
		DetectionRange = 1000.0f;
		AttackRange = 10f;
		AttackCooldown = 7f;
		AttackDamage = 2;
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

	
	public override void AttackPlayer()
	{
		base.AttackPlayer();
		Shoot();
	}
	
	private void Shoot()
	{
		if (PumpkinScene == null || player == null)
			return;
		
		Pumpkin projectile = PumpkinScene.Instantiate<Pumpkin>();

		projectile.GlobalTransform = shootPoint.GlobalTransform;

		GetTree().CurrentScene.AddChild(projectile);
		projectile.Initialize(player);
	}
}
