public abstract class PlayerStateBase
{
    protected Player Player;
    protected PlayerStateMachine StateMachine;

    protected PlayerStateBase(Player player, PlayerStateMachine stateMachine)
    {
        Player = player;
        StateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void HandleInput() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void Exit() { }
}