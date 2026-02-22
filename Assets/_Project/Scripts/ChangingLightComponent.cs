using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ChangingLightComponent : MonoBehaviour
{
    private Light2D _light;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
    }
    public void ChangeIntensit(float newIntensity)
    {
        _light.intensity = newIntensity;
    }
    public void ChangeColor(Color newColor)
    {
        _light.color = newColor;
    }
    public void SwapToRed()
    {
        _light.color = new Color(0.8f, 0.407f, 0.407f);
    }
}
