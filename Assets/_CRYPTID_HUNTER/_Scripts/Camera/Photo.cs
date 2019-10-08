using UnityEngine;

/// <summary>
/// Data structure to store a photo with its texture
/// </summary>
[System.Serializable]
public class Photo
{
	#region Variables
	/// <summary>
	/// The Texture taken from a Camera to store the image to
	/// </summary>
	private Texture2D texture;
	#endregion Variables

	#region Properties
	/// <summary>
	/// The Texture taken from a Camera to store the image to
	/// </summary>
	public Texture2D Texture
	{
		get { return texture; }
		set { texture = value; }
	}
	#endregion Properties

}