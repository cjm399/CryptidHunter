using UnityEngine;

using Sirenix.OdinInspector;

[RequireComponent(typeof(Collider))]
public class VictoryHandler : MonoBehaviour
{
	#region MonoBehaviour
	private void OnTriggerEnter(Collider other)
	{
		CheckCollider(other);
	}

	private void OnTriggerStay(Collider other)
	{
		CheckCollider(other);
	}
	#endregion MonoBehaviour

	#region Private Methods
	/// <summary>
	/// Check if a given collision is with the player once the player has completed their objective
	/// </summary>
	/// <param name="_other"></param>
	private void CheckCollider(Collider _other)
	{
		Debug.Log($"[{GetType().Name}] Checking collision with {_other}");
		if(_other.CompareTag("Player") && GameEndHandler.Instance.HasCompletedObjective)
		{
			EndGameWin();
		}
	}

	/// <summary>
	/// Method called when the game has officially been won
	/// </summary>
	private void EndGameWin()
	{
		LevelManager.Instance.playerCharacter.EndGame();
		GameManager.Instance.menu.Win(GameEndHandler.Instance.TopPhoto);
	}
	#endregion Private Methods
}