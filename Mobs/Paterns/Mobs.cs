using Godot;

public partial class Mobs : CharacterBody3D
{
    [Export] 
    protected int Health;
    [Export]
    protected float speed;
    [Export]
    protected float DetectionRange = 100.0f;
    [Export]
    protected float AttackRange;
    [Export]
    protected float AttackCooldown;
    [Export]
    protected int AttackDamage = 1;
    protected double _attackTimer = 0.0;
    
    public Player player;

    [Signal]
    public delegate void OnKilledEventHandler(Minion minion);

    public virtual void TakeDamage(int damage)
    {
        if (Health <= 0)
            return;

        Health -= damage;
        GD.Print($"{Name} took {damage} damage, HP = {Health}");

        if (Health <= 0)
        {
            EmitSignal(SignalName.OnKilled, this);
            QueueFree();
        }
    }
}