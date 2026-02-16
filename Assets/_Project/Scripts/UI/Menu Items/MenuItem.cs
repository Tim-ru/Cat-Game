using TMPro;
using UnityEngine;

public abstract class MenuItem : MonoBehaviour
{
    protected TextMeshProUGUI Text;
    private void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
    }
    public abstract void OnClick();
    public abstract void OnFocused();
    public abstract void OnUnfocused();
}
