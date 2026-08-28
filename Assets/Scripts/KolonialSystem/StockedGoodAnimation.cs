using System.Collections;
using UnityEngine;

public class StockedGoodAnimation
{
    GameObject objToDeform;

    // Animation tuning
    private const float ShrinkFactor = 0.7f;   // 40% smaller than original (1 - 0.4)
    private const float ShrinkDuration = 0.15f;
    private const float GrowDuration = 0.15f;

    private StockedGoodAnimationRunner _runner;

    public StockedGoodAnimation(GameObject gameObj)
    {
        objToDeform = gameObj;
    }

    // Call this to kick off the deform animation
    public void Play()
    {
        if (objToDeform == null) return;

        // Get or create the coroutine runner
        if (_runner == null)
        {
            GameObject runnerGO = new GameObject("StockedGoodAnimationRunner");
            _runner = runnerGO.AddComponent<StockedGoodAnimationRunner>();
            _runner.Owner = this;
        }

        _runner.StartCoroutine(DeformSequence());
    }

    private IEnumerator DeformSequence()
    {
        Vector3 originalScale = objToDeform.transform.localScale;
        Vector3 shrunkScale = new Vector3(originalScale.x * ShrinkFactor, originalScale.y * ShrinkFactor, originalScale.z);

        // --- Phase 1: shrink by 40% ---
        yield return LerpScale(originalScale, shrunkScale, ShrinkDuration);

        // --- Phase 2: grow back to original size ---
        yield return LerpScale(shrunkScale, originalScale, GrowDuration);

        // Clean up the runner now that the animation is finished
        if (_runner != null)
        {
            GameObject.Destroy(_runner.gameObject);
            _runner = null;
        }
    }

    private IEnumerator LerpScale(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (objToDeform == null)
                yield break; // object was destroyed mid-animation, bail out

            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            objToDeform.transform.localScale = Vector3.Lerp(from, to, t);

            yield return null;
        }

        // Snap to exact final scale
        if (objToDeform != null)
            objToDeform.transform.localScale = to;
    }
}

// Minimal MonoBehaviour used purely to run the coroutine
public class StockedGoodAnimationRunner : MonoBehaviour
{
    public StockedGoodAnimation Owner;
}