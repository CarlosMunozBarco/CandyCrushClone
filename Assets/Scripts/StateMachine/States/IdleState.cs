public class IdleState : IGameState
{
    private GameStateMachine _m;

    public void Enter(GameStateMachine m)
    {
        _m = m;

        if (!m.matchFinder.HasAnyValidMove(m.boardManager))
        {
            m.TransitionTo(m.StateShuffling);
            return;
        }

        m.inputHandler.IsEnabled = true;
        m.inputHandler.OnSwapRequested += OnSwap;
    }

    public void Update(GameStateMachine m) { }

    public void Exit(GameStateMachine m)
    {
        m.inputHandler.IsEnabled = false;
        m.inputHandler.OnSwapRequested -= OnSwap;
    }

    private void OnSwap(UnityEngine.Vector2Int posA, UnityEngine.Vector2Int posB)
    {
        _m.PendingSwapA = posA;
        _m.PendingSwapB = posB;
        _m.TransitionTo(_m.StateSwap);
    }
}
