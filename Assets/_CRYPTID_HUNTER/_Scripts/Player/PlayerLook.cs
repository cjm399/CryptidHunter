using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using Rewired;

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

	[MinValue(0f)]
	[SerializeField, Tooltip("The speed when looking horizontally")]
	float lookHorizSpeed = 1f;

	[MinValue(0f)]
	[SerializeField, Tooltip("The speed when looking vertically")]
	float lookVertSpeed = 0.5f;

	[Header("Camera Locking Rotation")]
	[MinValue(-360f), MaxValue(0f)]
	[SerializeField, Tooltip("The minimum rotation allowed for the camera's vertical axis")]
	float minCamHorizRotation = -90f;

	[MinValue(-0f), MaxValue(360f)]
	[SerializeField, Tooltip("The maximum rotation allowed for the camera's vertical axis")]
	float maxCamHorizRotation = 90f;

	[ReadOnly]
	[SerializeField, Tooltip("A list of all scripts blocking the player from looking around")]
	List<MonoBehaviour> lookBlocks = new List<MonoBehaviour>();
	#endregion Variables

	#region Properties
	/// <summary>
	/// Whether the player can look around
	/// </summary>
	public bool CanLook
	{
		get { return lookBlocks.Count == 0; }
	}
	#endregion Properties

	#region MonoBehaviour
	private void OnEnable()
	{
		StartCoroutine("InputSubscribe");
	}

	private void OnDisable()
	{
		GameManager.Instance?.RewiredPlayer?.RemoveInputEventDelegate(TryLook, UpdateLoopType.Update, InputActionEventType.AxisActive, lookVertActionName);
		GameManager.Instance?.RewiredPlayer?.RemoveInputEventDelegate(TryLook, UpdateLoopType.Update, InputActionEventType.AxisActive, lookHorzActionName);
	}
	#endregion MonoBehaviour

	#region Public Methods
	/// <summary>
	/// Rotate the player up/down based on look speed
	/// </summary>
	/// <param name="_lookVertSpeed">The speed to move at</param>
	public void ApplyVertRotation(float _lookVertSpeed)
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
	/// Add a new block to prevent the player from looking around
	/// </summary>
	/// <param name="_block">The MonoBehaviour stopping the player from looking around</param>
	public void AddLookBlock(MonoBehaviour _block)
	{
		if(!lookBlocks.Contains(_block))
		{
			lookBlocks.Add(_block);
		}
	}

	/// <summary>
	/// Remove a block that was preventing the player from looking around
	/// </summary>
	/// <param name="_block">The MonoBehaviour that was stopping the player from looking around</param>
	public void RemoveLookBlock(MonoBehaviour _block)
	{
		if (lookBlocks.Contains(_block))
		{
			lookBlocks.Remove(_block);
		}
	}
	#endregion Public Methods

	#region Private Methods
	/// <summary>
	/// Wait until after GameManager is fully instantiated before doing anything with it
	/// </summary>
	private IEnumerator InputSubscribe()
	{
		while (GameManager.Instance?.RewiredPlayer == null)
		{
			yield return null;
		}

		GameManager.Instance.RewiredPlayer.AddInputEventDelegate(TryLook, UpdateLoopType.Update, InputActionEventType.AxisActive, lookVertActionName);
		GameManager.Instance.RewiredPlayer.AddInputEventDelegate(TryLook, UpdateLoopType.Update, InputActionEventType.AxisActive, lookHorzActionName);
	}

	/// <summary>
	/// Try looking when receiving input
	/// </summary>
	/// <param name="_eventData">The Rewired input event data</param>
	private void TryLook(InputActionEventData _eventData)
	{
		if(PauseManager.Instance.Paused || !CanLook || GameManager.Instance.HasReachedEnd)
		{
			return;
		}

		float vertAxis = GameManager.Instance.RewiredPlayer.GetAxis(lookVertActionName);
		float horzAxis = GameManager.Instance.RewiredPlayer.GetAxis(lookHorzActionName);

		spinObject.transform.Rotate(new Vector3(0, lookHorizSpeed * horzAxis, 0));

		ApplyVertRotation(lookVertSpeed * vertAxis);
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