using UnityEngine;

public class StartMusicOnLoad : MonoBehaviour
{
    [SerializeField] private AudioClip melody;
    [SerializeField] private AudioClip percussion;

    private void Start()
    {
        if (MusicManager.Instance == null || melody == null) return;
        MusicManager.Instance.Play(melody, percussion);
    }
}
