using UnityEngine;

using Sirenix.OdinInspector;

/// <summary>
/// Just an easy way for every script to find the player and their components
/// </summary>
public class PlayerCharacter : Singleton<PlayerCharacter>
{
	#region Variables
	[Required]
	[SerializeField, Tooltip("The script allowing the player to take photos")]
	PhotoCamera photoCamera;

	[Required]
	[SerializeField, Tooltip("The script limiting the player's range")]
	PhotoCameraRange camRange;

	[Required]
	[SerializeField, Tooltip("The script handling player movement")]
	PlayerWalk playerWalk;

	[Required]
	[SerializeField, Tooltip("The script handling player looking around")]
	PlayerLook playerLook;

    public Canvas myCanvas;
	#endregion Variables

	#region Properties
	/// <summary>
	/// The script allowing the player to take photos
	/// </summary>
	public PhotoCamera PhotoCamera
	{
		get { return photoCamera; }
	}

	/// <summary>
	/// The script limiting the player's range
	/// </summary>
	public PhotoCameraRange CamRange
	{
		get { return camRange; }
	}

	/// <summary>
	/// The script handling player movement
	/// </summary>
	public PlayerWalk PlayerWalk
	{
		get { return playerWalk; }
	}

	/// <summary>
	/// The script handling player looking
	/// </summary>
	public PlayerLook PlayerLook
	{
		get { return playerLook; }
	}
	#endregion Properties

	#region Public Methods
	public void LockPlayer(MonoBehaviour _handler)
	{
		playerWalk.AddSpeedModifer(_handler, 0);
		playerLook.AddLookLock(_handler);
	}

	public void UnlockPlayer(MonoBehaviour _handler)
	{
		playerWalk.RemoveSpeedModifier(_handler);
		playerLook.RemoveLookLock(_handler);
	}

	public void EndGame()
	{
		myCanvas.enabled = false;
		playerWalk.AddSpeedModifer(this, 0);
		photoCamera.CanTakePhotos = false;
		playerLook.AddLookLock(this);

		LevelManager.Instance.IsGameOver = true;
	}
	#endregion Public Methods
}