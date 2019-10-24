using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField, Tooltip ("UI for entire pause menu")]
    GameObject pauseMenu;
    [SerializeField, Tooltip("UI for main pause menu with all the buttons")]
    GameObject pauseMain;
    [SerializeField, Tooltip("UI for settings menu that shows up after clicking settings")]
    GameObject settings;
    [SerializeField, Tooltip("UI for screen that shows up when winning")]
    GameObject winScreen;
    [SerializeField, Tooltip("UI for screen that shows up when losing")]
    GameObject loseScreen;


    [SerializeField, Tooltip("TMPro Text that displays score")]
    TextMeshProUGUI scoreDisplay;
    [SerializeField, Tooltip("TMPro Text that displays the lose condition")]
    TextMeshProUGUI loseCondition;

    [SerializeField, Tooltip("Score that shows up on win and lose screen")]
    PhotoScore photoScore;

    [SerializeField]
    PhotoCamera photoCamera;

    [SerializeField]
    RawImage bestPhoto;

    


    public void Win()
    {
        scoreDisplay.text = $"Score: {photoScore.Score}";
        winScreen.SetActive(true);
    }

    public void Lose()
    {
        scoreDisplay.text = $"Score: {photoScore.Score}";
        loseScreen.SetActive(true);
        if (photoCamera.PhotoCount == photoCamera.MaxPhotos)
        {
            loseCondition.text = "You ran out of film.";
            loseScreen.SetActive(true);
        }
    }

    public void PlayButton()
    {
        SceneManager.LoadScene(2);
    }

    public void RestartButton()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        pauseMenu.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
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
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
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
