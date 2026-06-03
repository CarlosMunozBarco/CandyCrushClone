using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BombBehaviour", menuName = "CandyCrush/Behaviours/Bomb")]
public class BombBehaviour : CandySpecialBehaviour
{
    public override bool IsColorIndependent => true;
    public override bool ActivatesOnSwap    => true;

    public override List<Vector2Int> GetAffectedPositions(Vector2Int origin, SpecialActivationContext ctx)
    {
        var positions = new List<Vector2Int>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                var neighbour = new Vector2Int(origin.x + dx, origin.y + dy);
                if (IsInBounds(neighbour, ctx))
                    positions.Add(neighbour);
            }
        }

        return positions;
    }
}
