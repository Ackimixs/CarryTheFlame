using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export]
	private Vector2 mouseSensitivity = new Vector2(0.1f, 0.1f);
	[Export]
	private float speed = 25f;
	private Camera3D camera;
	[Export]
	private float gravity = 20f;
	[Export]
	private float jumpVelocity = 10f;
	
	public override void _Ready()
	{
		camera = GetNode<Camera3D>("%Camera3D");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	
	public override void _UnhandledInput(InputEvent e)
	{
		if (e is InputEventMouseMotion mouseMotion)
		{
			RotateY(-mouseMotion.Relative.X * mouseSensitivity.X);
			camera.RotationDegrees = new Vector3(
				Mathf.Clamp(camera.RotationDegrees.X - mouseMotion.Relative.Y * mouseSensitivity.Y, -80f, 80f),
					camera.RotationDegrees.Y,
					camera.RotationDegrees.Z
			);
		}
		if (e.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 inputDirection2D = Input.GetVector("move_left", "move_right", "move_back", "move_forward");
		Vector3 inputDirection3D = new Vector3(
			inputDirection2D.X, 0.0f, -inputDirection2D.Y
		);

		Vector3 playerVelocity = Transform.Basis * inputDirection3D * speed;
		playerVelocity.Y = Velocity.Y - gravity * (float)delta;
		
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			playerVelocity.Y = jumpVelocity;
		}
		Velocity = playerVelocity;
		MoveAndSlide();
	}
}
