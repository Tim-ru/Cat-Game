using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioSettingsWidget _master;
    [SerializeField] private AudioSettingsWidget _music;
    [SerializeField] private AudioSettingsWidget _sfx;
    [SerializeField] private ScreenShakeToggle _screenShake;
    private void Start()
    {
        _master.SetModel(GameSettings.I.Master);
        _music.SetModel(GameSettings.I.Music);
        _sfx.SetModel(GameSettings.I.Sfx);
        _screenShake.SetModel(GameSettings.I._screenShake);
    }
}
