using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    [SerializeField] public BoardManager    boardManager;
    [SerializeField] public CandyPool       candyPool;
    [SerializeField] public MatchFinder     matchFinder;
    [SerializeField] public GravityResolver gravityResolver;
    [SerializeField] public InputHandler    inputHandler;
    [SerializeField] public GameUI          gameUI;

    public IdleState       StateIdle      { get; private set; }
    public SwapState       StateSwap      { get; private set; }
    public MatchCheckState StateMatch     { get; private set; }
    public ResolvingState  StateResolving { get; private set; }
    public FallingState    StateFalling   { get; private set; }
    public RefillState     StateRefill    { get; private set; }
    public ShuffleState    StateShuffling { get; private set; }

    public IGameState          CurrentState    { get; private set; }
    public int                 Score           { get; private set; }
    public int                 CascadeLevel    { get; set; }
    public Coroutine           ActiveCoroutine { get; set; }

    public Vector2Int          PendingSwapA    { get; set; }
    public Vector2Int          PendingSwapB    { get; set; }
    public HashSet<Vector2Int> CurrentMatches  { get; set; }
    public bool                IsPostSwapCheck { get; set; }

    private void Awake()
    {
        StateIdle      = new IdleState();
        StateSwap      = new SwapState();
        StateMatch     = new MatchCheckState();
        StateResolving = new ResolvingState();
        StateFalling   = new FallingState();
        StateRefill    = new RefillState();
        StateShuffling = new ShuffleState();
    }

    private void Start()
    {
        boardManager.BuildBoard();
        boardManager.PopulateInitial(candyPool);
        TransitionTo(StateIdle);
    }

    private void Update() => CurrentState?.Update(this);

    public void TransitionTo(IGameState next)
    {
        CurrentState?.Exit(this);
        CurrentState = next;
        CurrentState.Enter(this);
    }

    public void AddScore(int points)
    {
        Score += points;
        gameUI?.UpdateScore(Score);
    }
}
