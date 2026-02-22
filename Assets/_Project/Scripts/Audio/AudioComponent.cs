using System;
using UnityEngine;

public class AudioComponent : MonoBehaviour
{
    [SerializeField] private AudioData[] _audio;
    private MusicManager _musicManager;
    private void Awake()
    {
        _musicManager = FindAnyObjectByType<MusicManager>();
    }
    public void PlaySfx(string id)
    {
        foreach (var audio in _audio)
        {
            if (audio.id == id)
            {
                _musicManager.PlaySfx(audio.audio);
            }
        }
    }
    public void Play(string id)
    {
        foreach (var audio in _audio)
        {
            if (audio.id == id)
            {
                _musicManager.Play(audio.audio);
            }
        }
    }
    public void StopCurrentAudio()
    {
        _musicManager.StopCurrentSfx();
    }

    [Serializable]
    private class AudioData
    {
        public string id;
        public AudioClip audio;
    }
}
