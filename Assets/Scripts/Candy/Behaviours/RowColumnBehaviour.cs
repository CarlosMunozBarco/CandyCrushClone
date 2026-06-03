using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RowColumnBehaviour", menuName = "CandyCrush/Behaviours/RowColumn")]
public class RowColumnBehaviour : CandySpecialBehaviour
{
    public override List<Vector2Int> GetAffectedPositions(Vector2Int origin, SpecialActivationContext ctx)
    {
        var positions = new List<Vector2Int>();

        for (int col = 0; col < ctx.Board.Columns; col++)
            positions.Add(new Vector2Int(col, origin.y));

        for (int row = 0; row < ctx.Board.Rows; row++)
            positions.Add(new Vector2Int(origin.x, row));

        return positions;
    }
}
