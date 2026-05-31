using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private BoardConfig boardConfig;
    [SerializeField] private GameObject  cellBackgroundPrefab;
    [SerializeField] private Transform   boardParent;

    private CandyCell[,] _cells;

    public int         Columns { get; private set; }
    public int         Rows    { get; private set; }
    public BoardConfig Config  => boardConfig;

    public CandyCell GetCell(int col, int row)
    {
        if (col < 0 || col >= Columns || row < 0 || row >= Rows) return null;
        return _cells[col, row];
    }
    public CandyCell GetCell(Vector2Int pos) => GetCell(pos.x, pos.y);

    public bool IsActive(int col, int row)
    {
        CandyCell c = GetCell(col, row);
        return c != null && c.IsActive;
    }
    public bool IsActive(Vector2Int pos) => IsActive(pos.x, pos.y);

    public void BuildBoard()
    {
        bool[,] shape = boardConfig.ParseShape();
        Columns = shape.GetLength(0);
        Rows    = shape.GetLength(1);
        _cells  = new CandyCell[Columns, Rows];

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                Vector3 world    = GridToWorld(col, row);
                bool    isActive = shape[col, row];
                _cells[col, row] = new CandyCell(new Vector2Int(col, row), isActive, world);
                if (isActive) SpawnCellBackground(_cells[col, row]);
            }
        }
    }

    public void PopulateInitial(CandyPool pool)
    {
        for (int row = Rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < Columns; col++)
            {
                CandyCell cell = GetCell(col, row);
                if (cell == null || !cell.IsActive) continue;

                var forbidden = new List<CandyType>();

                CandyCell l1 = GetCell(col - 1, row);
                CandyCell l2 = GetCell(col - 2, row);
                if (l1?.Candy != null && l2?.Candy != null &&
                    l1.Candy.CandyType == l2.Candy.CandyType)
                    forbidden.Add(l1.Candy.CandyType);

                CandyCell d1 = GetCell(col, row + 1);
                CandyCell d2 = GetCell(col, row + 2);
                if (d1?.Candy != null && d2?.Candy != null &&
                    d1.Candy.CandyType == d2.Candy.CandyType)
                    forbidden.Add(d1.Candy.CandyType);

                var available = new List<CandyType>();
                foreach (var d in boardConfig.candyDataSet)
                    if (d != null && !forbidden.Contains(d.candyType))
                        available.Add(d.candyType);

                CandyType chosen = available[Random.Range(0, available.Count)];
                CandyData data   = boardConfig.GetCandyData(chosen);

                CandyBehaviour candy = pool.Get(data, GridToWorld(col, row));
                cell.SetCandy(candy);
                candy.SetSortingOrder(row);
            }
        }
    }

    public Vector3 GridToWorld(int col, int row)
    {
        Vector2 offset = boardConfig.ComputeBoardOffset();
        float   step   = boardConfig.cellSize + boardConfig.cellSpacing;
        return new Vector3(offset.x + col * step, offset.y + row * step, 0f);
    }
    public Vector3    GridToWorld(Vector2Int pos) => GridToWorld(pos.x, pos.y);

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector2 offset = boardConfig.ComputeBoardOffset();
        float   step   = boardConfig.cellSize + boardConfig.cellSpacing;
        return new Vector2Int(
            Mathf.RoundToInt((worldPos.x - offset.x) / step),
            Mathf.RoundToInt((worldPos.y - offset.y) / step));
    }

    public void SwapCandies(Vector2Int posA, Vector2Int posB)
    {
        CandyCell cellA = GetCell(posA);
        CandyCell cellB = GetCell(posB);
        if (cellA == null || cellB == null) return;

        CandyBehaviour candyA = cellA.Candy;
        CandyBehaviour candyB = cellB.Candy;
        cellA.SetCandy(candyB);
        cellB.SetCandy(candyA);
    }

    public void RemoveCandyAt(Vector2Int pos, CandyPool pool)
    {
        CandyCell cell = GetCell(pos);
        if (cell == null) return;
        CandyBehaviour c = cell.RemoveCandy();
        if (c != null) pool.Return(c);
    }

    private void SpawnCellBackground(CandyCell cell)
    {
        GameObject bg = Instantiate(cellBackgroundPrefab, cell.WorldPos,
                                    Quaternion.identity, boardParent);
        bg.transform.localScale = Vector3.one * boardConfig.cellSize;
    }
}
