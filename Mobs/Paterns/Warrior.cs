using Godot;
using System;

public partial class Warrior : Mobs
{
	public override void _Ready()
	{
		Health = 10;
		speed = 5f;
		DetectionRange = 10.0f;
		AttackRange = 2.0f;
		AttackCooldown = 2.5f;
		AttackDamage = 4;
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
	}
}
