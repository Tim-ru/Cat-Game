using System;
using UnityEngine;

[Serializable]
public class IntPersistentProperty : PrefsPersistentProperty<int>
{
    public IntPersistentProperty(int defaultValue, string key) : base(defaultValue, key)
    {
        Init();
    }

    protected override int Read(int defaultValue)
    {
        return PlayerPrefs.GetInt(Key, defaultValue);
    }

    protected override void Write(int value)
    {
        PlayerPrefs.SetInt(Key, value);
        PlayerPrefs.Save();
    }
}
