public class PlayerStateMachine
{
    public PlayerStateBase CurrentState { get; private set; }

    public void Initialize(PlayerStateBase initialState)
    {
        CurrentState = initialState;
        CurrentState.Enter();
    }

    public void ChangeState(PlayerStateBase newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
