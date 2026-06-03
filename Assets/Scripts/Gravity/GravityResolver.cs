using System.Collections.Generic;
using UnityEngine;

public class GravityResolver : MonoBehaviour
{
    public bool HasPendingFalls(BoardManager board)
    {
        for (int row = 1; row < board.Rows; row++)
        for (int col = 0; col < board.Columns; col++)
        {
            CandyCell cell = board.GetCell(col, row);
            if (cell == null || !cell.IsActive || cell.IsEmpty) continue;
            if (GetFallTarget(board, col, row).HasValue) return true;
        }
        return false;
    }

    public void ResolveSingleTick(BoardManager board)
    {
        var moves        = new List<(Vector2Int, Vector2Int)>();
        var destinations = new HashSet<Vector2Int>();

        for (int row = 1; row < board.Rows; row++)
        for (int col = 0; col < board.Columns; col++)
        {
            CandyCell cell = board.GetCell(col, row);
            if (cell == null || !cell.IsActive || cell.IsEmpty) continue;

            Vector2Int? target = GetFallTarget(board, col, row);
            if (target.HasValue && !destinations.Contains(target.Value))
            {
                moves.Add((new Vector2Int(col, row), target.Value));
                destinations.Add(target.Value);
            }
        }

        foreach (var (from, to) in moves)
        {
            CandyBehaviour candy = board.GetCell(from).RemoveCandy();
            board.GetCell(to).SetCandy(candy);
        }
    }

    // Prioridad: recto abajo > diagonal izquierda > diagonal derecha.
    // La diagonal solo se usa si la celda lateral es inactiva (no pertenece al tablero),
    // lo que indica una celda aislada que no puede recibir caramelos en linea recta.
    private Vector2Int? GetFallTarget(BoardManager board, int col, int row)
    {
        CandyCell below = board.GetCell(col, row - 1);
        if (below != null && below.IsActive && below.IsEmpty)
            return new Vector2Int(col, row - 1);

        CandyCell diagL = board.GetCell(col - 1, row - 1);
        CandyCell sideL = board.GetCell(col - 1, row);
        if (diagL != null && diagL.IsActive && diagL.IsEmpty)
        {
            bool sideIsInactive = sideL == null || !sideL.IsActive;
            if (sideIsInactive) return new Vector2Int(col - 1, row - 1);
        }

        CandyCell diagR = board.GetCell(col + 1, row - 1);
        CandyCell sideR = board.GetCell(col + 1, row);
        if (diagR != null && diagR.IsActive && diagR.IsEmpty)
        {
            bool sideIsInactive = sideR == null || !sideR.IsActive;
            if (sideIsInactive) return new Vector2Int(col + 1, row - 1);
        }

        return null;
    }
}
