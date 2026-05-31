using UnityEngine;

[CreateAssetMenu(fileName = "BoardConfig", menuName = "CandyCrush/BoardConfig")]
public class BoardConfig : ScriptableObject
{
    [TextArea(5, 20)]
    [Tooltip("'#' = celda activa, '.' = hueco. Primera linea = fila superior visual.")]
    public string shapePattern =
        "########\n########\n########\n########\n########\n########\n########\n########";

    public float       cellSize     = 0.9f;
    public float       cellSpacing  = 0.05f;
    public float       fallDuration = 0.13f;
    public float       swapDuration = 0.15f;
    public CandyData[] candyDataSet;
    public CandyData[] specialCandyDataSet;
    [Range(0f, 1f)]
    public float       specialCandySpawnChance = 0.02f;

    public CandyData GetCandyData(CandyType type)
    {
        foreach (var d in candyDataSet)
            if (d != null && d.candyType == type) return d;
        return null;
    }

    public CandyData GetSpecialCandyData(CandyType type)
    {
        foreach (var d in specialCandyDataSet)
            if (d != null && d.candyType == type) return d;
        return null;
    }

    public int Columns { get; private set; }
    public int Rows    { get; private set; }

    public bool[,] ParseShape()
    {
        string[] lines = shapePattern.Split('\n');
        Rows    = lines.Length;
        Columns = lines[0].TrimEnd().Length;

        bool[,] result = new bool[Columns, Rows];
        for (int r = 0; r < Rows; r++)
        {
            string line = lines[Rows - 1 - r].TrimEnd();
            for (int c = 0; c < Columns; c++)
                result[c, r] = c < line.Length && line[c] == '#';
        }
        return result;
    }

    public Vector2 ComputeBoardOffset()
    {
        float step        = cellSize + cellSpacing;
        float totalWidth  = Columns * step - cellSpacing;
        float totalHeight = Rows    * step - cellSpacing;
        return new Vector2(-totalWidth / 2f + cellSize / 2f,
                           -totalHeight / 2f + cellSize / 2f);
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(shapePattern)) return;
        string[] lines = shapePattern.Split('\n');
        Rows    = lines.Length;
        Columns = lines.Length > 0 ? lines[0].TrimEnd().Length : 0;
    }
}
