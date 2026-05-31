using System.Collections;
using UnityEngine;

public class SwapState : IGameState
{
    public void Enter(GameStateMachine m)
    {
        m.ActiveCoroutine = m.StartCoroutine(SwapCoroutine(m));
    }

    public void Update(GameStateMachine m) { }

    public void Exit(GameStateMachine m)
    {
        if (m.ActiveCoroutine != null) m.StopCoroutine(m.ActiveCoroutine);
    }

    private IEnumerator SwapCoroutine(GameStateMachine m)
    {
        Vector2Int posA = m.PendingSwapA;
        Vector2Int posB = m.PendingSwapB;

        CandyBehaviour candyA = m.boardManager.GetCell(posA)?.Candy;
        CandyBehaviour candyB = m.boardManager.GetCell(posB)?.Candy;

        if (candyA == null || candyB == null) { m.TransitionTo(m.StateIdle); yield break; }

        Vector3 worldA = m.boardManager.GridToWorld(posA);
        Vector3 worldB = m.boardManager.GridToWorld(posB);
        float   dur    = m.boardManager.Config.swapDuration;

        yield return AnimationHelper.WaitForAll(m, new IEnumerator[]
        {
            candyA.MoveTo(worldB, dur),
            candyB.MoveTo(worldA, dur)
        });

        m.boardManager.SwapCandies(posA, posB);
        m.IsPostSwapCheck = true;
        m.TransitionTo(m.StateMatch);
    }
}
