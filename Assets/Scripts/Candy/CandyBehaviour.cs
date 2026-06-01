using System.Collections;
using UnityEngine;

public class CandyBehaviour : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[]       explosionFrames;
    [SerializeField] private float          explosionFps = 12f;

    public CandyType             CandyType        { get; private set; }
    public bool                  IsSpecial        => SpecialBehaviour != null;
    public CandySpecialBehaviour SpecialBehaviour { get; private set; }
    public Vector2Int            GridPos          { get; set; }
    public bool                  IsMoving         { get; private set; }

    public void Initialize(CandyData data)
    {
        CandyType             = data.candyType;
        SpecialBehaviour      = data.specialBehaviour;
        spriteRenderer.sprite = data.sprite;
        spriteRenderer.color  = data.tintColor;
        gameObject.SetActive(true);

        if (SpecialBehaviour is BombBehaviour)
            Debug.Log($"[Bomb] Caramelo bomba creado en {GridPos}");
    }

    public void ResetForPool()
    {
        StopAllCoroutines();
        spriteRenderer.sprite = null;
        spriteRenderer.color  = Color.white;
        IsMoving              = false;
        gameObject.SetActive(false);
    }

    public IEnumerator MoveTo(Vector3 targetWorld, float duration)
    {
        IsMoving = true;
        yield return AnimationHelper.EaseOutPosition(transform, transform.position, targetWorld, duration);
        transform.position = targetWorld;
        IsMoving           = false;
    }

    public IEnumerator MoveToLinear(Vector3 targetWorld, float duration)
    {
        IsMoving = true;
        yield return AnimationHelper.LinearPosition(transform, transform.position, targetWorld, duration);
        transform.position = targetWorld;
        IsMoving           = false;
    }

    public IEnumerator PlayExplode(System.Action onComplete)
    {
        IsMoving = true;
        yield return AnimationHelper.PlaySpriteAnimation(spriteRenderer, explosionFrames, explosionFps);
        IsMoving = false;
        onComplete?.Invoke();
    }

    public IEnumerator UndoMove(Vector3 targetWorld, float duration)
    {
        yield return MoveTo(targetWorld, duration);
    }

    public void SetSortingOrder(int order)
    {
        spriteRenderer.sortingOrder = order;
    }
}
