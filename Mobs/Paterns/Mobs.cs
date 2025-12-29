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
    protected int AttackDamage;
    protected double _attackTimer;
    protected NavigationAgent3D  navigationAgent ;
    protected AnimationTree animationTree;
    protected State CurrentState;
    protected bool _everSeenPlayer;
    
    public Player player;
    
    public enum State
    {
        Idle,
        Moving,
    }

    [Signal]
    public delegate void OnKilledEventHandler(Mobs mob);
    
    public override void _PhysicsProcess(double delta)
    {
        _attackTimer -= delta;
        if (!_everSeenPlayer)
        {
            float distanceToPlayer = GlobalPosition.DistanceTo(player.GlobalPosition);
            if (distanceToPlayer <= DetectionRange && IsPlayerInNavigationRegion())
            {
                _everSeenPlayer = true;
            }
            else
            {
                return;
            }
        }

        switch (CurrentState)
        {
            case State.Idle:
                if (IsPlayerInNavigationRegion())
                {
                    animationTree.Set("parameters/Idle_Walking_Transition/transition_request", "Walking");
                    CurrentState = State.Moving;
                }
                break;
            case State.Moving:
                float distanceToPlayer = GlobalPosition.DistanceTo(player.GlobalPosition);
                if (!IsPlayerInNavigationRegion())
                {
                    animationTree.Set("parameters/Idle_Walking_Transition/transition_request", "Idle");
                    CurrentState = State.Idle;
                    Velocity = Vector3.Zero;
                }
                else if (distanceToPlayer <= AttackRange && _attackTimer <= 0.0)
                {
                    AttackPlayer();
                }
                else
                {
                    animationTree.Set("parameters/Idle_Walking_Transition/transition_request", "Idle");
                    LookForPlayer();
                }
                break;
        }
    }
    
    public virtual void AttackPlayer()
    {
        Velocity = Vector3.Zero;
        Vector3 dir = GlobalPosition.DirectionTo(player.GlobalPosition);
        dir.Y = 0;
        if (dir.Length() > 0.1f)
            LookAt(GlobalPosition - dir, Vector3.Up);
        animationTree.Set("parameters/Is_Attacking/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        _attackTimer = AttackCooldown;
    }

    public void DealDamageToPlayer()
    {
        player.TakeDamage(AttackDamage);
    }

    public virtual void TakeDamage(int damage)
    {
        animationTree.Set("parameters/Get_Hit/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        if (Health <= 0)
            return;
        Health -= damage;
        if (Health <= 0)
        {
            EmitSignal(SignalName.OnKilled, this);
            QueueFree();
        }
    }
    
    protected bool IsPlayerInNavigationRegion()
    {
        var closestPoint = NavigationServer3D.MapGetClosestPoint(navigationAgent.GetNavigationMap(), player.GlobalPosition);
        return Mathf.IsZeroApprox(closestPoint.X - player.GlobalPosition.X) && Mathf.IsZeroApprox(closestPoint.Z - player.GlobalPosition.Z);
    }
    
    protected void LookForPlayer()
    {
        navigationAgent.TargetPosition = player.GlobalPosition;
        if (navigationAgent.IsNavigationFinished())
        {
            Velocity = Vector3.Zero;
            return;
        }
        Vector3 nextPathPos = navigationAgent.GetNextPathPosition();
        Vector3 direction = GlobalPosition.DirectionTo(nextPathPos);
        Vector3 playerDirection = GlobalPosition.DirectionTo(player.GlobalPosition);
        playerDirection.Y = 0;
        if (playerDirection.Length() > 0.1f)
        {
            LookAt(GlobalPosition - playerDirection, Vector3.Up);
        }
        Velocity = direction * speed;
        MoveAndSlide();
    }
    
    public void _OnTimerTimeout()
    {
        navigationAgent.TargetPosition = player.GlobalPosition;
    }
}