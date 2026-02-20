using UnityEngine;

public class ChaseZone : MonoBehaviour
{
    private const string PlayerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(PlayerTag)) return;

        if (other.TryGetComponent(out PlayerController player))
            player.SetChase(true);

        if (MusicManager.Instance != null)
            MusicManager.Instance.SetChaseLayer(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(PlayerTag)) return;

        if (other.TryGetComponent(out PlayerController player))
            player.SetChase(false);

        if (MusicManager.Instance != null)
            MusicManager.Instance.SetChaseLayer(false);
    }
}
