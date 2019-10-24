using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class LevelManager : Singleton<LevelManager>
{
	#region Variables
	[ValidateInput("TimeFormattedString", "You must provide a string of the following format HH:MM (Military Time)")]
    [SerializeField] private string GameOverTime;

	[MinValue(0)]
	[SerializeField, Tooltip("The score required for the player to win")]
	int scoreRequired = 10;

    public PlayerCharacter playerCharacter;
	#endregion Variables

	#region Properties
	/// <summary>
	/// The score required for the player to win
	/// </summary>
	public int ScoreRequired
	{
		get { return scoreRequired; }
	}
	#endregion Properties

	#region Events
	public delegate void GameOverEventHandler();
	public event GameOverEventHandler OnGameOver;
	#endregion Events

	#region MonoBehaviour
	void Start()
    {
        TimeManager.Instance.OnMinutePassed += MinutePassedHandler;
    }

    private void OnDisable()
    {
        TimeManager.Instance.OnMinutePassed -= MinutePassedHandler;
    }

    private void OnDestroy()
    {
        TimeManager.Instance.OnMinutePassed -= MinutePassedHandler;
    }
	#endregion MonoBehaviour

	#region Private Methods
	private void MinutePassedHandler(int hours, int minutes)
    {
        string _text = TextHelper.Instance.FormatTime(hours, minutes, true);

        if(_text == GameOverTime)
        {
            Debug.Log("[LevelManager] Game Over");
			OnGameOver?.Invoke();
        }
    }
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
