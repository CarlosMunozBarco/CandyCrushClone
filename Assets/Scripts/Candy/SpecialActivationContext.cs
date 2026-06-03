using System.Collections.Generic;
using UnityEngine;

public class SpecialActivationContext
{
    public BoardManager        Board;
    public HashSet<Vector2Int> CurrentMatches;  // Positions being removed in this resolution step
    public CandyType?          SwapPartnerType; // Type of the candy swapped with (null if not a swap activation)
}
