using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFirstScene : MonoBehaviour
{
    [SerializeField] private Level levelToLoad;
    public void Start()
    {
        if(SceneManager.sceneCount == 1)
        {
            SceneManager.LoadScene((int)levelToLoad);
        }
        Debug.Log(SceneManager.sceneCount);
        Destroy(this);
    }
}
