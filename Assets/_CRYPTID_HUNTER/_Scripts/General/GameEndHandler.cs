using System.Collections;

using UnityEngine;

using Sirenix.OdinInspector;

public class GameEndHandler : MonoBehaviour
{
	#region Variables

	[ReadOnly]
	[SerializeField, Tooltip("The photo taken so far with the greatest score")]
	PhotoScore topPhoto;

    private Menu menu;

    #endregion Variables

    #region MonoBehaviour
    private void OnEnable()
	{
		StartCoroutine(InputSubscribe());
	}

    private void Start()
    {
        menu = GameManager.Instance.menu;
    }

	private void OnDisable()
	{
		TimeManager.Instance.OnMinutePassed -= MinutePassedHandler;
		PhotoScoreManager.Instance.OnPhotoScored -= ComparePhotoTakenToTop;
		LevelManager.Instance.OnGameOver -= EndGameTimeLimit;
		LevelManager.Instance.playerCharacter.PhotoCamera.OnMaxPhotosTaken -= EndGamePhotoLimit;
	}
	#endregion MonoBehaviour

	#region Private Methods
	IEnumerator InputSubscribe()
	{
		while(TimeManager.Instance == null || PhotoScoreManager.Instance == null || LevelManager.Instance == null || LevelManager.Instance.playerCharacter?.PhotoCamera == null)
		{
			yield return null;
		}

		TimeManager.Instance.OnMinutePassed += MinutePassedHandler;
		PhotoScoreManager.Instance.OnPhotoScored += ComparePhotoTakenToTop;
		LevelManager.Instance.OnGameOver += EndGameTimeLimit;
		LevelManager.Instance.playerCharacter.PhotoCamera.OnMaxPhotosTaken += EndGamePhotoLimit;
	}

	/// <summary>
	/// Check whether the game's end time has arrived yet
	/// </summary>
	/// <param name="hours">The current hour</param>
	/// <param name="minutes">The current minute</param>
	private void MinutePassedHandler(int hours, int minutes)
	{
		string _text = TextHelper.Instance.FormatTime(hours, minutes, true);

		if (_text == LevelManager.Instance.GameOverTime)
		{
			Debug.Log("[LevelManager] Game Over");
		}
	}

	/// <summary>
	/// End the game when time has run out
	/// </summary>
	private void EndGameTimeLimit()
	{
		LevelManager.Instance.playerCharacter.EndGame();
		menu.LoseTime(topPhoto);
	}

	private void EndGamePhotoLimit()
	{
		Debug.Log($"[GameEndHandler] Max photos taken. Ending game.");
		LevelManager.Instance.playerCharacter.EndGame();
		menu.LosePhotoCount(topPhoto);
	}

	/// <summary>
	/// Method called when a new photo is scored to compare the new photo to the photo saved with the top score
	/// </summary>
	/// <param name="_photoScore">The newest photo taken</param>
	private void ComparePhotoTakenToTop(PhotoScore _photoScore)
	{
		if(topPhoto == null || topPhoto.Score < _photoScore.Score)
		{
			topPhoto = _photoScore;

			if(topPhoto.Score >= LevelManager.Instance.ScoreRequired)
			{
				LevelManager.Instance.playerCharacter.EndGame();
				menu.Win(topPhoto);
			}
		}
	}
	#endregion Private Methods
}