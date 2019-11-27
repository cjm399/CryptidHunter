using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using Rewired;



/// <summary>
/// Player controller for looking around
/// </summary>
public class PlayerLook : MonoBehaviour
{
	#region Variables
	[Header("Input Settings")]

	[ValidateInput("StringNotEmpty", "You must provide a non-empty string")]
	[SerializeField, Tooltip("The Rewired action name for looking up and down")]
	string lookVertActionName = "Look Vertical";

	[ValidateInput("StringNotEmpty", "You must provide a non-empty string")]
	[SerializeField, Tooltip("The Rewired action name for looking left and right")]
	string lookHorzActionName = "Look Horizontal";

	[Header("Looking")]

	[Required]
	[SerializeField, Tooltip("The GameObject to spin when looking left and right")]
	GameObject spinObject;

	[Required]
	[SerializeField, Tooltip("The GameObject to rotate when looking up and down")]
	GameObject lookObject;

	[SerializeField, Tooltip("Whether to use unscaled sensitivity or scaled")]
	bool useScaledSpeeds = false;

	[Range(0f, 1f), HideIf("useScaledSpeeds")]
	[SerializeField, Tooltip("The speed when looking horizontally")]
    float lookHorizSpeed = 0.25f;
    
	[Range(0f, 1f), HideIf("useScaledSpeeds")]
	[SerializeField, Tooltip("The speed when looking vertically")]
	float lookVertSpeed = 0.25f;

	[MinValue(0f), ShowIf("useScaledSpeeds")]
	[SerializeField, Tooltip("The speed when looking horizontally, scaled based on DPI and screen size")]
	float lookHorizSpeedScaled = 12f;

	[MinValue(0f), ShowIf("useScaledSpeeds")]
	[SerializeField, Tooltip("The speed when looking vertically, scaled based on DPI and screen size")]
	float lookVertSpeedScaled = 4f;

	// The starting horizontal and vertical speeds for if using sensitivity that is scaled based on DPI and screen size
	float lookHorizStart = -1;
	float lookVertStart = -1;
	// The starting setting for multiplier on a scale of 0-1 so that we can scale relative to the baseline
	float lookHorizStartMult = -1;
	float lookVertStartMult = -1;

	[Header("Camera Locking Rotation")]
	[MinValue(-360f), MaxValue(0f)]
	[SerializeField, Tooltip("The minimum rotation allowed for the camera's vertical axis")]
	float minCamHorizRotation = -90f;

	[MinValue(-0f), MaxValue(360f)]
	[SerializeField, Tooltip("The maximum rotation allowed for the camera's vertical axis")]
	float maxCamHorizRotation = 90f;

	[ReadOnly]
	[SerializeField, Tooltip("A list of all scripts 'blocking' the player from looking around")]
	List<MonoBehaviour> lookLocks = new List<MonoBehaviour>();
	#endregion Variables

	#region MonoBehaviour
	private void OnEnable()
	{
		StartCoroutine(InputSubscribe());
    }

	private void OnDisable()
	{
		GameManager.Instance?.RewiredPlayer?.RemoveInputEventDelegate(TryLook, UpdateLoopType.Update, InputActionEventType.AxisActive, lookVertActionName);
		GameManager.Instance?.RewiredPlayer?.RemoveInputEventDelegate(TryLook, UpdateLoopType.Update, InputActionEventType.AxisActive, lookHorzActionName);
	}
    #endregion MonoBehaviour

    #region Public Methods
    /// <summary>
    /// Add a block, preventing the player from looking around (when called, just pass in the script that is calling this method with the 'this' keyword)
    /// </summary>
    /// <param name="_block">The script blocking the player from looking around</param>
	  public void AddLookLock(MonoBehaviour _lock)
	{
		if(!lookLocks.Contains(_lock))
		{
			lookLocks.Add(_lock);

			UpdateSpeeds();
		}
	}

	/// <summary>
	/// Remove the block, allowing the player to look around again if the list is now empty (when called, just pass in the script that is calling this method with the 'this' keyword)
	/// </summary>
	/// <param name="_block">The block to remove</param>
	public void RemoveLookLock(MonoBehaviour _lock)
	{
		if(lookLocks.Contains(_lock))
		{
			lookLocks.Remove(_lock);
		}
	}
	#endregion Public Methods

	#region Private Methods
	private IEnumerator InputSubscribe()
	{
		while(GameManager.Instance?.RewiredPlayer == null)
		{
			yield return null;
		}

		while(SettingsManager.Instance == null)
		{
			yield return null;
		}

		// Save the starting sensitivity settings for if not using the 0-1 scale
		lookHorizStart = lookHorizSpeedScaled;
		lookVertStart = lookVertSpeedScaled;
		// Also save the starting multiplier so we can scale relative to how the inital setting was
		lookHorizStartMult = SettingsManager.Instance.lookSensitivityX;
		lookVertStartMult = SettingsManager.Instance.lookSensitivityY;

		GameManager.Instance.RewiredPlayer.AddInputEventDelegate(TryLook, UpdateLoopType.Update, InputActionEventType.AxisActive, lookVertActionName);
		GameManager.Instance.RewiredPlayer.AddInputEventDelegate(TryLook, UpdateLoopType.Update, InputActionEventType.AxisActive, lookHorzActionName);
	}

	/// <summary>
	/// Try looking when receiving input
	/// </summary>
	/// <param name="_eventData">The Rewired input event data</param>
	private void TryLook(InputActionEventData _eventData)
	{
		/* Player cannot look around if any of the following are true:
		 *	- The game is paused
		 *	- The input script is unable to move
		 *	- Sky View is enabled
		 *	- There is at least one blocker registered on this script
		 */
		if (PauseManager.Instance.Paused || lookLocks.Count > 0)
		{
			UpdateSpeeds();
			return;
		}

		float vertAxis = GameManager.Instance.RewiredPlayer.GetAxis(lookVertActionName);
		float horzAxis = GameManager.Instance.RewiredPlayer.GetAxis(lookHorzActionName);
		
		if (useScaledSpeeds)
		{
			vertAxis *= Screen.dpi / Screen.height;
			horzAxis *= Screen.dpi / Screen.width;
			vertAxis *= lookVertSpeedScaled;
			horzAxis *= lookHorizSpeedScaled;
		}
		else
		{
			vertAxis *= lookVertSpeed;
			horzAxis *= lookHorizSpeed;
		}

		spinObject.transform.Rotate(new Vector3(0, horzAxis, 0));

		ApplyVertRotation(vertAxis);
	}

	/// <summary>
	/// Rotate the player up/down based on look speed
	/// </summary>
	/// <param name="_lookVertSpeed">The speed to move at</param>
	private void ApplyVertRotation(float _lookVertSpeed)
	{
		Vector3 camRotation = new Vector3(lookObject.transform.localEulerAngles.x, 0, 0);

		camRotation.x %= 360;

		if (camRotation.x > 180)
		{
			camRotation.x -= 360;
		}

		camRotation.x -= _lookVertSpeed;

		// Restrict the player to a certain range
		if (camRotation.x < minCamHorizRotation)
		{
			//Debug.Log($"[PlayerLook] Cam rotation too low at {camRotation.x}. Clamping to {minCamHorizRotation}");
			camRotation.x = minCamHorizRotation;
		}
		else if (camRotation.x > maxCamHorizRotation)
		{
			//Debug.Log($"[PlayerLook] Cam rotation too large at {camRotation.x}. Clamping to {maxCamHorizRotation}");
			camRotation.x = maxCamHorizRotation;
		}
		lookObject.transform.localEulerAngles = camRotation;
	}

    /// <summary>
    /// Debug console command to remove all look locks
    /// </summary>
    /// <param name="command"></param>
    private void RemoveLocks(string command)
    {
        string[] parts = command.Split(' ');
        if (parts[0] == "unlock")
        {
            lookLocks = new List<MonoBehaviour>();
            Debug.Log("Removed all Look Locks from Player");
        }
	}

	/// <summary>
	/// Update the horizontal look speed when changed in the settings
	/// </summary>
	/// <param name="_speed">The new looking speed value</param>
	private void LookHorizontalSpeedHandler(float _speed)
	{
		if (_speed > 0)
		{
			lookHorizSpeed = _speed;
		}
	}


	/// <summary>
	/// Update the vertical look speed when changed in the settings
	/// </summary>
	/// <param name="_speed">The new looking speed value</param>
	private void LookVerticalSpeedHandler(float _speed)
	{
		if (_speed > 0)
		{
			lookVertSpeed = _speed;
		}
	}

	/// <summary>
	/// Update sensitivity
	/// </summary>
	private void UpdateSpeeds()
	{
		if (useScaledSpeeds)
		{
			if (lookHorizStartMult > -1 && lookVertStartMult > -1 && lookHorizStart > -1 && lookVertStart > -1)
			{
				float horizMult = SettingsManager.Instance.lookSensitivityX / lookHorizStartMult;
				float vertMult = SettingsManager.Instance.lookSensitivityY / lookVertStartMult;

				lookHorizSpeedScaled = lookHorizStart * horizMult;
				lookVertSpeedScaled = lookVertStart * vertMult;
			}
		}
		else
		{
			lookHorizSpeed = SettingsManager.Instance.lookSensitivityX;
			lookVertSpeed = SettingsManager.Instance.lookSensitivityY;
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