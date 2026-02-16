using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MenuButton : MenuItem
{
    [SerializeField] private UnityEvent _onClickEvent;
    public override void OnClick()
    {
        _onClickEvent?.Invoke();
    }

    public override void OnFocused()
    {
        Text.color = Color.white;
    }

    public override void OnUnfocused()
    {
        Text.color = UIColors.gray;
    }
}
