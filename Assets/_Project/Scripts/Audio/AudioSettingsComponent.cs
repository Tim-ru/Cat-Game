using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSettingsComponent : MonoBehaviour
{
    [SerializeField] private GameSetting _mode;
    private AudioSource _source;
    private FloatPersistentProperty _model;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        _model = FindProperty();
        _model.OnChanged += OnSoundSettingsChanged;
        OnSoundSettingsChanged(_model.Value, _model.Value);
    }

    private void OnSoundSettingsChanged(float newValue, float oldValue)
    {
        _source.volume = newValue;
    }

    private FloatPersistentProperty FindProperty()
    {
        switch (_mode)
        {
            case GameSetting.Music:
                return GameSettings.I.Music;
            case GameSetting.Sfx:
                return GameSettings.I.Sfx;
        }
        throw new ArgumentException("Undefined mode");
    }

    private void OnDestroy()
    {
        _model.OnChanged -= OnSoundSettingsChanged;
    }
}
