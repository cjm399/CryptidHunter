using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

using Rewired;

public class CameraController : MonoBehaviour
{
	#region Variables

	[Header("Input Settings")]

	[ValidateInput("StringNotEmpty", "You must provide a non-empty string here")]
	[SerializeField, Tooltip("The Rewired action name for toggling camera")]
	string cameraActionName = "Toggle Camera";

	[Header("Camera Settings")]

	[SerializeField, Tooltip("Image for the overlay of the camera")]
	RawImage cameraOverlay;

	[Required]
	[SerializeField, Tooltip("The camera script for taking photos")]
	PhotoCamera photoCamera;

	#endregion Variables

	#region MonoBehavior
	private void OnEnable()
	{
		cameraOverlay.enabled = photoCamera.CanTakePhotos;

		GameManager.Instance.RewiredPlayer.AddInputEventDelegate(TryToggleCamera, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, cameraActionName);
	}

	private void OnDisable()
	{
		GameManager.Instance.RewiredPlayer.RemoveInputEventDelegate(TryToggleCamera, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, cameraActionName);
	}
	#endregion MonoBehavior

	#region Private Methods
	/// <summary>
	/// Try toggling whether the camera is out based on Rewired input
	/// </summary>
	/// <param name="_eventData">The Rewired input event data</param>
	private void TryToggleCamera(InputActionEventData _eventData)
	{
		if(PauseManager.Instance.Paused)
		{
			return;
		}

		if (photoCamera.CanTakePhotos == true)
		{
			cameraOverlay.enabled = false;
			photoCamera.CanTakePhotos = false;
		}
		else
		{
			cameraOverlay.enabled = true;
			photoCamera.CanTakePhotos = true;
		}
	}
	#endregion Private Methods

	#region Odin Validation
	/// <summary>
	/// Check whether a string is empty for Odin validation
	/// </summary>
	/// <param name="_text">The string to check</param>
	/// <returns>True if the string is not null or blank and false otherwise</returns>
	private bool StringNotEmpty(string _text)
	{
		return !string.IsNullOrEmpty(_text);
	}
	#endregion Odin Validation
}

