using System.Collections.Generic;
using UnityEngine;

public abstract class CandySpecialBehaviour : ScriptableObject
{
    [Range(0f, 1f)] public float spawnChance = 0.05f;
    public virtual bool IsColorIndependent => false;
    public virtual bool ActivatesOnSwap    => false;

    public abstract List<Vector2Int> GetAffectedPositions(Vector2Int origin, int boardWidth, int boardHeight);

    protected bool IsInBounds(Vector2Int pos, int width, int height)
        => pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
}
