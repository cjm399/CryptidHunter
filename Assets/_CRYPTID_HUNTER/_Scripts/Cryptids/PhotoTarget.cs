using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class PhotoTarget : MonoBehaviour
{
	#region Variables
	[ValidateInput("NoNullPoints", "There are null objects within this list", IncludeChildren = true)]
	[SerializeField, Tooltip("All the positions of different points that the player can receive score for capturing in the photo")]
	List<TargetPoint> targetPoints = new List<TargetPoint>();
	#endregion Variables

	#region Properties
	/// <summary>
	/// All the positions of different points that the player can receive score for capturing in the photo
	/// </summary>
	public List<TargetPoint> TargetPoints
	{
		get { return targetPoints; }
	}

	/// <summary>
	/// The maximum score that can be obtained for taking a photo including all target points
	/// </summary>
	public int MaxScore
	{
		get
		{
			int total = 0;

			foreach(TargetPoint point in targetPoints)
			{
				total += point.Score;
			}

			return total;
		}
	}
	#endregion Properties

	#region Odin Validation
	/// <summary>
	/// Check that there are no null references within the list
	/// </summary>
	/// <param name="_points">The list of all TargetPoints to check</param>
	/// <returns>True if no nulls are found and false otherwise</returns>
	private bool NoNullPoints(List<TargetPoint> _points)
	{
		foreach(TargetPoint point in _points)
		{
			if(point == null)
			{
				return false;
			}
		}

		return true;
	}
	#endregion Odin Validation
}