using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CodingJar.MultiScene;

public class LoadFirstScene : MonoBehaviour
{
    public void Start()
    {
        SceneManager.LoadScene((int)Level.Menu);
        Destroy(this);
    }
}
