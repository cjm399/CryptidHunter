using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using Sirenix.OdinInspector;

public class Menu : MonoBehaviour
{
	#region Variables
	[Required]
	[SerializeField, Tooltip("UI for entire pause menu")]
	GameObject pauseMenu;
	[Required]
	[SerializeField, Tooltip("UI for main pause menu with all the buttons")]
	GameObject pauseMain;
	[Required]
	[SerializeField, Tooltip("UI for settings menu that shows up after clicking settings")]
	GameObject settings;
	[Required]
	[SerializeField, Tooltip("UI for screen that shows up when winning")]
	GameObject winScreen;
	[Required]
	[SerializeField, Tooltip("UI for screen that shows up when losing")]
	GameObject loseScreen;
    [Required]
    [SerializeField, Tooltip("UI for credits screen")]
    GameObject creditsMenu;
    [Required]
    [SerializeField, Tooltip("UI for almanac")]
    GameObject almanac;

    [Required]
	[SerializeField, Tooltip("TMPro Text that displays score when the player wins")]
	TextMeshProUGUI winScoreDisplay;
	[Required]
	[SerializeField, Tooltip("Image to display the best-scoring photo when the player wins")]
	RawImage winPhotoDisplay;

	[Required]
	[SerializeField, Tooltip("TMPro Text that displays score when the player wins")]
	TextMeshProUGUI loseScoreDisplay;
	[Required]
	[SerializeField, Tooltip("Image to display the best-scoring photo when the player wins")]
	RawImage losePhotoDisplay;

	[Required]
	[SerializeField, Tooltip("TMPro Text that displays the lose condition")]
	TextMeshProUGUI loseCondition;
	#endregion Variables

	#region MonoBehaviour
	private void Start()
	{
		PauseManager.Instance.OnPause += ShowPauseMenu;
		PauseManager.Instance.OnResume += HidePauseMenu;
	}

	private void OnDisable()
	{
		PauseManager.Instance.OnPause -= ShowPauseMenu;
		PauseManager.Instance.OnResume -= HidePauseMenu;
	}
	#endregion MonoBehaviour

	#region Public Methods
	public void Win(PhotoScore _photoScore)
	{
		winScoreDisplay.text = $"Score: {_photoScore.Score}";
		winPhotoDisplay.texture = _photoScore.Photo.Texture;
		winScreen.SetActive(true);
		ShowCursor();
	}

	public void LoseTime(PhotoScore _photoScore)
	{
		loseCondition.text = "You ran out of time.";

		Lose(_photoScore);
	}

	public void LosePhotoCount(PhotoScore _photoScore)
	{
		loseCondition.text = "You ran out of film.";

		Lose(_photoScore);
	}

	public void ShowPauseMenu()
	{
		pauseMenu.SetActive(true);
		pauseMain.SetActive(true);
		settings.SetActive(false);
		ShowCursor();
	}

	public void HidePauseMenu()
	{
		pauseMenu.SetActive(false);
		HideCursor();
	}

	public void PlayButton()
	{
		HideCursor();
		SceneManager.LoadScene(2);
        AudioManager.instance?.Stop("Theme");
        //AudioManager.instance?.Play("Woods_Theme");
		AudioManager.instance?.Play("Wind");
        AudioManager.instance?.Play("Owl");
	}

	public void RestartButton()
	{
		HideCursor();
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
		pauseMenu.SetActive(false);
		winScreen.SetActive(false);
		loseScreen.SetActive(false);
		ShowCursor();
		SceneManager.LoadScene(1);
	}

	public void QuitButton()
	{
		Application.Quit();
		Debug.Log("Quit the Game");

#if UNITY_EDITOR
		UnityEditor.EditorApplication.ExitPlaymode();
#endif
	}

    public void CreditsButton()
    {
        creditsMenu.SetActive(true);
    }
	public void SettingsBackButton()
	{
		pauseMain.SetActive(true);
		settings.SetActive(false);
	}

    public void CreditsBackButton()
    {
        creditsMenu.SetActive(false);
    }

    public void SurveyButton()
    {
        Application.OpenURL("https://forms.gle/UQ5aHJNxcFc9VZrB8");
        Debug.Log("Menu.cs: Open Link");
    }

	public void OpenPhotosFolder()
	{
		string filePath = Application.persistentDataPath + $"/Photos/";

		Application.OpenURL("file://" + filePath);
	}

    public void ExitAlmanac()
    {
        almanac.SetActive(false);
    }

    public void OpenAlmanac()
    {
        almanac.SetActive(true);
    }
    #endregion Public Methods

    #region Private Methods
    private void Lose(PhotoScore _photoScore)
	{
		if (_photoScore != null && _photoScore.Score > 0)
		{
			loseScoreDisplay.text = $"Score: {_photoScore.Score}";
			losePhotoDisplay.texture = _photoScore.Photo.Texture;
		}
		else
		{
			loseScoreDisplay.text = "Score: 0";
			losePhotoDisplay.enabled = false;
		}

		loseScreen.SetActive(true);
		ShowCursor();
	}

	private void ShowCursor()
	{
#if !UNITY_EDITOR
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
#endif
	}

	private void HideCursor()
	{
#if !UNITY_EDITOR
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
#endif
	}
	#endregion Private Methods
}