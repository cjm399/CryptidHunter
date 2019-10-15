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
	#endregion Properties
}