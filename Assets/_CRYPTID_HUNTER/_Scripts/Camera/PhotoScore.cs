using UnityEngine;

using Sirenix.OdinInspector;

[System.Serializable]
public class PhotoScore
{
	#region Variables
	[SerializeField, Tooltip("The photo that was taken")]
	Photo photo;

	[SerializeField, Tooltip("The score this photo received")]
	int score;

	[SerializeField, Tooltip("The maximum score this photo could receive")]
	int maxScore;
	#endregion Variables

	#region Properties
	/// <summary>
	/// The photo that was taken
	/// </summary>
	public Photo Photo
	{
		get { return photo; }
		set { photo = value; }
	}

	/// <summary>
	/// The score this photo received
	/// </summary>
	public int Score
	{
		get { return score; }
		set
		{
			if(value >= 0 && value <= maxScore)
			{
				score = value;
			}
		}
	}

	/// <summary>
	/// The maximum score this photo could receive
	/// </summary>
	public int MaxScore
	{
		get { return maxScore; }
		set
		{
			if(maxScore >= 0)
			{
				maxScore = value;

				// Cap the score to the max score
				if(score > maxScore)
				{
					score = maxScore;
				}
			}
		}
	}
	#endregion Properties

	#region Constructors
	/// <summary>
	/// Constructor for a PhotoScore with a specified photo
	/// </summary>
	/// <param name="_photo">The photo that was taken</param>
	public PhotoScore(Photo _photo)
	{
		Photo = _photo;
		Score = 0;
		MaxScore = 0;
	}

	/// <summary>
	/// Constructor for a PhotoScore with a specified photo, score, and max score
	/// </summary>
	/// <param name="_photo">The photo that was taken</param>
	/// <param name="_score">The score of the photo</param>
	/// <param name="_maxScore">The max score the photo can have</param>
	public PhotoScore(Photo _photo, int _score, int _maxScore)
	{
		Photo = _photo;
		Score = _score;
		MaxScore = _maxScore;
	}
	#endregion Constructors
}