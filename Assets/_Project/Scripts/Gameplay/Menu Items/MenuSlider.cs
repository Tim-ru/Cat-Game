using UnityEngine;
using UnityEngine.UI;

public class MenuSlider : MenuItem
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private float _valueStep;
    private ColorBlock cb;
    private void Start()
    {
        cb = _slider.colors;
    }
    public override void OnFocused()
    {
        Text.color = Color.white;
        _backgroundImage.color = Color.white;
        cb.normalColor = Color.white;
        _slider.colors = cb;
    }

    public override void OnUnfocused()
    {
        Text.color = UIColors.gray;
        _backgroundImage.color = UIColors.gray;
        cb.normalColor = UIColors.gray;
        _slider.colors = cb;
    }
    public override void ChangeValue(float x)
    {
        float newValue = _slider.value + x * _valueStep;
        if (newValue < 0) newValue = 0;
        else if (newValue > 1) newValue = 1;
        _slider.value = newValue;
    }
}
