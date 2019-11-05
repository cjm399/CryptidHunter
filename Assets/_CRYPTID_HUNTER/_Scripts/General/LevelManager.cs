using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class LevelManager : Singleton<LevelManager>
{
    #region Variables
    [SerializeField, Tooltip("Is there a time limit?")]
    private bool hasTimeLimit = true;

    public bool HasTimeLimit
    {
        get;
        private set;
    }

    [EnableIf("hasTimeLimit")]
	[ValidateInput("TimeFormattedString", "You must provide a string of the following format HH:MM (Military Time)")]
    [SerializeField, Tooltip("The time at which the game ends")]
	private string gameOverTime;

	[MinValue(0)]
	[SerializeField, Tooltip("The score required for the player to win")]
	int scoreRequired = 10;

	[ReadOnly]
	[SerializeField, Tooltip("Whether the game is currently ongoing (will be true once an endgame state has been reached")]
	bool isGameOver = false;

	public PlayerCharacter playerCharacter;
	#endregion Variables

	#region Properties
	/// <summary>
	/// The time at which the game ends
	/// </summary>
	public string GameOverTime
	{
		get { return gameOverTime; }
	}
	
	/// <summary>
	/// The score required for the player to win
	/// </summary>
	public int ScoreRequired
	{
		get { return scoreRequired; }
	}

	/// <summary>
	/// Whether the game is currently running (will be true once an endgame state has been reached)
	/// </summary>
	public bool IsGameOver
	{
		get { return isGameOver; }
		set
		{
			if (isGameOver != value)
			{
				isGameOver = value;

				if (isGameOver)
				{
					OnGameOver?.Invoke();
				}
			}
		}
	}
	#endregion Properties

	#region Events
	public delegate void GameOverEventHandler();
	public event GameOverEventHandler OnGameOver;
	#endregion Events

	#region Private Methods
	#endregion Private Methods

	#region ODIN_VALIDATION
	/// <summary>
	/// Check whether a string is formatted to time;
	/// </summary>
	/// <param name="_text">The string to check</param>
	/// <returns>True if the string is formatted to military time</returns>
	private bool TimeFormattedString(string _text)
    {
        if (_text.Length != 5)
            return false;

        string hours = _text.Substring(0, 2);
        int hrs;
        if (!int.TryParse(hours, out hrs))
        {
            return false;
        }

        if (hrs > 23 || hrs < 0)
        {
            return false;
        }

        string minutes = _text.Substring(3, 2);
        int mins;
        if (!int.TryParse(minutes, out mins))
        {
            return false;
        }
        if (mins > 59 || mins < 00)
        {
            return false;
        }

        return true;
    }

#endregion Odin Validation
}
