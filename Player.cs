using Godot;
using System;

public class StepResult
{
    public Vector3 DiffPosition = Vector3.Zero;
    public Vector3 Normal = Vector3.Zero;
    public bool IsStepUp = false;
}


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
	[Export] public float StepHeight = 0.6f;
	[Export] public float MaxStepSlope = 40.0f;
	[Export] public int StepCheckCount = 2;

	
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
		// Vector2 inputDirection2D = Input.GetVector("move_left", "move_right", "move_back", "move_forward");
		/*Vector3 inputDirection3D = new Vector3(
			inputDirection2D.X, 0.0f, -inputDirection2D.Y
		);*/

		Vector2 input = Input.GetVector("move_left", "move_right", "move_back", "move_forward");
		Vector3 direction = (Transform.Basis * new Vector3(input.X, 0, -input.Y)).Normalized();

		Vector3 horizontalVelocity = direction * speed;
		Vector3 velocity = Velocity;
		velocity.Y -= gravity * (float)delta;
		
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = jumpVelocity;

		StepResult stepResult = new StepResult();
		if (StepCheck(delta, horizontalVelocity, stepResult))
		{
			GlobalTransform = GlobalTransform.Translated(stepResult.DiffPosition);
			velocity.Y = 0;
		}
		velocity.X = horizontalVelocity.X;
		velocity.Z = horizontalVelocity.Z;

		Velocity = velocity;
		MoveAndSlide();
	}

	private bool StepCheck(double delta, Vector3 horizontalVelocity, StepResult result)
	{
		if (!IsOnFloor())
			return false;

		Vector3 stepHeightVec = Vector3.Up * StepHeight;
		Vector3 stepIncrement = stepHeightVec / StepCheckCount;

		for (int i = 0; i < StepCheckCount; i++)
		{
			Vector3 stepOffset = stepHeightVec - stepIncrement * i;

			// 1️⃣ Test UP
			Transform3D upTransform = GlobalTransform;
			upTransform.Origin += stepOffset;

			// 2️⃣ Test FORWARD
			var forwardParams = new PhysicsTestMotionParameters3D
			{
				From = upTransform,
				Motion = horizontalVelocity * (float)delta
			};

			var forwardResult = new PhysicsTestMotionResult3D();

			if (PhysicsServer3D.BodyTestMotion(GetRid(), forwardParams, forwardResult))
				continue;

			upTransform.Origin += forwardParams.Motion;

			// 3️⃣ Test DOWN
			var downParams = new PhysicsTestMotionParameters3D
			{
				From = upTransform,
				Motion = -stepOffset
			};

			var downResult = new PhysicsTestMotionResult3D();

			if (PhysicsServer3D.BodyTestMotion(GetRid(), downParams, downResult))
			{
				float slopeAngle = downResult
					.GetCollisionNormal()
					.AngleTo(Vector3.Up);

				if (slopeAngle <= Mathf.DegToRad(MaxStepSlope))
				{
					result.IsStepUp = true;
					result.DiffPosition = -downResult.GetRemainder();
					result.Normal = downResult.GetCollisionNormal();
					return true;
				}
			}
		}

		return false;
	}

}
