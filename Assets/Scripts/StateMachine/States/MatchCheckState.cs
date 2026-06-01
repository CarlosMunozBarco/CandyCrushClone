using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchCheckState : IGameState
{
    public void Enter(GameStateMachine m)
    {
        m.ActiveCoroutine = m.StartCoroutine(CheckCoroutine(m));
    }

    public void Update(GameStateMachine m) { }

    public void Exit(GameStateMachine m)
    {
        if (m.ActiveCoroutine != null) m.StopCoroutine(m.ActiveCoroutine);
    }

    private IEnumerator CheckCoroutine(GameStateMachine m)
    {
        yield return null;

        HashSet<Vector2Int> matches = m.IsPostSwapCheck
            ? m.matchFinder.FindMatchesForSwap(m.boardManager, m.PendingSwapA, m.PendingSwapB)
            : m.matchFinder.FindAllMatches(m.boardManager);

        if (m.IsPostSwapCheck)
            AddSwapActivatedSpecials(m, matches);

        if (matches.Count > 0)
        {
            m.CurrentMatches = matches;
            m.TransitionTo(m.StateResolving);
        }
        else if (m.IsPostSwapCheck)
        {
            yield return UndoSwapCoroutine(m);
            m.TransitionTo(m.StateIdle);
        }
        else
        {
            m.TransitionTo(m.StateIdle);
        }
    }

    private void AddSwapActivatedSpecials(GameStateMachine m, HashSet<Vector2Int> matches)
    {
        foreach (Vector2Int pos in new[] { m.PendingSwapA, m.PendingSwapB })
        {
            CandyCell cell = m.boardManager.GetCell(pos);
            if (cell == null || cell.IsEmpty) continue;

            CandySpecialBehaviour beh = cell.Candy.SpecialBehaviour;
            if (beh == null || !beh.ActivatesOnSwap) continue;

            matches.Add(pos);
            foreach (Vector2Int affected in beh.GetAffectedPositions(pos, m.boardManager.Columns, m.boardManager.Rows))
                matches.Add(affected);
        }
    }

    // Tras SwapState los datos estan: cellA=candyB, cellB=candyA.
    // Transforms: candyA.transform=worldB, candyB.transform=worldA.
    // Para deshacer: revertir datos, animar cada candy a su worldPos original.
    private IEnumerator UndoSwapCoroutine(GameStateMachine m)
    {
        Vector2Int posA = m.PendingSwapA;
        Vector2Int posB = m.PendingSwapB;

        CandyBehaviour candyAtA = m.boardManager.GetCell(posA)?.Candy;
        CandyBehaviour candyAtB = m.boardManager.GetCell(posB)?.Candy;

        Vector3 worldA = m.boardManager.GridToWorld(posA);
        Vector3 worldB = m.boardManager.GridToWorld(posB);

        m.boardManager.SwapCandies(posA, posB);

        float dur = m.boardManager.Config.swapDuration;
        yield return AnimationHelper.WaitForAll(m, new IEnumerator[]
        {
            candyAtA != null ? candyAtA.UndoMove(worldB, dur) : EmptyRoutine(),
            candyAtB != null ? candyAtB.UndoMove(worldA, dur) : EmptyRoutine()
        });
    }

    private IEnumerator EmptyRoutine() { yield break; }
}
