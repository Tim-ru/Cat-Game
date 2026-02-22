using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsWidget : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private FloatPersistentProperty _model;

    private void Start()
    {
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void SetModel(FloatPersistentProperty model)
    {
        _model = model;
        model.OnChanged += OnValueChanged;
        OnValueChanged(model.Value, model.Value);
    }

    private void OnSliderValueChanged(float value)
    {
        _model.Value = value;
    }

    private void OnValueChanged(float newValue, float oldValue)
    {
        _slider.value = newValue;
    }

    private void OnDestroy()
    {
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        _model.OnChanged -= OnValueChanged;
    }
}
