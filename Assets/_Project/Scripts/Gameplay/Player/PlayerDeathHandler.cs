using System.Collections;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Animator fadeAnimator;
    [SerializeField] private float deathDelayBeforeRespawn = 0.5f;

    private PlayerController playerController;
    private Rigidbody2D rb;
    private bool isDead;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void HandleDeathTrigger(GameObject other, GameObject trigger)
    {
        if (other != gameObject) return;
        Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        if (playerController != null) playerController.enabled = false;
        if (fadeAnimator != null) fadeAnimator.SetTrigger("Die");
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathDelayBeforeRespawn);
        transform.position = respawnPoint != null ? respawnPoint.position : transform.position;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        isDead = false;
        if (playerController != null) playerController.enabled = true;
    }
}
