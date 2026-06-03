using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RainbowBehaviour", menuName = "CandyCrush/Behaviours/Rainbow")]
public class RainbowBehaviour : CandySpecialBehaviour
{
    public override bool IsColorIndependent => true;
    public override bool ActivatesOnSwap    => true;
    public override bool MatchesAnyColor    => true;

    public override List<Vector2Int> GetAffectedPositions(Vector2Int origin, SpecialActivationContext ctx)
    {
        CandyType? target = (ctx.SwapPartnerType.HasValue && ctx.SwapPartnerType != CandyType.All)
            ? ctx.SwapPartnerType
            : FindMostCommonColor(ctx.CurrentMatches, origin, ctx.Board);

        return target.HasValue ? ctx.Board.GetAllPositionsOfColor(target.Value) : new List<Vector2Int>();
    }

    private CandyType? FindMostCommonColor(HashSet<Vector2Int> matches, Vector2Int self, BoardManager board)
    {
        if (matches == null) return null;
        var counts = new Dictionary<CandyType, int>();
        foreach (var pos in matches)
        {
            if (pos == self) continue;
            CandyCell cell = board.GetCell(pos);
            if (cell == null || cell.IsEmpty || cell.Candy.CandyType == CandyType.All) continue;
            CandyType t = cell.Candy.CandyType;
            counts[t] = counts.TryGetValue(t, out int c) ? c + 1 : 1;
        }
        CandyType? best = null;
        int bestCount = 0;
        foreach (var kvp in counts)
            if (kvp.Value > bestCount) { bestCount = kvp.Value; best = kvp.Key; }
        return best;
    }
}
