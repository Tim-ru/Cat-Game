using UnityEngine;
using UnityEngine.UI;

public class MenuToggle : MenuItem
{
    [SerializeField] private Toggle _toggle;
    public override void OnClick()
    {
        throw new System.NotImplementedException();
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
