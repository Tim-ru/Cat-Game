using UnityEngine;
using UnityEngine.UI;

public class MenuToggle : MenuItem
{
    [SerializeField] private Toggle _toggle;
    private ColorBlock cb;
    private void Start()
    {
        cb = _toggle.colors;
    }
    public override void OnClick()
    {
        _toggle.isOn = !_toggle.isOn;
    }

    public override void OnFocused()
    {
        Text.color = Color.white;
        cb.normalColor = Color.white;
        _toggle.colors = cb;
    }

    public override void OnUnfocused()
    {
        Text.color = UIColors.gray;
        cb.normalColor = UIColors.gray;
        _toggle.colors = cb;
    }
}
