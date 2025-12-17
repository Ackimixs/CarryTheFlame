using Godot;
using System;

public partial class MovingWall : AnimatableBody3D
{
	[Export] public Vector3 TargetPositionOffset = new Vector3(0, 0, -6); // Faire descendre le mur
	[Export] public float Duration = 1.5f;

	public void OpenWall()
	{
		Tween tween = GetTree().CreateTween();
		// On déplace le mur vers sa position actuelle + l'offset
		tween.TweenProperty(this, "position", Position + TargetPositionOffset, Duration)
			 .SetTrans(Tween.TransitionType.Expo)
			 .SetEase(Tween.EaseType.Out);
	}
}
