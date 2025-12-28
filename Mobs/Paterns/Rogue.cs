using Godot;
using System;

public partial class Rogue : Mobs
{
	[Export]
	private PackedScene ArrowScene;
	private Marker3D shootPoint;
	
	public override void _Ready()
	{
		Health = 4;
		speed = 7f;
		DetectionRange = 30.0f;
		AttackRange = 15f;
		AttackCooldown = 10f;
		AttackDamage = 3;
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
		if (ArrowScene == null || player == null)
			return;
		Arrow projectile = ArrowScene.Instantiate<Arrow>();

		// Set position
		projectile.GlobalTransform = shootPoint.GlobalTransform;

		// Aim toward player
		Vector3 direction = (player.GlobalPosition - shootPoint.GlobalPosition).Normalized();
		projectile.LookAt(shootPoint.GlobalPosition + direction, Vector3.Up);

		// Add to scene (important!)
		GetTree().CurrentScene.AddChild(projectile);
	}
}
