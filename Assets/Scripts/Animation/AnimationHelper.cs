using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationHelper
{
    public static IEnumerator LinearPosition(Transform t, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed    += Time.deltaTime;
            t.position  = Vector3.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        t.position = to;
    }

    public static IEnumerator EaseOutPosition(Transform t, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t01   = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - (1f - t01) * (1f - t01);
            t.position  = Vector3.Lerp(from, to, eased);
            yield return null;
        }
        t.position = to;
    }

    public static IEnumerator PlaySpriteAnimation(SpriteRenderer sr, Sprite[] frames, float fps,
                                                    System.Action onComplete = null)
    {
        if (frames == null || frames.Length == 0) { onComplete?.Invoke(); yield break; }
        float interval = 1f / fps;
        foreach (Sprite frame in frames)
        {
            sr.sprite = frame;
            yield return new WaitForSeconds(interval);
        }
        onComplete?.Invoke();
    }

    public static IEnumerator WaitForAll(MonoBehaviour runner, IEnumerable<IEnumerator> coroutines)
    {
        int total = 0, done = 0;
        foreach (IEnumerator cr in coroutines)
        {
            total++;
            runner.StartCoroutine(RunThenCount(cr, () => done++));
        }
        while (done < total) yield return null;
    }

    private static IEnumerator RunThenCount(IEnumerator cr, System.Action onDone)
    {
        yield return cr;
        onDone();
    }
}
