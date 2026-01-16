using Godot;

[GlobalClass]
public partial class ChestMapHazard : BaseMapHazard
{
    [Export] public int MaxChestsToActivate = 3;

    private int _currentChestsToActivate;

    public override void ApplyHazard(RoundManager roundManager)
    {
        base.ApplyHazard(roundManager);

        _currentChestsToActivate = roundManager.nbChestsToActivate;
        roundManager.nbChestsToActivate = MaxChestsToActivate;
    }

    public override void RemoveHazard(RoundManager roundManager)
    {
        base.RemoveHazard(roundManager);

        roundManager.nbChestsToActivate = _currentChestsToActivate;
    }
}