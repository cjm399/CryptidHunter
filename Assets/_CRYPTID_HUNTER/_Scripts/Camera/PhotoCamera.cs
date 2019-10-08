using UnityEngine;

using Sirenix.OdinInspector;

public class PhotoCamera : MonoBehaviour
{
	#region Variables
	[Required]
	[SerializeField, Tooltip("The Unity Camera used for taking pictures")]
	Camera camera;

	[MinValue(0), MaxValue(1920)]
	[SerializeField, Tooltip("The width of photos taken with this camera")]
	int photoWidth = 1280;

	[MinValue(0), MaxValue(1080)]
	[SerializeField, Tooltip("The height of photos taken with this camera")]
	int photoHeight = 720;
	#endregion Variables

	#region Events
	/// <summary>
	/// Handler for event called when a photo is taken
	/// </summary>
	/// <param name="_photo">The photo that has been taken</param>
	public delegate void TakePhotoEventHandler(Photo _photo);
	/// <summary>
	/// Event invoked when photo is taken
	/// </summary>
	public event TakePhotoEventHandler OnTakePhoto;
	#endregion Events

	#region Public Methods
	/// <summary>
	/// Take a photo with the camera
	/// </summary>
	/// <returns>A Photo with texture rendered from the camera or null if the camera is invalid</returns>
	[Button("Take Photo", ButtonSizes.Medium)]
	public Photo TakePhoto()
	{
		if(camera == null)
		{
			return null;
		}

		Photo photo = new Photo();

		RenderTexture renderTexture = new RenderTexture(photoWidth, photoHeight, 24);

		camera.targetTexture = renderTexture;

		Texture2D texture = new Texture2D(photoWidth, photoHeight, TextureFormat.RGB24, false);

		camera.Render();

		RenderTexture.active = renderTexture;

		texture.ReadPixels(new Rect(0, 0, photoWidth, photoHeight), 0, 0);
		texture.Apply();

		camera.targetTexture = null;
		RenderTexture.active = null;

		photo.Texture = texture;

		OnTakePhoto?.Invoke(photo);

		return photo;
	}
	#endregion Public Methods
}