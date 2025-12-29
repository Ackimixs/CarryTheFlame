using Godot;
using System;

public partial class Mage : Mobs
{
	[Export]
	private PackedScene PumpkinScene;
	private Marker3D shootPoint;

	public override void _Ready()
	{
		Health = 3;
		speed = 6f;
		DetectionRange = 25.0f;
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

		// Set position
		projectile.GlobalTransform = shootPoint.GlobalTransform;

		// Aim toward player
		Vector3 direction = (player.GlobalPosition - shootPoint.GlobalPosition).Normalized();
		projectile.LookAt(shootPoint.GlobalPosition + direction, Vector3.Up);

		// Add to scene (important!)
		GetTree().CurrentScene.AddChild(projectile);
		projectile.Initialize(player);
	}
}
