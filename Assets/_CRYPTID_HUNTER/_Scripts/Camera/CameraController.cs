using System.Collections;

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

	[Required]
	[SerializeField, Tooltip("Group of Canvas images for the overlay of the camera")]
	GameObject cameraOverlay;

	[Required]
	[SerializeField, Tooltip("The camera script for taking photos")]
	PhotoCamera photoCamera;

	#endregion Variables

	#region MonoBehavior
	private void OnEnable()
	{
		cameraOverlay.SetActive(photoCamera.CameraOut);
		photoCamera.OnCameraOutToggle += ToggleCameraOverlay;

		StartCoroutine(InputSubscribe());
	}

	private void OnDisable()
	{
		GameManager.Instance?.RewiredPlayer?.RemoveInputEventDelegate(TryToggleCamera, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, cameraActionName);
		photoCamera.OnCameraOutToggle -= ToggleCameraOverlay;
	}
	#endregion MonoBehavior

	#region Private Methods
	/// <summary>
	/// Wait until the GameManager is instantiated before subscribing to input events
	/// </summary>
	private IEnumerator InputSubscribe()
	{
		while(GameManager.Instance?.RewiredPlayer == null)
		{
			yield return null;
		}

		GameManager.Instance.RewiredPlayer.AddInputEventDelegate(TryToggleCamera, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, cameraActionName);
	}

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

		photoCamera.CameraOut = !photoCamera.CameraOut;
	}

	/// <summary>
	/// Toggle the camera overlay when the camera itself is toggled on or off
	/// </summary>
	/// <param name="_cameraOut">Whether the camera is now out</param>
	private void ToggleCameraOverlay(bool _cameraOut)
	{
		cameraOverlay.SetActive(_cameraOut);
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

