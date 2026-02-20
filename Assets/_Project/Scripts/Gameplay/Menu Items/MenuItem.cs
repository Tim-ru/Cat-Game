using TMPro;
using UnityEngine;

public abstract class MenuItem : MonoBehaviour
{
    protected TextMeshProUGUI Text;
    protected void Awake()
    {
        Text = GetComponent<TextMeshProUGUI>();
    }
    public virtual void OnClick() { }
    public abstract void OnFocused();
    public abstract void OnUnfocused();
    public virtual void ChangeValue(float x) { }
}
