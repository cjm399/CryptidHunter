using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class PlanetaryOrbit : MonoBehaviour
{
    [ValidateInput("TimeFormattedString", "You must provide a string of the following format HH:MM (Military Time)")]
    [SerializeField] private string timeAtInitialConfig;

    private int initHours;
    private int initMinutes;
    private int percentage = 0;
    private float degreesPerDay = 360;
    private Transform cachedTransform;


    void Start()
    {
        cachedTransform = transform;
        TimeManager.Instance.OnMinutePassed += OnMinutePassedHandler;
    }

    private void Update()
    {
        float secondsPerDay = TimeManager.Instance.TimePerMinute * 60 * 24;
        float degreesPerSecond = (degreesPerDay / secondsPerDay);
        cachedTransform.RotateAround(Vector3.zero, Vector3.right, Time.deltaTime * degreesPerSecond);
        transform.LookAt(Vector3.zero);
    }


    private void OnMinutePassedHandler(int hours, int minutes)
    {
        if (hours == 0 && minutes == 0)
        {
            percentage = 0;
        }
        else
        {
            percentage = 2359 / (hours * 100 + minutes);
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
