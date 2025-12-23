using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] private Vector2 mouseSensitivity = new Vector2(0.1f, 0.1f);
	private float currentSpeed;
	[Export] private float baseSpeed = 8f;
	[Export] private float sprintMultiplier = 1.5f;

	[Export] private float health = 16f;
	[Export] private float gravity = 20f;
	[Export] private float jumpVelocity = 10f;
	[Export] private float acceleration = 10f;
	[Export] private float airAcceleration = 2f;
	[Export] private float baseDamage = 2f;

	private int jumpNb = 0;
	private int maxJump = 1;

	// Paramètres pour les marches
	[Export] private float maxStepHeight = 0.5f;
	private float verticalVelocity = 0;
	private int currentWeaponIndex = 0;
	private System.Collections.Generic.List<Node3D> weapons = new System.Collections.Generic.List<Node3D>();
	private Camera3D camera;
	private float damage;

	private float speed;
	private float sprintSpeed;

	[Export] public PowerManager powerManager;

	public override void _Ready()
	{
		camera = GetNode<Camera3D>("%Camera3D");
		HideCursor();

		// On récupère automatiquement Musketoon, PistolAxe et PistolManager
		foreach (Node child in camera.GetChildren())
		{
			if (child is Node3D weapon)
			{
				weapons.Add(weapon);
				weapon.Hide(); // Cache toutes les armes au départ
			}
		}

		// Affiche la première arme par défaut
		if (weapons.Count > 0) weapons[currentWeaponIndex].Show();

		damage = baseDamage;
		speed = baseSpeed;
		sprintSpeed = baseSpeed * sprintMultiplier;
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
			ShowCursor();
		}

		if (weapons.Count > 0)
		{
			if (e.IsActionPressed("next_weapon"))
			{
				ChangeWeapon(1);
			}
			else if (e.IsActionPressed("prev_weapon"))
			{
				ChangeWeapon(-1);
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_back", "move_forward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, -inputDir.Y)).Normalized();

		float targetSpeed = Input.IsActionPressed("sprint") ? sprintSpeed : speed;
		Vector3 targetVelocity = direction * targetSpeed;

		Vector3 velocity = Velocity;

		float accel = IsOnFloor() ? acceleration : airAcceleration;

		velocity.X = Mathf.Lerp(velocity.X, targetVelocity.X, accel * (float)delta);
		velocity.Z = Mathf.Lerp(velocity.Z, targetVelocity.Z, accel * (float)delta);

		if (!IsOnFloor())
		{
			verticalVelocity -= gravity * (float)delta;
		}
		else
		{
			verticalVelocity = 0;
			jumpNb = 0;
		}

		if (Input.IsActionJustPressed("jump") && (IsOnFloor() || (jumpNb < maxJump)))
		{
			verticalVelocity = jumpVelocity;
			jumpNb++;
		}

		velocity.Y = verticalVelocity;

		if (direction.Length() > 0)
		{
			ApplyStepClimb(ref velocity, direction, (float)delta);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void ApplyStepClimb(ref Vector3 currentVelocity, Vector3 direction, float delta)
	{
		if (!IsOnFloor() && verticalVelocity > 0) return;

		var params3D = new PhysicsTestMotionParameters3D();
		params3D.From = GlobalTransform;
		params3D.Motion = direction * currentVelocity.Length() * delta;
		
		var result = new PhysicsTestMotionResult3D();

		if (PhysicsServer3D.BodyTestMotion(GetRid(), params3D, result))
		{
			Vector3 stepUpDist = Vector3.Up * maxStepHeight;
			params3D.From = GlobalTransform;
			params3D.Motion = stepUpDist;

			if (!PhysicsServer3D.BodyTestMotion(GetRid(), params3D, result))
			{
				Transform3D testTransform = GlobalTransform;
				testTransform.Origin += stepUpDist;
				params3D.From = testTransform;
				params3D.Motion = direction * speed * delta;
				
				if (!PhysicsServer3D.BodyTestMotion(GetRid(), params3D, result))
				{
					testTransform.Origin += params3D.Motion;
					params3D.From = testTransform;
					params3D.Motion = -stepUpDist;

					if (PhysicsServer3D.BodyTestMotion(GetRid(), params3D, result))
					{
						float climbedAmount = maxStepHeight + result.GetTravel().Y;
						if (climbedAmount > 0.01f)
						{
							GlobalTransform = new Transform3D(GlobalTransform.Basis, GlobalTransform.Origin + Vector3.Up * climbedAmount);
						}
					}
				}
			}
		}
	}

	private void ChangeWeapon(int direction)
	{
		weapons[currentWeaponIndex].Hide(); // Cache l'arme actuelle
		
		currentWeaponIndex = (currentWeaponIndex + direction + weapons.Count) % weapons.Count;

		weapons[currentWeaponIndex].Show(); // Affiche la nouvelle arme
	}

	public void ShowCursor()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	public void HideCursor()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public void SetSpeed(float newSpeed)
	{
		speed = newSpeed;
		sprintSpeed = newSpeed * sprintMultiplier;
	}

	public void AddSpeed(float newSpeed)
	{
		speed += newSpeed;
		sprintSpeed += (newSpeed * sprintMultiplier);
	}

	public float GetSpeed()
	{
		return speed;
	}

	public float GetSprintSpeed()
	{
		return sprintSpeed;
	}

	public float GetBaseSpeed()
	{
		return baseSpeed;
	}

	public float GetSprintMultiplier()
	{
		return sprintMultiplier;
	}

	public void AddJump(int amount)
	{
		maxJump += amount;
	}

	public void TakeDamage(int attackDamage)
	{
		health -= attackDamage;

		if (health <= 0)
		{
			GD.Print("Player is dead!");
		}
	}

	public void AddDamage(float amount)
	{
		damage += amount;
	}

	public float GetDamage()
	{
		return damage;
	}

	public float GetBaseDamage()
	{
		return baseDamage;
	}

	public void SetHealth(float newHealth)
	{
		health = newHealth;
	}

	public void AddHealth(float amount)
	{
		health += amount;
	}

	public float GetHealth()
	{
		return health;
	}
}
