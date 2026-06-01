using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolvingState : IGameState
{
    public void Enter(GameStateMachine m)
    {
        m.ActiveCoroutine = m.StartCoroutine(ResolveCoroutine(m));
    }

    public void Update(GameStateMachine m) { }

    public void Exit(GameStateMachine m)
    {
        if (m.ActiveCoroutine != null) m.StopCoroutine(m.ActiveCoroutine);
    }

    private IEnumerator ResolveCoroutine(GameStateMachine m)
    {
        var specialsToSpawn  = BuildSpecialsToSpawn(m);
        var specialPositions = new HashSet<Vector2Int>();
        foreach (var entry in specialsToSpawn) specialPositions.Add(entry.pos);

        var allToRemove = ExpandForSpecials(m, m.CurrentMatches);
        var toExplode   = new List<CandyBehaviour>();

        foreach (Vector2Int pos in allToRemove)
        {
            if (specialPositions.Contains(pos)) continue;
            CandyCell cell = m.boardManager.GetCell(pos);
            if (cell == null || cell.IsEmpty) continue;
            CandyBehaviour candy = cell.RemoveCandy();
            if (candy != null) toExplode.Add(candy);
        }

        foreach (Vector2Int pos in specialPositions)
        {
            CandyCell cell = m.boardManager.GetCell(pos);
            if (cell == null || cell.IsEmpty) continue;
            CandyBehaviour candy = cell.RemoveCandy();
            if (candy != null) toExplode.Add(candy);
        }

        m.AddScore(toExplode.Count * 100);
        yield return AnimationHelper.WaitForAll(m, BuildExplosions(toExplode));
        foreach (CandyBehaviour candy in toExplode) m.candyPool.Return(candy);

        foreach (var entry in specialsToSpawn)
        {
            CandyData specialData = m.boardManager.Config.GetSpecialCandyData(entry.type);
            if (specialData == null)
            {
                Debug.LogWarning($"[Special] No SO especial para CandyType={entry.type}. Revisa specialCandyDataSet en BoardConfig.");
                continue;
            }
            Vector3 worldPos = m.boardManager.GridToWorld(entry.pos);
            CandyBehaviour sc = m.candyPool.Get(specialData, worldPos);
            m.boardManager.GetCell(entry.pos).SetCandy(sc);
            sc.SetSortingOrder(entry.pos.y);
        }

        m.TransitionTo(m.StateFalling);
    }

    private List<SpawnEntry> BuildSpecialsToSpawn(GameStateMachine m)
    {
        var runs          = FindRuns(m.CurrentMatches, 4);
        var result        = new List<SpawnEntry>();
        var usedPositions = new HashSet<Vector2Int>();

        foreach (var run in runs)
        {
            Vector2Int spawnPos;

            if (m.IsPostSwapCheck)
            {
                if      (run.Contains(m.PendingSwapB)) spawnPos = m.PendingSwapB;
                else if (run.Contains(m.PendingSwapA)) spawnPos = m.PendingSwapA;
                else                                   spawnPos = run[run.Count / 2];
            }
            else
            {
                spawnPos = run[run.Count / 2];
            }

            if (usedPositions.Contains(spawnPos)) continue;
            usedPositions.Add(spawnPos);

            CandyCell cell = m.boardManager.GetCell(spawnPos);
            if (cell == null || cell.IsEmpty) continue;

            result.Add(new SpawnEntry(spawnPos, cell.Candy.CandyType));
        }

        return result;
    }

    private List<List<Vector2Int>> FindRuns(HashSet<Vector2Int> matches, int minLength)
    {
        var runs = new List<List<Vector2Int>>();

        var byRow = new Dictionary<int, List<int>>();
        foreach (Vector2Int pos in matches)
        {
            if (!byRow.ContainsKey(pos.y)) byRow[pos.y] = new List<int>();
            byRow[pos.y].Add(pos.x);
        }
        foreach (var kvp in byRow)
        {
            kvp.Value.Sort();
            ExtractConsecutiveRuns(kvp.Value, kvp.Key, isHorizontal: true,  minLength, runs);
        }

        var byCol = new Dictionary<int, List<int>>();
        foreach (Vector2Int pos in matches)
        {
            if (!byCol.ContainsKey(pos.x)) byCol[pos.x] = new List<int>();
            byCol[pos.x].Add(pos.y);
        }
        foreach (var kvp in byCol)
        {
            kvp.Value.Sort();
            ExtractConsecutiveRuns(kvp.Value, kvp.Key, isHorizontal: false, minLength, runs);
        }

        return runs;
    }

    private void ExtractConsecutiveRuns(List<int> sorted, int fixedAxis,
                                        bool isHorizontal, int minLength,
                                        List<List<Vector2Int>> output)
    {
        var current = new List<Vector2Int>();

        for (int i = 0; i < sorted.Count; i++)
        {
            Vector2Int pos = isHorizontal
                ? new Vector2Int(sorted[i], fixedAxis)
                : new Vector2Int(fixedAxis,  sorted[i]);

            if (current.Count == 0 || sorted[i] == sorted[i - 1] + 1)
            {
                current.Add(pos);
            }
            else
            {
                if (current.Count >= minLength) output.Add(new List<Vector2Int>(current));
                current.Clear();
                current.Add(pos);
            }
        }

        if (current.Count >= minLength) output.Add(current);
    }

    private HashSet<Vector2Int> ExpandForSpecials(GameStateMachine m, HashSet<Vector2Int> matches)
    {
        var expanded = new HashSet<Vector2Int>(matches);
        var queue    = new Queue<Vector2Int>();

        foreach (Vector2Int pos in matches)
        {
            CandyCell cell = m.boardManager.GetCell(pos);
            if (cell != null && !cell.IsEmpty && cell.Candy.IsSpecial)
                queue.Enqueue(pos);
        }

        while (queue.Count > 0)
        {
            Vector2Int pos      = queue.Dequeue();
            CandyCell  origin   = m.boardManager.GetCell(pos);
            if (origin == null || origin.IsEmpty) continue;

            var affected = origin.Candy.SpecialBehaviour.GetAffectedPositions(
                pos, m.boardManager.Columns, m.boardManager.Rows);

            foreach (Vector2Int np in affected)
            {
                if (expanded.Add(np))
                {
                    CandyCell c = m.boardManager.GetCell(np);
                    if (c != null && !c.IsEmpty && c.Candy.IsSpecial)
                        queue.Enqueue(np);
                }
            }
        }

        return expanded;
    }

    private IEnumerable<IEnumerator> BuildExplosions(List<CandyBehaviour> candies)
    {
        foreach (var candy in candies)
            yield return candy.PlayExplode(null);
    }

    private struct SpawnEntry
    {
        public Vector2Int pos;
        public CandyType  type;
        public SpawnEntry(Vector2Int pos, CandyType type) { this.pos = pos; this.type = type; }
    }
}
