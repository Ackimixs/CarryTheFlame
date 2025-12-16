using Godot;

[GlobalClass]
public partial class DashPower : PowerData
{
    [Export] public float DashForce = 600f;

    public override void Apply(Node player)
    {
        GD.Print("Dash applied");
    }
}