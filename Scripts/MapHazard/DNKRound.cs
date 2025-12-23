using Godot;

[GlobalClass]
public partial class DNKRound : BaseMapHazard
{
    [Export] public int RoundDuration = 30;
    [Export] public int EnemyCount = 10;

    public override void ApplyHazard(RoundManager roundManager)
    {
        base.ApplyHazard(roundManager);

        roundManager.StartDNKRound(RoundDuration, EnemyCount);
    }

    public override void RemoveHazard(RoundManager roundManager)
    {
        base.RemoveHazard(roundManager);

        roundManager.ResetDNKRound();
    }
}
