using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private Camera       gameCamera;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private float        dragThreshold = 0.35f;

    public bool IsEnabled { get; set; }
    public event System.Action<Vector2Int, Vector2Int> OnSwapRequested;

    private bool       _isDragging;
    private Vector3    _dragStartWorld;
    private Vector2Int _selectedGridPos;
    private bool       _fired;

    private void Update()
    {
        if (!IsEnabled) return;
        if (Input.GetMouseButtonDown(0)) HandleDown();
        if (_isDragging && Input.GetMouseButton(0)) HandleDrag();
        if (Input.GetMouseButtonUp(0)) { _isDragging = false; _fired = false; }
    }

    private void HandleDown()
    {
        Vector3    world   = ScreenToWorld(Input.mousePosition);
        Vector2Int gridPos = boardManager.WorldToGrid(world);
        CandyCell  cell    = boardManager.GetCell(gridPos);
        if (cell == null || !cell.IsActive || cell.IsEmpty) return;

        _selectedGridPos = gridPos;
        _dragStartWorld  = world;
        _isDragging      = true;
        _fired           = false;
    }

    private void HandleDrag()
    {
        if (_fired) return;
        Vector2 delta = (Vector2)(ScreenToWorld(Input.mousePosition) - _dragStartWorld);
        if (delta.magnitude < dragThreshold) return;

        _fired      = true;
        _isDragging = false;

        Vector2Int dir    = SnapToCardinal(delta);
        Vector2Int target = _selectedGridPos + dir;

        CandyCell targetCell = boardManager.GetCell(target);
        if (targetCell == null || !targetCell.IsActive || targetCell.IsEmpty) return;

        OnSwapRequested?.Invoke(_selectedGridPos, target);
    }

    private Vector3 ScreenToWorld(Vector3 screen)
    {
        Vector3 w = gameCamera.ScreenToWorldPoint(screen);
        w.z = 0f;
        return w;
    }

    private Vector2Int SnapToCardinal(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            return delta.x > 0 ? Vector2Int.right : Vector2Int.left;
        return delta.y > 0 ? Vector2Int.up : Vector2Int.down;
    }
}
