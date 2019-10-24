using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
	#region MonoBehaviour
	private void LateUpdate()
	{
		transform.LookAt(LevelManager.Instance.playerCharacter.transform, Vector3.up);
	}
	#endregion MonoBehaviour
}