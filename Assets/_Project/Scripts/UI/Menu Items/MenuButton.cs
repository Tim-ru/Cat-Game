using TMPro;
using UnityEngine;

public class MenuButton : MenuItem
{
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
