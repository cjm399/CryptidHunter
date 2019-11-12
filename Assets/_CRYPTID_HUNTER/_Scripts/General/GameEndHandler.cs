using System.Collections;

using UnityEngine;

using Sirenix.OdinInspector;

public class GameEndHandler : Singleton<GameEndHandler>
{
	#region Variables

	[ReadOnly]
	[SerializeField, Tooltip("The photo taken so far with the greatest score")]
	PhotoScore topPhoto;

	[Required]
	[SerializeField, Tooltip("The object to enable when the player has taken a photo with the minimum score so that they can then complete the last tasks to win")]
	VictoryHandler victoryHandler;

	#endregion Variables

	#region Properties
	/// <summary>
	/// The photo taken so far with the greatest score
	/// </summary>
	public PhotoScore TopPhoto
	{
		get { return topPhoto; }
	}

	/// <summary>
	/// Whether the player has completed the required objectives to win, meaning the player has a photo with the minimum score
	/// </summary>
	public bool HasCompletedObjective
	{
		get { return topPhoto != null && topPhoto.Score >= LevelManager.Instance.ScoreRequired; }
	}
	#endregion Properties

	#region MonoBehaviour
	private void OnEnable()
	{
		victoryHandler.gameObject.SetActive(false);
		StartCoroutine(InputSubscribe());
	}

	private void OnDisable()
	{
		InputUnsubscribe();
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
		LevelManager.Instance.playerCharacter.PhotoCamera.OnMaxPhotosTaken += EndGamePhotoLimit;
	}

	private void InputUnsubscribe()
	{
		TimeManager.Instance.OnMinutePassed -= MinutePassedHandler;
		PhotoScoreManager.Instance.OnPhotoScored -= ComparePhotoTakenToTop;
		LevelManager.Instance.playerCharacter.PhotoCamera.OnMaxPhotosTaken -= EndGamePhotoLimit;
	}

	/// <summary>
	/// Check whether the game's end time has arrived yet
	/// </summary>
	/// <param name="hours">The current hour</param>
	/// <param name="minutes">The current minute</param>
	private void MinutePassedHandler(int hours, int minutes)
	{
		string _text = TextHelper.Instance.FormatTime(hours, minutes, true);

		if (LevelManager.Instance.HasTimeLimit &&_text == LevelManager.Instance.GameOverTime)
		{
			Debug.Log("[LevelManager] Game Over");
			InputUnsubscribe();
			LevelManager.Instance.playerCharacter.EndGame();
			GameManager.Instance.menu.LoseTime(topPhoto);
		}
	}

	/// <summary>
	/// End the game when time has run out
	/// </summary>
	private void EndGameTimeLimit()
	{
		InputUnsubscribe();
		LevelManager.Instance.playerCharacter.EndGame();
		GameManager.Instance.menu.LoseTime(topPhoto);
	}

	/// <summary>
	/// End the game when player has run out of film
	/// </summary>
	private void EndGamePhotoLimit()
	{
		Debug.Log($"[GameEndHandler] Max photos taken. Ending game.");
		InputUnsubscribe();
		LevelManager.Instance.playerCharacter.EndGame();
		GameManager.Instance.menu.LosePhotoCount(topPhoto);
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
				InputUnsubscribe();
				victoryHandler.gameObject.SetActive(true);
			}
		}
	}
	#endregion Private Methods
}