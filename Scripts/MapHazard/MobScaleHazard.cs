using Godot;

[GlobalClass]
public partial class MobScaleHazard : BaseMapHazard
{
    [Export] public float ScaleMultiplier = 1f;

    public override void ApplyHazard(RoundManager roundManager)
    {
        base.ApplyHazard(roundManager);

        roundManager.SetMobScaleMultiplier(ScaleMultiplier);
    }

    public override void RemoveHazard(RoundManager roundManager)
    {
        base.RemoveHazard(roundManager);

        roundManager.SetMobScaleMultiplier(1f);
    }
}