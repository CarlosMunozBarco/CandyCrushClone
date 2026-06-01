using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefillState : IGameState
{
    public void Enter(GameStateMachine m)
    {
        m.ActiveCoroutine = m.StartCoroutine(RefillCoroutine(m));
    }

    public void Update(GameStateMachine m) { }

    public void Exit(GameStateMachine m)
    {
        if (m.ActiveCoroutine != null) m.StopCoroutine(m.ActiveCoroutine);
    }

    private IEnumerator RefillCoroutine(GameStateMachine m)
    {
        var   allAnims = new List<IEnumerator>();
        float step     = m.boardManager.Config.cellSize + m.boardManager.Config.cellSpacing;

        // Animar caramelos que cayeron lógicamente pero aún no tienen animación
        for (int col = 0; col < m.boardManager.Columns; col++)
        for (int row = 0; row < m.boardManager.Rows; row++)
        {
            CandyCell cell = m.boardManager.GetCell(col, row);
            if (cell == null || !cell.IsActive || cell.IsEmpty) continue;
            CandyBehaviour candy = cell.Candy;
            if (Vector3.Distance(candy.transform.position, cell.WorldPos) > 0.01f)
            {
                float dist = Vector3.Distance(candy.transform.position, cell.WorldPos);
                float dur  = m.boardManager.Config.fallDuration * Mathf.Max(1f, dist / step);
                allAnims.Add(candy.MoveToLinear(cell.WorldPos, dur));
            }
        }

        // Spawnear caramelos nuevos para las celdas vacías restantes
        for (int col = 0; col < m.boardManager.Columns; col++)
        {
            int spawnOffset = 0;

            for (int row = 0; row < m.boardManager.Rows; row++)
            {
                CandyCell cell = m.boardManager.GetCell(col, row);
                if (cell == null || !cell.IsActive || !cell.IsEmpty) continue;

                int     topRow     = GetTopActiveRow(m.boardManager, col);
                Vector3 spawnWorld = m.boardManager.GridToWorld(col, topRow)
                                     + Vector3.up * step * (spawnOffset + 1);
                spawnOffset++;

                int       typeIndex = Random.Range(0, m.boardManager.Config.candyDataSet.Length);
                CandyData data      = m.boardManager.Config.candyDataSet[typeIndex];

                CandyData special = m.boardManager.Config.RollSpecialCandyData(data.candyType);
                if (special != null && special.specialBehaviour is BombBehaviour)
                    Debug.Log($"[Bomb] Bomba seleccionada en refill");
                if (special != null) data = special;

                CandyBehaviour candy = m.candyPool.Get(data, spawnWorld);
                cell.SetCandy(candy);
                candy.SetSortingOrder(row);

                Vector3 target = m.boardManager.GridToWorld(candy.GridPos);
                float   dist   = Vector3.Distance(spawnWorld, target);
                float   dur    = m.boardManager.Config.fallDuration * Mathf.Max(1f, dist / step);
                allAnims.Add(candy.MoveToLinear(target, dur));
            }
        }

        if (allAnims.Count == 0)
        {
            m.IsPostSwapCheck = false;
            m.TransitionTo(m.StateMatch);
            yield break;
        }

        yield return AnimationHelper.WaitForAll(m, allAnims);

        m.IsPostSwapCheck = false;
        m.TransitionTo(m.StateMatch);
    }

    private int GetTopActiveRow(BoardManager board, int col)
    {
        for (int row = board.Rows - 1; row >= 0; row--)
        {
            CandyCell cell = board.GetCell(col, row);
            if (cell != null && cell.IsActive) return row;
        }
        return 0;
    }
}
