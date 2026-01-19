using Godot;
using System;

public partial class Chest : Node3D
{
	[Export] public Area3D Area;
	[Export] public AnimationPlayer AnimationPlayer;
	[Export] public Godot.Collections.Array<PackedScene> ContainedWeapon;
	[Export] public AudioStreamPlayer3D AudioPlayer;

	[Export] public bool IsActive = true;

	protected Player Player;

	protected bool HasGivenWeapon = false;

	public override void _Ready()
	{
		Area.BodyEntered += OnBodyEntered;
		Area.BodyExited += OnBodyExited;
		AudioPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");

	}

	public override void _Process(double delta)
	{
	}

	public void GiveWeaponToPlayer()
	{
		if (ContainedWeapon.Count > 0 && Player != null && !HasGivenWeapon)
		{
			int randomIndex = (int) GD.Randi() % ContainedWeapon.Count;

			randomIndex = Math.Clamp(randomIndex, 0, ContainedWeapon.Count - 1);

			PackedScene weaponScene = ContainedWeapon[randomIndex];
			Player.AddWeapon(weaponScene);
			HasGivenWeapon = true;
		}
	}

	public void DisableChest(bool forceDisable = false)
	{
		if (HasGivenWeapon || forceDisable)
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
				AudioPlayer.Play();
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
