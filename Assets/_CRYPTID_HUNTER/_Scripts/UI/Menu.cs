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


	#region Public Methods
	public void Win(PhotoScore _photoScore)
	{
		winScoreDisplay.text = $"Score: {_photoScore.Score}";
		winPhotoDisplay.texture = _photoScore.Photo.Texture;
		winScreen.SetActive(true);
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

    public void SurveyButton()
    {
        Application.OpenURL("https://forms.gle/TNL6Gw9UdkwExpGt6");
        Debug.Log("Menu.cs: Open Link");
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
	}
	#endregion Private Methods
}
