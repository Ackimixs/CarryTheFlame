using Godot;
using System;
using System.Data;

public partial class Minion : Mobs
{
	public override void _Ready()
	{
		Health = 4;
		speed = 7f;
		DetectionRange = 15.0f;
		AttackRange = 2.0f;
		AttackCooldown = 2.0f;
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
	}
}
