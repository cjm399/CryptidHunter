using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFirstScene : MonoBehaviour
{
    public void Start()
    {
        if(SceneManager.sceneCount == 1)
        {
            SceneManager.LoadScene((int)Level.Menu);
        }
        Destroy(this);
    }
}
