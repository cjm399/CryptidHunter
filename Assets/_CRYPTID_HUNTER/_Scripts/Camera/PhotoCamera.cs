using System.Collections;

using UnityEngine;

using Sirenix.OdinInspector;

using Rewired;

public class PhotoCamera : MonoBehaviour
{
	#region Variables
	[Header("Input Settings")]

	[ValidateInput("StringNotEmpty", "You must provide a non-empty string here")]
	[SerializeField, Tooltip("The Rewired action name for taking pictures")]
	string photoActionName = "Take Photo";

	[Header("Camera Settings")]
	
	[SerializeField, Tooltip("Whether the player can take pictures")]
	bool canTakePhotos = true;

	[Required]
	[SerializeField, Tooltip("The Unity Camera used for taking pictures")]
	Camera camera;

	[MinValue(0), MaxValue(1920)]
	[SerializeField, Tooltip("The width of photos taken with this camera")]
	int photoWidth = 1280;

	[MinValue(0), MaxValue(1080)]
	[SerializeField, Tooltip("The height of photos taken with this camera")]
	int photoHeight = 720;

	[Header("Movement")]
	[SerializeField, Tooltip("A multiplier to apply to player speed when the camera is out")]
	float speedMultiplier = .8f;
	#endregion Variables

	#region Properties
	/// <summary>
	/// Whether the player can take pictures
	/// </summary>
	public bool CanTakePhotos
	{
		get { return canTakePhotos; }
		set
		{
			if (canTakePhotos != value)
			{
				canTakePhotos = value;

				camera.enabled = canTakePhotos;

				if(canTakePhotos)
				{
					PlayerCharacter.Instance.PlayerWalk.AddSpeedModifer(this, speedMultiplier);					
				}
				else
				{
					PlayerCharacter.Instance.PlayerWalk.RemoveSpeedModifier(this);
				}
			}
		}
	}

	/// <summary>
	/// The Unity Camera used for taking pictures
	/// </summary>
	public Camera Camera
	{
		get { return camera; }
	}
	#endregion Properties

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

	#region MonoBehaviour
	private void OnEnable()
	{
		StartCoroutine(InputSubscribe());
	}

    private void OnDisable()
	{
		GameManager.Instance?.RewiredPlayer?.RemoveInputEventDelegate(TryTakePhoto, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, photoActionName);
	}
	#endregion MonoBehaviour

	#region Public Methods
	/// <summary>
	/// Take a photo with the camera
	/// </summary>
	/// <returns>A Photo with texture rendered from the camera or null if the camera is invalid</returns>
	[Button("Take Photo", ButtonSizes.Medium)]
	public Photo TakePhoto()
	{
		if (camera == null || !canTakePhotos || PauseManager.Instance.Paused)
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

	#region Private Methods
	/// <summary>
	/// Wait until after GameManager is fully instantiated before doing anything with it
	/// </summary>
	private IEnumerator InputSubscribe()
	{
		while(GameManager.Instance?.RewiredPlayer == null)
		{
			yield return null;
		}

		GameManager.Instance.RewiredPlayer.AddInputEventDelegate(TryTakePhoto, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, photoActionName);
	}

	/// <summary>
	/// Try to take a photo when Rewired input registers that the player is pressing a button to take a picture
	/// </summary>
	/// <param name="_eventData">The Rewired action event data</param>
	private void TryTakePhoto(InputActionEventData _eventData)
	{
		TakePhoto();
	}
	#endregion Private Methods

	#region Odin Validation
	/// <summary>
	/// Check that a given string is not null or empty (used with Odin to ensure Rewired input action names are not blank)
	/// </summary>
	/// <param name="_text">The string to check</param>
	/// <returns></returns>
	private bool StringNotEmpty(string _text)
	{
		return !string.IsNullOrEmpty(_text);
	}
	#endregion Odin Validation
}