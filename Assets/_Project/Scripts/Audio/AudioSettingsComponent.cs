using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSettingsComponent : MonoBehaviour
{
    [SerializeField] private GameSetting _mode;
    private AudioSource _source;
    private FloatPersistentProperty _model;
    private FloatPersistentProperty _masterModel;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        _model = FindProperty();
        _model.OnChanged += OnSoundSettingsChanged;
        _masterModel = GameSettings.I.Master;
        _masterModel.OnChanged += OnSoundSettingsChanged;
        OnSoundSettingsChanged(_model.Value, _model.Value);
    }

    private void OnSoundSettingsChanged(float newValue, float oldValue)
    {
        _source.volume = _model.Value * _masterModel.Value;
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
        _masterModel.OnChanged -= OnSoundSettingsChanged;
    }
}
