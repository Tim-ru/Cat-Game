using UnityEngine;

public class DestroyComponent : MonoBehaviour
{
    [SerializeField] private GameObject _go;
    public void Destroy()
    {
        Destroy(_go);
    }
}
