using Godot;
using System;

public partial class Bow : LongRangeWeapon
{
	private MeshInstance3D arrow;

	public override void _Ready()
	{
		arrow = GetNode<MeshInstance3D>("%arrow");
		if (arrow != null) arrow.Hide();
	}

	protected override void HandleShoot()
	{
		HandleBowLogic();
	}

	private void HandleBowLogic()
	{
		if (Input.IsActionJustPressed("shoot"))
		{
			if (arrow != null) arrow.Show();
			PlayAnimation("local/charge");
		}

		if (Input.IsActionJustReleased("shoot"))
		{
			if (arrow != null) arrow.Hide();
			Shoot();
		}
	}
}
