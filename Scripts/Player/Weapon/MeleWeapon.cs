using Godot;
using System;

public partial class MeleWeapon : Weapon
{
	[Export] private ShapeCast3D _hitCast;

	public override void _Process(double delta)
	{
	}

	protected override void HandleShoot()
	{
		AttackMelee();
	}

	protected void AttackMelee()
	{
		PlayAnimation("local/shoot");
	}

	public void CheckHit()
	{
		_hitCast.ForceShapecastUpdate();

		int hitCount = _hitCast.GetCollisionCount();

		for (int i = 0; i < hitCount; i++)
		{
			var collider = _hitCast.GetCollider(i);

			if (collider is Hitbox box)
			{
				Mobs mob = box.GetParentOrNull<Mobs>();
				if (mob != null)
				{
					mob.TakeDamage(Damage * box.DamageMultiplier);
				}
			}
			else if (collider is Pumpkin pumpkin)
			{
				pumpkin.QueueFree();
			}
		}
	}
}
