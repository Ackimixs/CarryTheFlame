using Godot;

[GlobalClass]
public partial class GravityMapHazard : BaseMapHazard
{
    [Export] public float GravityMultiplier = 1f;

    public override void ApplyHazard(RoundManager roundManager)
    {
        base.ApplyHazard(roundManager);

        roundManager._player.AddGravity(roundManager._player.GetBaseGravity() * (GravityMultiplier - 1));
    }

    public override void RemoveHazard(RoundManager roundManager)
    {
        base.RemoveHazard(roundManager);

        roundManager._player.AddGravity(-roundManager._player.GetBaseGravity() * (GravityMultiplier - 1));
    }
}