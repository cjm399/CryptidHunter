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
	#endregion Properties
}