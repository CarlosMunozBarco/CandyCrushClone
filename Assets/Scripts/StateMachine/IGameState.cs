public interface IGameState
{
    void Enter(GameStateMachine m);
    void Update(GameStateMachine m);
    void Exit(GameStateMachine m);
}
