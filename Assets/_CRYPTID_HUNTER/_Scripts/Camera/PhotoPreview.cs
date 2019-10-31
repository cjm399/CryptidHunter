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

	[Required]
	[SerializeField, Tooltip("The UI element to show the save-message instruction on")]
	TextMeshProUGUI saveTipDisplay;

	[ValidateInput("StringNotEmpty", "You must provide a message here")]
	[SerializeField, Tooltip("The message to display when able to save a picture")]
	string saveTipMessage = "Press 'Q' to save photo.";

	[ValidateInput("StringNotEmpty", "You must provide a message here")]
	[SerializeField, Tooltip("The message to display when a picture has been saved")]
	string saveConfirmMessage = "The photo has been saved.";

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
		camera.OnSavePhoto += ShowSaveMessage;
	}

	private void OnDisable()
	{
		camera.OnTakePhoto -= ShowPreview;
		camera.OnSavePhoto -= ShowSaveMessage;
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

		photoDisplay.enabled = true;
		saveTipDisplay.text = saveTipMessage;
		saveTipDisplay.enabled = true;

		previewRoutine = StartCoroutine(PreviewTimer());
	}

	/// <summary>
	/// The timer showing the preview for a certain amount of time and then fading out
	/// </summary>
	/// <returns></returns>
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
			saveTipDisplay.color = fadeColor;
		}

		photoDisplay.enabled = false;
		saveTipDisplay.enabled = false;
		photoDisplay.color = Color.white;
		saveTipDisplay.color = Color.white;
	}

	/// <summary>
	/// Show the message confirming that the last photo taken has been saved to file
	/// </summary>
	private void ShowSaveMessage()
	{
		saveTipDisplay.text = saveConfirmMessage;
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