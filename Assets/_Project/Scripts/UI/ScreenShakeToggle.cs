using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShakeToggle : MonoBehaviour
{
    private Toggle _toggle;

    private IntPersistentProperty _model;

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
    }

    private void Start()
    {
        _toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void SetModel(IntPersistentProperty model)
    {
        _model = model;
        model.OnChanged += OnValueChanged;
        OnValueChanged(model.Value, model.Value);
    }

    private void OnToggleValueChanged(bool value)
    {
        if (value)
        {
            _model.Value = 1;
        }
        else _model.Value = 0;
    }

    private void OnValueChanged(int newValue, int oldValue)
    {
        _toggle.isOn = newValue != 0;
    }

    private void OnDestroy()
    {
        _toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        _model.OnChanged -= OnValueChanged;
    }
}
