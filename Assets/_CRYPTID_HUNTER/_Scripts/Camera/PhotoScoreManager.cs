using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using TMPro;

public class PhotoScoreManager : Singleton<PhotoScoreManager>
{
	#region Structs
	private struct ScoreComponent
	{
		public string name;
		public int score;
		public int count;

		public ScoreComponent(string _name, int _score, int _count)
		{
			name = _name;
			score = _score;
			count = _count;
		}
	}

	private struct ScoreComponents
	{
		public List<ScoreComponent> components;

		/// <summary>
		/// Add a new score component to keep track of
		/// </summary>
		/// <param name="_name"></param>
		/// <param name="_score"></param>
		public void AddComponent(string _name, int _score)
		{
			int index = -1;

			for(int i = 0; i < components.Count; ++i)
			{
				if(components[i].name == _name)
				{
					index = i;
				}
			}

			if (index > -1)
			{
				ScoreComponent component = components[index];

				++component.count;

				components[index] = component;
			}
			else
			{
				components.Add(new ScoreComponent(_name, _score, 1));
			}
		}
	}
	#endregion Structs

	#region Variables
	[Required, SceneObjectsOnly]
	[SerializeField, Tooltip("The cryptid targets of this level")]
	List<PhotoTarget> targetCryptids;

	[Header("Scoring")]

	[Range(0f, 1f)]
	[SerializeField, Tooltip("The percentage penalty applied to the photo score if taken when outside the camera's range")]
	float rangePenalty = .5f;
	
	[MinValue(1f)]
	[SerializeField, Tooltip("The multiplier applied to the photo score if the target is centered")]
	float centeredBonus = 2f;

	[SerializeField, Tooltip("The layers to check for an obstruction")]
	LayerMask obstacleLayers;

	[SerializeField, Tooltip("A list of all photos taken by the player in this session")]
	List<PhotoScore> photos;

	[Header("Debug")]

	[SerializeField, Tooltip("The debug display of the score of the last photo taken")]
	TextMeshProUGUI debugScoreDisplay;

	[MinValue(0f)]
	[SerializeField, Tooltip("The amount of time in seconds to show the preview for")]
	float scoreDisplayTime = 5f;

	[MinValue(0f)]
	[SerializeField, Tooltip("The amount of time to spend on a fade-out after the preview time has passed")]
	float scoreFadeOutTime = 0.5f;
	#endregion Variables

	#region Events
	/// <summary>
	/// Handler for event invoked when a photo is scored
	/// </summary>
	/// <param name="_photoScore">The photo with a score attached</param>
	public delegate void PhotoScoredEventHandler(PhotoScore _photoScore);
	/// <summary>
	/// Event invoked when a photo is given a score
	/// </summary>
	public event PhotoScoredEventHandler OnPhotoScored;
	#endregion Events

	#region MonoBehaviour
	private void Start()
	{
		LevelManager.Instance.playerCharacter.PhotoCamera.OnTakePhoto += ScorePhoto;
	}

	private void OnDisable()
	{
		LevelManager.Instance.playerCharacter.PhotoCamera.OnTakePhoto -= ScorePhoto;
	}
	#endregion MonoBehaviour

	#region Protected Methods
	protected override void CustomAwake()
	{
		photos = new List<PhotoScore>();

		if (debugScoreDisplay != null)
		{
			debugScoreDisplay.enabled = false;
		}
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
		photoScore.MaxScore = 0;

		string feedbackText = "";

		foreach (PhotoTarget targetCryptid in targetCryptids)
		{
			photoScore.MaxScore += targetCryptid.MaxScore;
		}

		photoScore.MaxScore = Mathf.FloorToInt(photoScore.MaxScore * centeredBonus);

		// Calculate the total score achieved by the player
		int score = 0;

		Camera cam = LevelManager.Instance.playerCharacter.PhotoCamera.Camera;

		float closestDistance = -1;

		ScoreComponents scoreComponents = new ScoreComponents();
		scoreComponents.components = new List<ScoreComponent>();
		int targetCount = 0;
		int targetBaseScores = 0;

		foreach (PhotoTarget targetCryptid in targetCryptids)
		{
			Vector3 targetPosition = targetCryptid.Collider.bounds.center;

			// First check if on-screen
			Vector3 viewPoint = cam.WorldToViewportPoint(targetPosition);

			if (viewPoint.x >= 0 && viewPoint.x <= 1 && viewPoint.y >= 0 && viewPoint.y <= 1 && viewPoint.z >= 0)
			{
				// Check if the base score was achieved for the target, which happens if the target's center on screen
				Vector3 screenPoint = cam.WorldToScreenPoint(targetPosition);
				screenPoint.z = cam.nearClipPlane;
				Vector3 screenPointInWorld = cam.ScreenToWorldPoint(screenPoint);

				float distance = Vector3.Distance(targetPosition, screenPointInWorld);

				Ray ray = cam.ScreenPointToRay(screenPoint);

				RaycastHit result;

				Physics.Raycast(ray, out result, distance, obstacleLayers, QueryTriggerInteraction.Ignore);

				if (result.collider?.gameObject == targetCryptid.gameObject)
				{
					Debug.Log($"[{GetType().Name}] Direct photo of target. Awarding base score {targetCryptid.BaseScore}");
					score += targetCryptid.BaseScore;
					++targetCount;

					targetBaseScores += targetCryptid.BaseScore;
				}
			}

			// Now give bonus points for each target point on the cryptid's body that is on-screen
			foreach (TargetPoint targetPoint in targetCryptid.TargetPoints)
			{
				// First check if on-screen
				viewPoint = cam.WorldToViewportPoint(targetPoint.transform.position);

				if(viewPoint.x < 0 || viewPoint.x > 1 || viewPoint.y < 0 || viewPoint.y > 1 || viewPoint.z < 0)
				{
					continue;
				}

				Vector3 screenPoint = cam.WorldToScreenPoint(targetPoint.transform.position);
				screenPoint.z = cam.nearClipPlane;
				Vector3 screenPointInWorld = cam.ScreenToWorldPoint(screenPoint);

				float distance = Vector3.Distance(targetPoint.transform.position, screenPointInWorld);

				Ray ray = cam.ScreenPointToRay(screenPoint);
				Debug.DrawRay(ray.origin, ray.direction * distance, Color.cyan, 15);

				RaycastHit result;

				//Physics.Raycast(ray, out result, distance, ~0, QueryTriggerInteraction.Ignore);
				Physics.Raycast(ray, out result, distance, obstacleLayers, QueryTriggerInteraction.Ignore);

                // If there is a clear line of sight between the target point and the camera, then give the player points for it
                Debug.Log(result.collider?.gameObject?.name);
				if(result.collider == null)
				//if (result.collider?.GetComponent<TargetPoint>() != null)
				{
					score += targetPoint.Score;

					scoreComponents.AddComponent(targetPoint.PointName, targetPoint.Score);
				}
			}

			// The player is not aiming directly at a cryptid while in range of it, so we need to calculate the least distance to a cryptid
			// Then compare that later to the camera's max range to decide whether a penalty needs to be applied
			if(!LevelManager.Instance.playerCharacter.CamRange.InRange)
			{
				float currDistance = Vector3.Distance(LevelManager.Instance.playerCharacter.PhotoCamera.transform.position, targetCryptid.transform.position);

				if(closestDistance < 0 || closestDistance > currDistance)
				{
					closestDistance = currDistance;
				}
			}
		}

		feedbackText += $"{targetCount} Target{((targetCount != 1) ? "s" : "")}! +{targetBaseScores}";


		foreach(ScoreComponent component in scoreComponents.components)
		{
			feedbackText += $"\n{component.count} {component.name}! +{component.score}";
		}

		if (LevelManager.Instance.playerCharacter.CamRange.Centered)
		{
			score = Mathf.FloorToInt(score * centeredBonus);
			feedbackText += $"\nCentered! x{centeredBonus}";
			Debug.Log($"[{GetType().Name}] Photo is centered. Awarding multiplier to {score}");
		}

		// The player is not aiming directly at a cryptid, so check if the player was found to be in range of at least one cryptid
		if (!LevelManager.Instance.playerCharacter.CamRange.InRange)
		{
			bool inRange = closestDistance <= LevelManager.Instance.playerCharacter.CamRange.MaxRange;

			if (!inRange)
			{
				score = Mathf.FloorToInt(score * rangePenalty);
				feedbackText += $"\nOut of Range... x{rangePenalty}";
				Debug.Log($"[{GetType().Name}] Target is not in range. Penalizing to {score}");
			}
		}

		// Save the final score to the PhotoScore
		photoScore.Score = score;

		Debug.Log($"[{GetType().Name}] Score: {score}/{photoScore.MaxScore}");
		feedbackText += $"\nScore: {score}/{photoScore.MaxScore}";

		if(debugScoreDisplay != null)
		{
			debugScoreDisplay.text = feedbackText;
			debugScoreDisplay.enabled = true;

			StopAllCoroutines();

			StartCoroutine(ScoreDisplayFadeOut());
		}

		photos.Add(photoScore);

		OnPhotoScored?.Invoke(photoScore);
	}

	/// <summary>
	/// The timer showing the preview for a certain amount of time and then fading out
	/// </summary>
	/// <returns></returns>
	private IEnumerator ScoreDisplayFadeOut()
	{
		yield return new WaitForSeconds(scoreDisplayTime);

		AnimationCurve interp = AnimationCurve.Linear(0, 1, 1, 0);

		float timeElapsed = 0f;
		Color fadeColor = Color.white;

		while (timeElapsed < scoreFadeOutTime)
		{
			yield return null;

			timeElapsed += Time.deltaTime;

			fadeColor.a = interp.Evaluate(timeElapsed / scoreFadeOutTime);
			debugScoreDisplay.color = fadeColor;
		}

		debugScoreDisplay.enabled = false;
		debugScoreDisplay.color = Color.white;
	}
	#endregion Private Methods
}