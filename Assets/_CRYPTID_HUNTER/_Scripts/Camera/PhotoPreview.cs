using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

using TMPro;

public class PhotoPreview : MonoBehaviour
{
	#region Variables
	[Required]
	[SerializeField, Tooltip("The Camera to show a preview from")]
	PhotoCamera camera;

	[Required]
	[SerializeField, Tooltip("The UI element to show the photo with")]
	RawImage photoDisplay;

	[Header("Zoom Out")]

	[SerializeField, Tooltip("The initial positioning of the preview image")]
	Rect startPosition = new Rect(new Vector2(-640, 0), new Vector2(1280, 720));

	[SerializeField, Tooltip("The zoomed-out positioning of the preview image")]
	Rect endPosition = new Rect(new Vector2(315, 237), new Vector2(240, 135));

	[SerializeField, Tooltip("The amount of time to take in the zoom-out animation")]
	float zoomOutTime = 1f;

	[SerializeField, Tooltip("The interpolation to use in zooming out")]
	AnimationCurve zoomOutCurve = AnimationCurve.Linear(0, 0, 1, 1);

	[MinValue(0f)]
	[SerializeField, Tooltip("The amount of time in seconds to show the preview for")]
	float previewTime = 5f;

	[MinValue(0f)]
	[SerializeField, Tooltip("The amount of time to spend on a fade-out after the preview time has passed")]
	float fadeOutTime = 0.5f;

	/// <summary>
	/// The Coroutine handling the preview timer and fade-out
	/// </summary>
	private Coroutine previewRoutine = null;
	#endregion Variables

	#region MonoBehaviour
	private void OnEnable()
	{
		camera.OnTakePhoto += ShowPreview;
	}

	private void OnDisable()
	{
		camera.OnTakePhoto -= ShowPreview;
	}
	#endregion MonoBehaviour

	#region Private Methods
	/// <summary>
	/// Start showing a preview of the image taken by the PhotoCamera
	/// </summary>
	/// <param name="_photo">The photo that was taken</param>
	private void ShowPreview(Photo _photo)
	{
		if(_photo == null)
		{
			return;
		}

		if(previewRoutine != null)
		{
			StopCoroutine(previewRoutine);
		}

		photoDisplay.enabled = false;
		photoDisplay.texture = _photo.Texture;
		photoDisplay.rectTransform.anchoredPosition = startPosition.position;
		photoDisplay.rectTransform.sizeDelta = startPosition.size;

		photoDisplay.enabled = true;

		previewRoutine = StartCoroutine(ZoomOut());
	}

	/// <summary>
	/// The timer for a zoom-out animation when a photo is taken
	/// </summary>
	private IEnumerator ZoomOut()
	{
		float deltaX = endPosition.x - startPosition.x;
		float deltaY = endPosition.y - startPosition.y;
		float deltaWidth = endPosition.width - startPosition.width;
		float deltaHeight = endPosition.height - startPosition.height;

		float timeElapsed = 0;

		while(timeElapsed < zoomOutTime)
		{
			yield return null;

			timeElapsed += Time.deltaTime;

			photoDisplay.rectTransform.anchoredPosition = startPosition.position + (new Vector2(deltaX, deltaY) * zoomOutCurve.Evaluate(timeElapsed / zoomOutTime));
			photoDisplay.rectTransform.sizeDelta = startPosition.size + (new Vector2(deltaWidth, deltaHeight) * zoomOutCurve.Evaluate(timeElapsed / zoomOutTime));
		}
		
		photoDisplay.rectTransform.anchoredPosition = endPosition.position;
		photoDisplay.rectTransform.sizeDelta = endPosition.size;

		previewRoutine = StartCoroutine(PreviewTimer());
	}

	/// <summary>
	/// The timer showing the preview for a certain amount of time and then fading out
	/// </summary>
	private IEnumerator PreviewTimer()
	{
		yield return new WaitForSeconds(previewTime);

		AnimationCurve interp = AnimationCurve.Linear(0, 1, 1, 0);

		float timeElapsed = 0f;
		Color fadeColor = Color.white;

		while (timeElapsed < fadeOutTime)
		{
			yield return null;

			timeElapsed += Time.deltaTime;

			fadeColor.a = interp.Evaluate(timeElapsed / fadeOutTime);
			photoDisplay.color = fadeColor;
		}

		photoDisplay.enabled = false;
		photoDisplay.color = Color.white;
	}
	#endregion Private Methods

	#region Odin Validation
	/// <summary>
	/// Check that a given string is not null or empty (used with Odin to ensure Rewired input action names are not blank)
	/// </summary>
	/// <param name="_text">The string to check</param>
	/// <returns></returns>
	private bool StringNotEmpty(string _text)
	{
		return !string.IsNullOrEmpty(_text);
	}
	#endregion Odin Validation
}