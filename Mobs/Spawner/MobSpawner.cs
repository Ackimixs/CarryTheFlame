using Godot;
using System;

public partial class MobSpawner : Node3D
{
    [Export] private CollisionShape3D _spawnArea;
    private RandomNumberGenerator _rng = new RandomNumberGenerator();

    [Export] private Godot.Collections.Array<PackedScene> _mobScenes;

    public override void _Ready()
    {
    }

    public Mobs SpawnMob()
    {
        int mobIndex = _rng.RandiRange(0, _mobScenes.Count - 1);
        PackedScene mobScene = _mobScenes[mobIndex];

        Mobs mob = mobScene.Instantiate<Mobs>();

        Vector3 randomPosition = GetRandomPositionInBox();

        AddChild(mob);
        mob.GlobalPosition = GlobalPosition + randomPosition;

        return mob;
    }

    private Vector3 GetRandomPositionInBox()
    {
        BoxShape3D shape = (BoxShape3D)_spawnArea.Shape;
        Vector3 size = shape.Size;

        float x = _rng.RandfRange(-size.X / 2, size.X / 2);
        float y = 0;
        float z = _rng.RandfRange(-size.Z / 2, size.Z / 2);

        return new Vector3(x, y, z);
    }
}