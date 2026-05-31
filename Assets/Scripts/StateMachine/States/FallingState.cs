using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : IGameState
{
    public void Enter(GameStateMachine m)
    {
        m.ActiveCoroutine = m.StartCoroutine(FallCoroutine(m));
    }

    public void Update(GameStateMachine m) { }

    public void Exit(GameStateMachine m)
    {
        if (m.ActiveCoroutine != null) m.StopCoroutine(m.ActiveCoroutine);
    }

    private IEnumerator FallCoroutine(GameStateMachine m)
    {
        while (m.gravityResolver.HasPendingFalls(m.boardManager))
            m.gravityResolver.ResolveSingleTick(m.boardManager);
        m.TransitionTo(m.StateRefill);
        yield break;
    }
}
