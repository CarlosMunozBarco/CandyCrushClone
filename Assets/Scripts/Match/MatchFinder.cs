using System.Collections.Generic;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    public HashSet<Vector2Int> FindAllMatches(BoardManager board)
    {
        var result = new HashSet<Vector2Int>();
        for (int row = 0; row < board.Rows; row++)
        for (int col = 0; col < board.Columns; col++)
        {
            AddRunToSet(result, GetHorizontalRun(board, col, row));
            AddRunToSet(result, GetVerticalRun(board, col, row));
        }
        return result;
    }

    public HashSet<Vector2Int> FindMatchesForSwap(BoardManager board,
                                                   Vector2Int posA, Vector2Int posB)
    {
        var result = new HashSet<Vector2Int>();
        foreach (var pos in new[] { posA, posB })
        {
            AddRunToSet(result, GetHorizontalRun(board, pos.x, pos.y));
            AddRunToSet(result, GetVerticalRun(board, pos.x, pos.y));
        }
        return result;
    }

    private void AddRunToSet(HashSet<Vector2Int> set, List<Vector2Int> run)
    {
        if (run.Count >= 3) foreach (var p in run) set.Add(p);
    }

    private List<Vector2Int> GetHorizontalRun(BoardManager board, int col, int row)
    {
        CandyCell start = board.GetCell(col, row);
        if (start == null || !start.IsActive || start.IsEmpty)
            return new List<Vector2Int>();

        CandyType type = start.Candy.CandyType;
        if (start.Candy.IsSpecial && start.Candy.SpecialBehaviour.MatchesAnyColor) return new List<Vector2Int>();

        int leftCol = col;
        while (leftCol > 0 && SameType(board.GetCell(leftCol - 1, row), type))
            leftCol--;

        var run = new List<Vector2Int>();
        for (int c = leftCol; c < board.Columns; c++)
        {
            if (!SameType(board.GetCell(c, row), type)) break;
            run.Add(new Vector2Int(c, row));
        }
        return run;
    }

    private List<Vector2Int> GetVerticalRun(BoardManager board, int col, int row)
    {
        CandyCell start = board.GetCell(col, row);
        if (start == null || !start.IsActive || start.IsEmpty)
            return new List<Vector2Int>();

        CandyType type = start.Candy.CandyType;
        if (start.Candy.IsSpecial && start.Candy.SpecialBehaviour.MatchesAnyColor) return new List<Vector2Int>();

        int bottomRow = row;
        while (bottomRow > 0 && SameType(board.GetCell(col, bottomRow - 1), type))
            bottomRow--;

        var run = new List<Vector2Int>();
        for (int r = bottomRow; r < board.Rows; r++)
        {
            if (!SameType(board.GetCell(col, r), type)) break;
            run.Add(new Vector2Int(col, r));
        }
        return run;
    }

    public bool HasAnyValidMove(BoardManager board)
    {
        for (int row = 0; row < board.Rows; row++)
        for (int col = 0; col < board.Columns; col++)
        {
            CandyCell cellA = board.GetCell(col, row);
            if (cellA == null || !cellA.IsActive || cellA.IsEmpty) continue;

            // Check right neighbor
            if (col + 1 < board.Columns)
            {
                CandyCell cellB = board.GetCell(col + 1, row);
                if (cellB != null && cellB.IsActive && !cellB.IsEmpty)
                {
                    board.SwapCandies(cellA.GridPos, cellB.GridPos);
                    bool match = FindMatchesForSwap(board, cellA.GridPos, cellB.GridPos).Count > 0;
                    board.SwapCandies(cellA.GridPos, cellB.GridPos);
                    if (match) return true;
                }
            }

            // Check upper neighbor
            if (row + 1 < board.Rows)
            {
                CandyCell cellB = board.GetCell(col, row + 1);
                if (cellB != null && cellB.IsActive && !cellB.IsEmpty)
                {
                    board.SwapCandies(cellA.GridPos, cellB.GridPos);
                    bool match = FindMatchesForSwap(board, cellA.GridPos, cellB.GridPos).Count > 0;
                    board.SwapCandies(cellA.GridPos, cellB.GridPos);
                    if (match) return true;
                }
            }
        }
        return false;
    }

    private bool SameType(CandyCell cell, CandyType type)
        => cell != null && cell.IsActive && !cell.IsEmpty &&
           (cell.Candy.CandyType == type ||
            (cell.Candy.IsSpecial && cell.Candy.SpecialBehaviour.MatchesAnyColor));
}
