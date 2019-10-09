using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    #region Variables
    
    [SerializeField, Tooltip("Image for the overlay of the camera")]
    RawImage cameraOverlay;

    [SerializeField, Tooltip("Camera Speed")]
    float lookSpeed = 3f;

    Vector2 rotation = new Vector2(0, 0);

    #endregion Variables

    #region MonoBehavior

    /// <summary>
    /// Camera movement with Y axis clamp. Currently does not rotate the player
    /// </summary>
    void FixedUpdate()
    {
        rotation.y += Input.GetAxis("Mouse X");
        rotation.x += -Input.GetAxis("Mouse Y");
        rotation.x = Mathf.Clamp(rotation.x, -30f, 30f);
        transform.eulerAngles = (Vector2)rotation * lookSpeed;

    }

    /// <summary>
    ///  Turns on/off the camera when Q is pressed
    /// </summary>
    void Update()
    {
        PhotoCamera photoCamera = new PhotoCamera();

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

