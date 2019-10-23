using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    GameObject pauseMain;
    [SerializeField]
    GameObject settings;

    [SerializeField]
    PauseManager pauseManager;


    public void PlayButton()
    {
        SceneManager.LoadScene(2);
    }

    public void RestartButton()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        pauseMenu.SetActive(false);
    }

    public void SettingsButton()
    {
        pauseMain.SetActive(false);
        settings.SetActive(true);
    }

    public void MenuButton()
    {
        SceneManager.LoadScene(1);
        pauseMenu.SetActive(false);
    }

    public void QuitButton()
    {
        Application.Quit();
        Debug.Log("Quit the Game");
    }

    public void BackButton()
    {
        pauseMain.SetActive(true);
        settings.SetActive(false);
    }

    

    
}
