using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using TMPro;

public class PhotoScoreManager : Singleton<PhotoScoreManager>
{
	#region Variables
	[Required, SceneObjectsOnly]
	[SerializeField, Tooltip("The cryptid target of this level")]
	PhotoTarget targetCryptid;

	[Header("Scoring")]

	[Range(0f, 1f)]
	[SerializeField, Tooltip("The percentage penalty applied to the photo score if taken when outside the camera's range")]
	float rangePenalty = .5f;

	[Range(0f, 1f)]
	[SerializeField, Tooltip("The percentage penalty applied to the photo score if the target is not centered")]
	float centeredPenalty = .5f;

	[Range(0f, 1f)]
	[SerializeField, Tooltip("The percentage score the player must get for a photo to pass in the range [0, 1]")]
	float minPercent = .75f;

	[SerializeField, Tooltip("The layers to check for an obstruction")]
	LayerMask obstacleLayers;

	[SerializeField, Tooltip("A list of all photos taken by the player in this session")]
	List<PhotoScore> photos;

	[Header("Debug")]

	[SerializeField, Tooltip("The debug display of the score of the last photo taken")]
	TextMeshProUGUI debugScoreDisplay;
	#endregion Variables

	#region MonoBehaviour
	private void OnEnable()
	{
		PlayerCharacter.Instance.PhotoCamera.OnTakePhoto += ScorePhoto;
	}

	private void OnDisable()
	{
		PlayerCharacter.Instance.PhotoCamera.OnTakePhoto -= ScorePhoto;
	}
	#endregion MonoBehaviour

	#region Protected Methods
	protected override void CustomAwake()
	{
		photos = new List<PhotoScore>();
	}
	#endregion Protected Methods

	#region Private Methods
	/// <summary>
	/// Score a photo when it is taken
	/// </summary>
	/// <param name="_photo">The photo that was just taken</param>
	private void ScorePhoto(Photo _photo)
	{
		PhotoScore photoScore = new PhotoScore(_photo);

		photoScore.MaxScore = targetCryptid.MaxScore;

		// Calculate the total score achieved by the player
		int score = 0;

		Camera cam = PlayerCharacter.Instance.PhotoCamera.Camera;

		foreach(TargetPoint targetPoint in targetCryptid.TargetPoints)
		{
			float distance = Vector3.Distance(targetPoint.transform.position, cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2, cam.nearClipPlane)));

			Vector3 screenPoint = cam.WorldToScreenPoint(targetPoint.transform.position);

			Ray ray = cam.ScreenPointToRay(screenPoint);
			RaycastHit result;
			Physics.Raycast(ray, out result, distance, obstacleLayers, QueryTriggerInteraction.Ignore);

			// If there is a clear line of sight between the target point and the camera, then give the player points for it
			if(result.collider == null)
			{
				score += targetPoint.Score;
			}
		}

		// The player is not aiming directly at the cryptid while in range of it
		if(!PlayerCharacter.Instance.CamRange.InRange)
		{
			bool centered = PlayerCharacter.Instance.CamRange.Centered;

			bool inRange = Vector3.Distance(PlayerCharacter.Instance.PhotoCamera.transform.position, targetCryptid.transform.position) <= PlayerCharacter.Instance.CamRange.MaxRange;

			if(!centered)
			{
				score = Mathf.FloorToInt(score * centeredPenalty);
			}

			if(!inRange)
			{
				score = Mathf.FloorToInt(score * rangePenalty);
			}
		}

		// Save the final score to the PhotoScore
		photoScore.Score = score;

		Debug.Log($"[{GetType().Name}] Last Photo Score: {score}/{photoScore.MaxScore}");

		if(debugScoreDisplay != null)
		{
			debugScoreDisplay.text = $"Last Photo Score: {score}/{photoScore.MaxScore}";
			debugScoreDisplay.enabled = true;
		}

		photos.Add(photoScore);
	}
	#endregion Private Methods
}