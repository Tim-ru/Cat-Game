using UnityEngine;

public class KillComponent : MonoBehaviour
{
    public void Kill(GameObject go, GameObject go2)
    {
        go.GetComponent<PlayerDeathHandler>().Die();
    }
}
