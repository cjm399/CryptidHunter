using System.Collections;

using UnityEngine;

using Sirenix.OdinInspector;

public class CameraFlash : MonoBehaviour
{
	#region Variables
	[Required]
	[SerializeField, Tooltip("The Light to enable during the flash")]
	Light flash;

	[MinValue(0f)]
	[SerializeField, Tooltip("The amount of time the light should stay on")]
	float flashTime = 1f;
	#endregion Variables

	#region MonoBehaviour
	private void OnEnable()
	{
		LevelManager.Instance.playerCharacter.PhotoCamera.OnTakePhoto += StartFlash;
	}

	private void OnDisable()
	{
		LevelManager.Instance.playerCharacter.PhotoCamera.OnTakePhoto -= StartFlash;
	}
	#endregion MonoBehaviour

	#region Private Methods
	private void StartFlash(Photo _photo)
	{
		StopAllCoroutines();
		StartCoroutine(FlashRoutine());
	}

	IEnumerator FlashRoutine()
	{
		flash.enabled = true;

		yield return new WaitForSeconds(flashTime);

		flash.enabled = false;
	}
	#endregion Private Methods
}