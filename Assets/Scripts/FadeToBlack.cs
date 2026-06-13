using UnityEngine;
using System.Collections;

public class FadeToBlack : MonoBehaviour
{
    public CanvasGroup fadePanel;
    public GameObject finalTextPanel;
    public float fadeDuration = 1.5f;

    public void StartFade()
    {
        StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.alpha = alpha;
            yield return null;
        }

        fadePanel.alpha = 1f;

        yield return new WaitForSecondsRealtime(0.5f);

        if (finalTextPanel != null)
            finalTextPanel.SetActive(true);

        Time.timeScale = 0f;
    }
}