using System.Collections;

using UnityEngine;

using Sirenix.OdinInspector;

public class GameEndHandler : MonoBehaviour
{
	#region Variables
	[SerializeField, Tooltip("The Menu script tracking all types of menus")]
	Menu menu;

	[ReadOnly]
	[SerializeField, Tooltip("The photo taken so far with the greatest score")]
	PhotoScore topPhoto;
	#endregion Variables

	#region MonoBehaviour
	private void OnEnable()
	{
		StartCoroutine(InputSubscribe());
	}

	private void OnDisable()
	{
		PhotoScoreManager.Instance.OnPhotoScored -= ComparePhotoTakenToTop;
		LevelManager.Instance.OnGameOver -= EndGameTimeLimit;
		PlayerCharacter.Instance.PhotoCamera.OnMaxPhotosTaken -= EndGamePhotoLimit;
	}
	#endregion MonoBehaviour

	#region Private Methods
	IEnumerator InputSubscribe()
	{
		while(PhotoScoreManager.Instance == null || LevelManager.Instance == null || PlayerCharacter.Instance?.PhotoCamera == null)
		{
			yield return null;
		}

		PhotoScoreManager.Instance.OnPhotoScored += ComparePhotoTakenToTop;
		LevelManager.Instance.OnGameOver += EndGameTimeLimit;
		PlayerCharacter.Instance.PhotoCamera.OnMaxPhotosTaken += EndGamePhotoLimit;
	}

	/// <summary>
	/// End the game when time has run out
	/// </summary>
	private void EndGameTimeLimit()
	{
		PlayerCharacter.Instance.EndGame();
		menu.LoseTime(topPhoto);
	}

	private void EndGamePhotoLimit()
	{
		Debug.Log($"[GameEndHandler] Max photos taken. Ending game.");
		PlayerCharacter.Instance.EndGame();
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
				PlayerCharacter.Instance.EndGame();
				menu.Win(topPhoto);
			}
		}
	}
	#endregion Private Methods
}