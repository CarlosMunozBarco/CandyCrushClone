using System.Collections.Generic;
using UnityEngine;

public abstract class CandySpecialBehaviour : ScriptableObject
{
    [Range(0f, 1f)] public float spawnChance = 0.05f;
    public virtual bool IsColorIndependent => false;
    public virtual bool ActivatesOnSwap    => false;
    public virtual bool MatchesAnyColor    => false;

    public abstract List<Vector2Int> GetAffectedPositions(Vector2Int origin, SpecialActivationContext ctx);

    protected bool IsInBounds(Vector2Int pos, SpecialActivationContext ctx)
        => pos.x >= 0 && pos.x < ctx.Board.Columns && pos.y >= 0 && pos.y < ctx.Board.Rows;
}
