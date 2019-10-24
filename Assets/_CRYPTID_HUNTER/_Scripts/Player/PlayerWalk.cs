using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using Rewired;

public class PlayerWalk : MonoBehaviour
{
	#region Structs
	[System.Serializable]
	public struct SpeedModifier
	{
		/// <summary>
		/// The MonoBehaviour responsible for this modifier
		/// </summary>
		public MonoBehaviour handler;

		/// <summary>
		/// The change applied to the player's speed
		/// </summary>
		public float multiplier;

		/// <summary>
		/// Constructor for SpeedModifier
		/// </summary>
		/// <param name="_handler">The MonoBehaviour responsible for this modifier</param>
		/// <param name="_multiplier">The multiplier to apply to speed</param>
		public SpeedModifier(MonoBehaviour _handler, float _multiplier = 1)
		{
			handler = _handler;
			multiplier = _multiplier;
		}
	}
	#endregion Structs

	#region Variables
	[Header("Input Settings")]

	[ValidateInput("StringNotEmpty", "You must provide a non-empty string")]
	[SerializeField, Tooltip("The Rewired action name for moving forward and backward")]
	string walkActionName = "Walk";

	[ValidateInput("StringNotEmpty", "You must provide a non-empty string")]
	[SerializeField, Tooltip("The Rewired action name for moving left and right")]
	string strafeActionName = "Strafe";

	[ValidateInput("StringNotEmpty", "You must provide a non-empty string")]
	[SerializeField, Tooltip("The Rewired action name for toggling sprint")]
	string sprintActionName = "Sprint";

	[Required]
	[SerializeField, Tooltip("The CharacterController for the player")]
	CharacterController controller;

	[Header("Walking")]

	[MinValue(0f)]
	[SerializeField, Tooltip("The starting speed when moving before accelerating")]
	float startWalkSpeed = 5f;

	[MinValue(0f)]
	[SerializeField, Tooltip("The max walking speed after full acceleration")]
	float maxWalkSpeed = 10f;

	[MinValue(0f)]
	[SerializeField, Tooltip("The acceleration when starting to walk")]
	float walkAcceleration = 0.1f;

	[Header("Sprinting")]

	[MinValue(0f)]
	[SerializeField, Tooltip("The sprinting speed")]
	float sprintSpeed = 13f;

	[MinValue(0f)]
	[SerializeField, Tooltip("The acceleration when starting to sprint")]
	float sprintAcceleration = 0.2f;

	[MinValue(0f)]
	[SerializeField, Tooltip("The max energy the player has for sprinting")]
	float maxSprintEnergy = 100f;

	[MinValue(0f)]
	[SerializeField, Tooltip("The amount of energy the player loses each frame when sprinting")]
	float sprintEnergyDepletion = 1f;

	[ReadOnly]
	[SerializeField, Tooltip("The current amount of energy the player has")]
	float currentEnergy;

	[ReadOnly]
	[SerializeField, Tooltip("The current movement speed")]
	float currentSpeed;

	/// <summary>
	/// Whether the player is walking
	/// </summary>
	bool isWalking = false;

	[ReadOnly]
	[SerializeField, Tooltip("Whether the player is sprinting")]
	bool isSprinting = false;

	[ReadOnly]
	[SerializeField, Tooltip("A list of all scripts applying modifiers to the player's speed")]
	List<SpeedModifier> speedModifiers = new List<SpeedModifier>();
	#endregion Variables

	#region Getters & Setters
	/// <summary>
	/// Whether the player is walking
	/// </summary>
	public bool IsWalking
	{
		get { return isWalking; }
		set
		{
			if (isWalking != value)
			{
				isWalking = value;

				if (isWalking)
				{
					OnStartMoving?.Invoke();
				}
				else
				{
					IsSprinting = false;

					OnStopMoving?.Invoke();
				}
			}
		}
	}

	/// <summary>
	/// Whether the player is sprinting
	/// </summary>
	public bool IsSprinting
	{
		get { return isSprinting; }
		set
		{
			if (isSprinting != value)
			{
				isSprinting = value;

				if (isSprinting)
				{
					StartCoroutine("Sprint");
					OnStartSprinting?.Invoke();
				}
				else
				{
					StartCoroutine("SprintRecharge");
					OnStopSprinting?.Invoke();
				}
			}
		}
	}
	#endregion Getters & Setters

	#region Events
	/// <summary>
	/// Handler for event invoked when the player starts moving
	/// </summary>
	public delegate void onStartMoving();
	/// <summary>
	/// Event invoked when the player starts moving
	/// </summary>
	public event onStartMoving OnStartMoving;

	/// <summary>
	/// Handler for event invoked when the player stops moving
	/// </summary>
	public delegate void onStopMoving();
	/// <summary>
	/// Event invoked when the player stops moving
	/// </summary>
	public event onStopMoving OnStopMoving;

	/// <summary>
	/// Handler for event invoked when the player starts sprinting
	/// </summary>
	public delegate void onStartSprinting();
	/// <summary>
	/// Event invoked when the player starts sprinting
	/// </summary>
	public event onStartSprinting OnStartSprinting;

	/// <summary>
	/// Handler for event invoked when the player stops sprinting
	/// </summary>
	public delegate void onStopSprinting();
	/// <summary>
	/// Event invoked when the player stops sprinting
	/// </summary>
	public event onStopSprinting OnStopSprinting;
	#endregion Events

	#region MonoBehaviour
	private void OnEnable()
	{
		StartCoroutine(InputSubscribe());

		currentSpeed = startWalkSpeed;

		currentEnergy = maxSprintEnergy;
	}

	private void OnDisable()
	{
		GameManager.Instance?.RewiredPlayer?.RemoveInputEventDelegate(TryWalk, UpdateLoopType.Update, InputActionEventType.AxisActive, walkActionName);
		GameManager.Instance?.RewiredPlayer?.RemoveInputEventDelegate(TryWalk, UpdateLoopType.Update, InputActionEventType.AxisActive, strafeActionName);
		GameManager.Instance?.RewiredPlayer?.RemoveInputEventDelegate(TrySprint, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, sprintActionName);
		GameManager.Instance?.RewiredPlayer?.RemoveInputEventDelegate(TrySprint, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, sprintActionName);
	}
	#endregion MonoBehaviour

	#region Public Methods
	/// <summary>
	/// Add a new speed modifier to the player, changing how fast they move
	/// </summary>
	/// <param name="_handler">The MonoBehaviour that is adding this modifier</param>
	/// <param name="_multiplier">The multiplier to apply to speed</param>
	public void AddSpeedModifer(MonoBehaviour _handler, float _multiplier)
	{
		foreach(SpeedModifier modifier in speedModifiers)
		{
			if(modifier.handler == _handler)
			{
				return;
			}
		}

		speedModifiers.Add(new SpeedModifier(_handler, _multiplier));
	}

	/// <summary>
	/// Remove a speed modifier from the player
	/// </summary>
	/// <param name="_handler">The MonoBehaviour that added the modifier before</param>
	public void RemoveSpeedModifier(MonoBehaviour _handler)
	{
		// Create a default with null handler to check later
		SpeedModifier targetModifier = new SpeedModifier(null, 1);

		foreach(SpeedModifier modifier in speedModifiers)
		{
			if(modifier.handler == _handler)
			{
				targetModifier = modifier;
			}
		}

		if(targetModifier.handler == _handler)
		{
			speedModifiers.Remove(targetModifier);
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

		GameManager.Instance.RewiredPlayer.AddInputEventDelegate(TryWalk, UpdateLoopType.Update, InputActionEventType.AxisActive, walkActionName);
		GameManager.Instance.RewiredPlayer.AddInputEventDelegate(TryWalk, UpdateLoopType.Update, InputActionEventType.AxisActive, strafeActionName);
		GameManager.Instance.RewiredPlayer.AddInputEventDelegate(TrySprint, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, sprintActionName);
		GameManager.Instance.RewiredPlayer.AddInputEventDelegate(TrySprint, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, sprintActionName);
	}

	/// <summary>
	/// Try walking when receiving Rewired input
	/// </summary>
	/// <param name="_eventData">The Rewired input event data</param>
	private void TryWalk(InputActionEventData _eventData)
	{
		float walkAxis = GameManager.Instance.RewiredPlayer.GetAxis(walkActionName);
		float strafeAxis = GameManager.Instance.RewiredPlayer.GetAxis(strafeActionName);

		// This function is called when either axis is active, so this is a way to ensure that, when both axes are active, the function only runs once per frame
		// Arbitrary way to decide which one is the one that exits without doing anything
		if(_eventData.actionName == walkActionName)
		{
			if (walkAxis <= strafeAxis)
			{
				return;
			}
		}
		else
		{
			if (strafeAxis < walkAxis)
			{
				return;
			}
		}

		if(PauseManager.Instance.Paused || GameManager.Instance.HasReachedEnd)
		{
			walkAxis = 0;
			strafeAxis = 0;
		}

		// Player is not moving
		if (walkAxis == 0 && strafeAxis == 0)
		{
			currentSpeed = startWalkSpeed;
			IsWalking = false;
			IsSprinting = false;

			// Restore sprint completely if the player stops moving
			currentEnergy = maxSprintEnergy;
		}
		else
		{
			Vector3 moveVector = Vector3.zero;
			Vector2 inputAxes = new Vector2(strafeAxis, walkAxis);

			moveVector += transform.forward * inputAxes.y;
			moveVector += transform.right * inputAxes.x;

			if (moveVector.magnitude > 1)
			{
				moveVector.Normalize();
			}

			float multiplier = 1;

			foreach(SpeedModifier modifier in speedModifiers)
			{
				multiplier *= modifier.multiplier;
			}

			moveVector *= currentSpeed * multiplier;

			controller.SimpleMove(moveVector);

			// ACCELERATION
			if (!isSprinting)
			{
				if (currentSpeed < maxWalkSpeed)
				{
					currentSpeed = Mathf.Clamp(currentSpeed + walkAcceleration, startWalkSpeed, maxWalkSpeed);
				}
				else if (currentSpeed > maxWalkSpeed)
				{
					currentSpeed = Mathf.Clamp(currentSpeed - sprintAcceleration, maxWalkSpeed, sprintSpeed);
				}
			}
			else
			{
				if (currentSpeed < sprintSpeed)
				{
					currentSpeed = Mathf.Clamp(currentSpeed + sprintAcceleration, startWalkSpeed, sprintSpeed);
				}
			}

			IsWalking = true;
		}
	}

	/// <summary>
	/// Start or stop sprinting based on player button input
	/// </summary>
	/// <param name="_eventData">The Rewired input event data</param>
	private void TrySprint(InputActionEventData _eventData)
	{
		IsSprinting = isWalking && _eventData.GetButton();
	}

	/// <summary>
	/// Deplete the sprint energy as time passes while player sprints
	/// </summary>
	IEnumerator Sprint()
	{
		while (isSprinting)
		{
			yield return new WaitForFixedUpdate();

			currentEnergy = Mathf.Clamp(currentEnergy - sprintEnergyDepletion, 0, maxSprintEnergy);

			if (currentEnergy == 0)
			{
				IsSprinting = false;
			}
		}
	}

	/// <summary>
	/// Recharge sprint energy as time passes while player isn't sprinting
	/// </summary>
	IEnumerator SprintRecharge()
	{
		while (!isSprinting)
		{
			yield return new WaitForFixedUpdate();

			// Regain energy if the player is not sprinting but is still moving
			currentEnergy = Mathf.Clamp(currentEnergy + sprintEnergyDepletion, 0, maxSprintEnergy);
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