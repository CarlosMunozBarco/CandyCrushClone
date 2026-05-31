using UnityEngine;

public class CandyCell
{
    public readonly Vector2Int GridPos;
    public readonly bool       IsActive;
    public readonly Vector3    WorldPos;

    public CandyBehaviour Candy { get; private set; }
    public bool IsEmpty => IsActive && Candy == null;

    public CandyCell(Vector2Int gridPos, bool isActive, Vector3 worldPos)
    {
        GridPos  = gridPos;
        IsActive = isActive;
        WorldPos = worldPos;
    }

    // Asigna un candy a esta celda logicamente. NO mueve el transform.
    public void SetCandy(CandyBehaviour candy)
    {
        Candy = candy;
        if (candy != null)
            candy.GridPos = GridPos;
    }

    public CandyBehaviour RemoveCandy()
    {
        CandyBehaviour c = Candy;
        Candy = null;
        return c;
    }
}
