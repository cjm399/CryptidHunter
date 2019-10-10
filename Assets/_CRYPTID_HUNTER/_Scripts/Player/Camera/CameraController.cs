using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

public class CameraController : MonoBehaviour
{
    #region Variables
    
    [SerializeField, Tooltip("Image for the overlay of the camera")]
    RawImage cameraOverlay;

	[Required]
	[SerializeField, Tooltip("The camera script for taking photos")]
	PhotoCamera photoCamera;

	#endregion Variables

	#region MonoBehavior
	private void OnEnable()
	{
		cameraOverlay.enabled = photoCamera.CanTakePhotos;
	}

	/// <summary>
	///  Turns on/off the camera when Q is pressed
	/// </summary>
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (photoCamera.CanTakePhotos == true)
            {
                cameraOverlay.enabled = false;
                photoCamera.CanTakePhotos = false;
			}
            else
            {
                cameraOverlay.enabled = true;
                photoCamera.CanTakePhotos = true;
            }
        }
    }
    #endregion MonoBehavior
}

