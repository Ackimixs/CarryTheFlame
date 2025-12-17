using Godot;

public partial class TargetStatue : StaticBody3D
{
	// On expose le mur à déplacer dans l'inspecteur
	[Export] public MovingWall WallToMove;

	public void OnHit()
	{
		// 1. Déclenche le mouvement du mur
		if (WallToMove != null)
		{
			WallToMove.OpenWall();
		}

		// 2. Effet visuel ou destruction
		// Tu peux ajouter des particules ici avant de supprimer la statue
		QueueFree(); 
	}
}
