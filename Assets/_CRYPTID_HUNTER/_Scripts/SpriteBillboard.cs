using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
	#region MonoBehaviour
	private void LateUpdate()
	{
		transform.LookAt(PlayerCharacter.Instance.transform, Vector3.up);
	}
	#endregion MonoBehaviour
}