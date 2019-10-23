using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFirstScene : MonoBehaviour
{
    public void Start()
    {
        SceneManager.LoadScene((int)Level.TestScene);
        Destroy(this);
    }
}
