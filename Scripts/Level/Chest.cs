using Godot;
using System;

public partial class Chest : Node3D
{
	[Export] public Area3D Area;
	[Export] public AnimationPlayer AnimationPlayer;
	[Export] public Godot.Collections.Array<PackedScene> ContainedWeapon;

	[Export] public bool IsActive = true;

	protected Player Player;

	protected bool HasGivenWeapon = false;

	public override void _Ready()
	{
		Area.BodyEntered += OnBodyEntered;
		Area.BodyExited += OnBodyExited;
	}

	public override void _Process(double delta)
	{
	}

	public void GiveWeaponToPlayer()
	{
		if (ContainedWeapon.Count > 0 && Player != null && !HasGivenWeapon)
		{
			int randomIndex = (int) GD.Randi() % ContainedWeapon.Count;
			GD.Print("Giving weapon index: " + randomIndex);
			PackedScene weaponScene = ContainedWeapon[randomIndex];
			Player.AddWeapon(weaponScene);
			HasGivenWeapon = true;
		}
	}

	public void DeleteChest()
	{
		if (HasGivenWeapon)
		{
			IsActive = false;
			Hide();
		}
	}

	public void ActivateChest()
	{
		IsActive = true;
		HasGivenWeapon = false;
		Show();
	}

	public void OnBodyEntered(Node body)
	{
		if (IsActive)
		{
			if (body is Player player)
			{
				AnimationPlayer.Play("open-close");
				Player = player;
			}
		}
	}

	public void OnBodyExited(Node body)
	{
		if (IsActive)
		{
			if (body is Player)
			{
				Player = null;
			}
		}
	}
}
