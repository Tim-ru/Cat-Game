using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneComponent : MonoBehaviour
{
    public void Load(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
