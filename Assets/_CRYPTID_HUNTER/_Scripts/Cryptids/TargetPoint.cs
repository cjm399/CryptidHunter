using UnityEngine;

using Sirenix.OdinInspector;

public class TargetPoint : MonoBehaviour
{
	#region Variables
	[ValidateInput("StringNotEmpty", "You must specify a name for this target point, or it won't show up in the score breakdown")]
	[SerializeField, Tooltip("The name of the target point (like 'Head' or 'Left Foot')")]
	string pointName = "";

	[MinValue(0)]
	[SerializeField, Tooltip("The score this point is worth")]
	int score = 0;
	#endregion Variables

	#region Properties
	/// <summary>
	/// The name of the target point (like "Head" or "Left Foot")
	/// </summary>
	public string PointName
	{
		get { return pointName; }
	}

	/// <summary>
	/// The score this point is worth
	/// </summary>
	public int Score
	{
		get { return score; }
	}
	#endregion Properties

	#region Odin Validation
	/// <summary>
	/// Check whether a string is empty for Odin validation
	/// </summary>
	/// <param name="_text">The string to check</param>
	/// <returns>True if the string is not null or blank and false otherwise</returns>
	private bool StringNotEmpty(string _text)
	{
		return !string.IsNullOrEmpty(_text);
	}
	#endregion Odin Validation
}