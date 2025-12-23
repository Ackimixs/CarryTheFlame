using Godot;

public partial class BaseMapHazard : Resource
{
    [Export] public string Id;
    [Export] public string DisplayName;
    [Export] public string Description;

    public virtual void ApplyHazard(RoundManager roundManager)
    {
        GD.Print($"Apply map hazard: {DisplayName}");
    }

    public virtual void RemoveHazard(RoundManager roundManager)
    {
        GD.Print($"Remove map hazard: {DisplayName}");
    }

}
