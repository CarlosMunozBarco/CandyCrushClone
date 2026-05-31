using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleState : IGameState
{
    public void Enter(GameStateMachine m)
    {
        m.ActiveCoroutine = m.StartCoroutine(ShuffleCoroutine(m));
    }

    public void Update(GameStateMachine m) { }

    public void Exit(GameStateMachine m)
    {
        if (m.ActiveCoroutine != null) m.StopCoroutine(m.ActiveCoroutine);
    }

    private IEnumerator ShuffleCoroutine(GameStateMachine m)
    {
        m.gameUI.ShowNoMovesMessage();
        yield return new WaitForSeconds(1.5f);

        // Collect all active non-empty candies
        var candies   = new List<CandyBehaviour>();
        var positions = new List<Vector2Int>();

        for (int row = 0; row < m.boardManager.Rows; row++)
        for (int col = 0; col < m.boardManager.Columns; col++)
        {
            CandyCell cell = m.boardManager.GetCell(col, row);
            if (cell == null || !cell.IsActive || cell.IsEmpty) continue;
            candies.Add(cell.Candy);
            positions.Add(cell.GridPos);
        }

        // Shuffle until at least one valid move exists (max 100 attempts)
        int attempts = 0;
        do
        {
            FisherYatesShuffle(positions);

            // Clear all active cells without returning candies to pool
            for (int i = 0; i < candies.Count; i++)
                m.boardManager.GetCell(positions[i])?.RemoveCandy();

            // Re-assign candies to shuffled positions
            for (int i = 0; i < candies.Count; i++)
                m.boardManager.GetCell(positions[i])?.SetCandy(candies[i]);

            attempts++;
        }
        while (!m.matchFinder.HasAnyValidMove(m.boardManager) && attempts < 100);

        // Animate all candies flying to their new world positions
        var anims = new List<IEnumerator>();
        foreach (CandyBehaviour candy in candies)
            anims.Add(candy.MoveTo(m.boardManager.GridToWorld(candy.GridPos), 0.5f));
        yield return AnimationHelper.WaitForAll(m, anims);

        m.gameUI.HideNoMovesMessage();
        m.TransitionTo(m.StateIdle);
    }

    private static void FisherYatesShuffle(List<Vector2Int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
