using System;
using UnityEngine;

public class AudioComponent : MonoBehaviour
{
    [SerializeField] private AudioData[] _audio;
    [SerializeField] private AudioSource _audioSource;
    public void SetAudio(string id)
    {
        foreach (var audio in _audio)
        {
            if (audio.id == id)
            {
                _audioSource.PlayOneShot(audio.audio);
            }
        }
    }

    [Serializable]
    private class AudioData
    {
        public string id;
        public AudioClip audio;
    }
}
