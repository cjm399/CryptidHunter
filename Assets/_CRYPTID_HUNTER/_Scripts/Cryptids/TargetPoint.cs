using UnityEngine;

using Sirenix.OdinInspector;

public class TargetPoint : MonoBehaviour
{
	#region Variables
	[MinValue(0)]
	[SerializeField, Tooltip("The score this point is worth")]
	int score = 0;
	#endregion Variables

	#region Properties
	/// <summary>
	/// The score this point is worth
	/// </summary>
	public int Score
	{
		get { return score; }
	}
	#endregion Properties
}