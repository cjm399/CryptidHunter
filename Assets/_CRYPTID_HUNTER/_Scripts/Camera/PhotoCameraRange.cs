using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

public class PhotoCameraRange : MonoBehaviour
{
	#region Variables
	[Header("Camera")]

	[Required]
	[SerializeField, Tooltip("The camera for taking pictures")]
	PhotoCamera photoCam;

	[Header("Range Settings")]

	[MinValue(0f)]
	[SerializeField, Tooltip("The range of the camera")]
	float maxRange = 10f;

	[ReadOnly]
	[SerializeField, Tooltip("Whether the player is in range of their target")]
	bool inRange = false;

	[ReadOnly]
	[SerializeField, Tooltip("Whether the player is aiming directly at their target")]
	bool centered = false;

	[ReadOnly]
	[SerializeField, Tooltip("Whether the target is obstructed")]
	bool obstructed = false;

	[SerializeField, Tooltip("The Layers to check for a collision with a target")]
	LayerMask targetLayers = LayerMask.GetMask();

	[Header("Reticle Settings")]

	[Required, AssetsOnly]
	[SerializeField, Tooltip("The reticle to use when in range of a target")]
	Texture inRangeReticle;

	[Required, AssetsOnly]
	[SerializeField, Tooltip("The reticle to use when out of range of a target")]
	Texture outRangeReticle;

	[Required]
	[SerializeField, Tooltip("The UI Raw Image to display the reticle on")]
	RawImage reticleDisplay;
	#endregion Variables

	#region Properties
	/// <summary>
	/// The range of the camera
	/// </summary>
	public float MaxRange
	{
		get { return maxRange; }
	}

	/// <summary>
	/// Whether the player is in range of their target
	/// </summary>
	public bool InRange
	{
		get { return inRange; }
		private set
		{
			if (inRange != value)
			{
				inRange = value;

				OnInRangeChange?.Invoke(inRange);
			}
		}
	}

	/// <summary>
	/// Whether the player is aiming directly at their target
	/// </summary>
	public bool Centered
	{
		get { return centered; }
		private set
		{
			if (centered != value)
			{
				centered = value;
			}
		}
	}

	/// <summary>
	/// Whether the target is obstructed
	/// </summary>
	public bool Obstructed
	{
		get { return obstructed; }
		set
		{
			if(obstructed != value)
			{
				obstructed = value;
			}
		}
	}
	#endregion Properties

	#region Events
	/// <summary>
	/// Handler for event invoked when in-range status changes
	/// </summary>
	/// <param name="_inRange">Whether now in range</param>
	public delegate void InRangeChangeEventHandler(bool _inRange);
	/// <summary>
	/// Event invoked when the player becomes in range or out of range of their target
	/// </summary>
	public event InRangeChangeEventHandler OnInRangeChange;
	#endregion Events

	#region MonoBehaviour
	private void Update()
	{
		Camera cam = photoCam.Camera;

		Ray ray = cam.ScreenPointToRay(new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2, 0));

		RaycastHit result;

		Physics.Raycast(ray, out result, maxRange * 4, targetLayers, QueryTriggerInteraction.Ignore);

		Centered = result.collider?.GetComponent<PhotoTarget>() != null;

		if(centered)
		{
			RaycastHit obstacleCheck;

			float distance = Vector3.Distance(photoCam.transform.position, result.collider.gameObject.transform.position);
			InRange = distance <= maxRange;

			// Now check if there is an obstacle blocking the view
			obstructed = Physics.Raycast(ray, out obstacleCheck, distance, ~0 - targetLayers, QueryTriggerInteraction.Ignore);
		}
		else
		{
			InRange = false;
		}

		if (centered && inRange && !obstructed)
		{
			reticleDisplay.texture = inRangeReticle;
		}
		else
		{
			reticleDisplay.texture = outRangeReticle;
		}
	}
	#endregion MonoBehaviour
}