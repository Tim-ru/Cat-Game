using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FadeTriggerComponent : MonoBehaviour
{
    public enum FadeType { Fade, Unfade }

    [SerializeField] private Animator fadeAnimator;
    [SerializeField] private FadeType fadeType = FadeType.Fade;
    [SerializeField] private string triggerTag = "Player";
    [SerializeField] private bool canvasIfInactive = true;
    [SerializeField] private float unfadeAfterSeconds = 1f;
    [SerializeField] private bool triggerOnlyOnce = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (fadeAnimator == null) return;
        if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag)) return;

        if (canvasIfInactive && !fadeAnimator.gameObject.activeInHierarchy)
            fadeAnimator.gameObject.SetActive(true);

        fadeAnimator.SetTrigger(fadeType == FadeType.Fade ? "Fade" : "Unfade");
        if (fadeType == FadeType.Fade)
            StartCoroutine(UnfadeAfterDelay());

        if (triggerOnlyOnce)
            GetComponent<Collider2D>().enabled = false;
    }

    private IEnumerator UnfadeAfterDelay()
    {
        yield return new WaitForSeconds(unfadeAfterSeconds);
        fadeAnimator.SetTrigger("Unfade");
    }
}
