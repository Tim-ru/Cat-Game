using UnityEngine;

[CreateAssetMenu(menuName = "Data/GameSettings", fileName = "GameSettings")]
public class GameSettings : ScriptableObject
{
    [SerializeField] private FloatPersistentProperty _master;
    [SerializeField] private FloatPersistentProperty _music;
    [SerializeField] private FloatPersistentProperty _sfx;

    private static GameSettings _instance;
    public static GameSettings I => _instance == null ? LoadGameSettings() : _instance;

    public FloatPersistentProperty Master => _master;
    public FloatPersistentProperty Music => _music;
    public FloatPersistentProperty Sfx => _sfx;

    public IntPersistentProperty _screenShake;

    private static GameSettings LoadGameSettings()
    {
        return _instance = Resources.Load<GameSettings>("GameSettings");
    }

    private void OnEnable()
    {
        _master = new FloatPersistentProperty(1, GameSetting.MasterVolume.ToString());
        _music = new FloatPersistentProperty(1, GameSetting.Music.ToString());
        _sfx = new FloatPersistentProperty(1, GameSetting.Sfx.ToString());
        _screenShake = new IntPersistentProperty(1, GameSetting.ScreenShake.ToString());
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _master.Validate();
        _music.Validate();
        _sfx.Validate();
        _screenShake.Validate();
    }
#endif
}
public enum GameSetting
{
    MasterVolume,
    Music,
    Sfx,
    ScreenShake
}
