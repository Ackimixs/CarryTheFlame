using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] private Vector2 mouseSensitivity = new Vector2(0.1f, 0.1f);
	[Export] private float sprintSpeed = 12f;
	private float currentSpeed;
	[Export] private float speed = 8f;
	[Export] private float gravity = 20f;
	[Export] private float jumpVelocity = 10f;
	
	// Paramètres pour les marches
	[Export] private float maxStepHeight = 0.5f;
	private float verticalVelocity = 0;
	
	private Camera3D camera;

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
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_back", "move_forward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, -inputDir.Y)).Normalized();

		// Gestion du Sprint
		// Assure-toi d'avoir configuré "sprint" dans tes Input Map ou utilise KeyList.Shift
		if (Input.IsActionPressed("sprint") && inputDir.Y > 0) // On sprint seulement si on avance
		{
			currentSpeed = sprintSpeed;
		}
		else
		{
			currentSpeed = speed;
		}

		Vector3 velocity = Velocity;

		// Gravité
		if (!IsOnFloor())
			verticalVelocity -= gravity * (float)delta;
		else
			verticalVelocity = 0;

		// Saut
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			verticalVelocity = jumpVelocity;

		// Mouvement horizontal (on utilise currentSpeed ici)
		Vector3 horizontalVelocity = direction * currentSpeed;
		velocity.X = horizontalVelocity.X;
		velocity.Z = horizontalVelocity.Z;
		velocity.Y = verticalVelocity;

		// Application du Step Climb
		if (direction.Length() > 0)
		{
			ApplyStepClimb(ref velocity, direction, (float)delta);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void ApplyStepClimb(ref Vector3 currentVelocity, Vector3 direction, float delta)
	{
		// On ne monte des marches que si on est au sol ou presque
		if (!IsOnFloor() && verticalVelocity > 0) return;

		var params3D = new PhysicsTestMotionParameters3D();
		params3D.From = GlobalTransform;
		params3D.Motion = direction * currentVelocity.Length() * delta;
		
		var result = new PhysicsTestMotionResult3D();

		// Si on touche un obstacle devant nous
		if (PhysicsServer3D.BodyTestMotion(GetRid(), params3D, result))
		{
			// 1. On tente de monter (Levage virtuel)
			Vector3 stepUpDist = Vector3.Up * maxStepHeight;
			params3D.From = GlobalTransform;
			params3D.Motion = stepUpDist;
			
			// Si on peut monter sans heurter un plafond
			if (!PhysicsServer3D.BodyTestMotion(GetRid(), params3D, result))
			{
				// 2. On avance une fois en haut
				Transform3D testTransform = GlobalTransform;
				testTransform.Origin += stepUpDist;
				params3D.From = testTransform;
				params3D.Motion = direction * speed * delta;
				
				if (!PhysicsServer3D.BodyTestMotion(GetRid(), params3D, result))
				{
					// 3. On redescend pour se poser sur la marche
					testTransform.Origin += params3D.Motion;
					params3D.From = testTransform;
					params3D.Motion = -stepUpDist;

					if (PhysicsServer3D.BodyTestMotion(GetRid(), params3D, result))
					{
						// On calcule la distance réellement montée
						float climbedAmount = maxStepHeight + result.GetTravel().Y;
						if (climbedAmount > 0.01f)
						{
							// On téléporte légèrement le joueur vers le haut pour "enjamber"
							GlobalTransform = new Transform3D(GlobalTransform.Basis, GlobalTransform.Origin + Vector3.Up * climbedAmount);
						}
					}
				}
			}
		}
	}
}
