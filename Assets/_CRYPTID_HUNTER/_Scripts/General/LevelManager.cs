using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class LevelManager : MonoBehaviour
{
    [ValidateInput("TimeFormattedString", "You must provide a string of the following format HH:MM (Military Time)")]
    [SerializeField] private string GameOverTime;

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


    private void MinutePassedHandler(int hours, int minutes)
    {
        string _text = TextHelper.Instance.FormatTime(hours, minutes, true);

        if(_text == GameOverTime)
        {
            Debug.Log("Game Over");
        }
    }

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
