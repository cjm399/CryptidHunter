using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Sirenix.OdinInspector;

using Rewired;

using TMPro;

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

	[MinValue(0)]
	[SerializeField, Tooltip("The maximum amount of photos the player can take (set to 0 to enable infinite photos)")]
	int maxPhotos = 12;

	[ReadOnly]
	[SerializeField, Tooltip("A list of all photos taken")]
	List<Photo> photos = new List<Photo>();

	[Header("Camera Flash")]
	[Required]
	[SerializeField, Tooltip("The Light to enable during the flash")]
	Light flash;

	[MinValue(0f)]
	[SerializeField, Tooltip("The amount of time the light should stay on")]
	float flashTime = 1f;

	[Header("Display")]
	[Required]
	[SerializeField, Tooltip("A display field to show how many photos the player still can take")]
	TextMeshProUGUI photosLeftCountDisplay;

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
					LevelManager.Instance.playerCharacter.PlayerWalk.AddSpeedModifer(this, speedMultiplier);
					LevelManager.Instance.playerCharacter.PlayerWalk.CanSprint = false;
				}
				else
				{
					LevelManager.Instance.playerCharacter.PlayerWalk.RemoveSpeedModifier(this);
					LevelManager.Instance.playerCharacter.PlayerWalk.CanSprint = true;
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

	/// <summary>
	/// The max amount of photos the player may take
	/// </summary>
	public int MaxPhotos
	{
		get { return maxPhotos; }
	}

	/// <summary>
	/// The number of photos the player has taken
	/// </summary>
	public int PhotoCount
	{
		get { return photos.Count; }
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

	/// <summary>
	/// Handler for event called when the last photo taken has been saved to file
	/// </summary>
	public delegate void SavePhotoEventHandler();
	/// <summary>
	/// Event invoked when the last photo taken is saved to file
	/// </summary>
	public event SavePhotoEventHandler OnSavePhoto;

	/// <summary>
	/// Handler for event called when the player has reached maximum number of photos allowed
	/// </summary>
	public delegate void MaxPhotosTakenEventHandler();
	/// <summary>
	/// Event invoked when player has reached maximum number of photos allowed
	/// </summary>
	public event MaxPhotosTakenEventHandler OnMaxPhotosTaken;
	#endregion Events

	#region MonoBehaviour
	private void OnEnable()
	{
		StartCoroutine(InputSubscribe());

		UpdatePhotosLeftDisplayText();

		camera.enabled = canTakePhotos;
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
		if (camera == null || !canTakePhotos || PauseManager.Instance.Paused || (maxPhotos > 0 && PhotoCount >= maxPhotos) || LevelManager.Instance.IsGameOver)
		{
			return null;
		}

		StartFlash();

        AudioManager.instance?.Play("Camera_Shutter");

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

		photos.Add(photo);

		// Now save the photo to the hard drive
		SavePhoto(photo);

		UpdatePhotosLeftDisplayText();

		OnTakePhoto?.Invoke(photo);

		if(PhotoCount >= maxPhotos && maxPhotos > 0)
		{
			Debug.Log($"[PhotoCamera] Max photos taken");
			OnMaxPhotosTaken?.Invoke();
		}

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

	/// <summary>
	/// Save the last photo taken to file
	/// </summary>
	private void SavePhoto(Photo _photo)
	{
		byte[] bytes = _photo.Texture.EncodeToPNG();

		string filePath = Application.persistentDataPath + $"/Photos/";

		string fileName = $"CryptidHunters_Photo_{System.DateTime.Now:dd-MM-yyyy_hh-mm-ss}.png";

		Directory.CreateDirectory(filePath);

		try
		{
			File.WriteAllBytes(filePath + fileName, bytes);
			OnSavePhoto?.Invoke();
			Debug.Log($"[PhotoCamera] Creating photo at {filePath}{fileName}");
		}
		catch (System.Exception e)
		{
			Debug.LogError($"[PhotoCamera] Encountered error trying to save photo. Error Stack Trace:\n{e.StackTrace}");
		}
	}

	/// <summary>
	/// Update display to show how many photos the player can still take
	/// </summary>
	private void UpdatePhotosLeftDisplayText()
	{
		if(photosLeftCountDisplay != null)
		{
			if (maxPhotos > 0)
			{
				photosLeftCountDisplay.text = $"×{maxPhotos - PhotoCount}";
			}
			else
			{
				photosLeftCountDisplay.enabled = false;
			}
		}
	}
	
	/// <summary>
	/// Start the flash effect
	/// </summary>
	private void StartFlash()
	{
		StopCoroutine(FlashRoutine());
		StartCoroutine(FlashRoutine());
	}

	/// <summary>
	/// Show the flash for a set amount of time
	/// </summary>
	IEnumerator FlashRoutine()
	{
		flash.enabled = true;

		yield return new WaitForSeconds(flashTime);

		flash.enabled = false;
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