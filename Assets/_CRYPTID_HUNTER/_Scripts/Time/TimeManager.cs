using UnityEngine;

using Sirenix.OdinInspector;

public class TimeManager : Singleton<TimeManager>
{
    #region Attributes

    [Header("Initial Time Settings")]
    [ValidateInput("TimeFormattedString", "You must provide a string of the following format HH:MM (Military Time)")]
    [SerializeField] private string startTime = "18:00";

    /***************************************************/

    [Header("Time Passing Settings")]

    [Range(.001f, 2f)]
    [Tooltip("The amount of time, in seconds, that need to pass in order for a minute to pass in-game")]
    [SerializeField] private float timePerMinute;


    [ReadOnly][SerializeField] private int currHours = 0;
    [ReadOnly] [SerializeField] private int currMinutes = 0;
    private float timeUntilNextMinute = 100f;

    #endregion

    #region PROPERTIES
    public float TimePerMinute
    {
        get
        {
            return timePerMinute;
        }
    }

    #endregion

    #region EVENTS

    /// <summary>
    /// Handler for event called when a minute passes
    /// </summary>
    /// <param name="_photo">The current time as a string</param>
    public delegate void MinutePassedEventHandler(int hours, int minutes);
    /// <summary>
    /// Event invoked when a minute passes
    /// </summary>
    public event MinutePassedEventHandler OnMinutePassed;
    
    /// <summary>
    /// Handler for event called when an hour passes
    /// </summary>
    /// <param name="_photo">The current time as a string</param>
    public delegate void HourPassedEventHandler(int hours, int minutes);
    /// <summary>
    /// Event invoked when an hour passes
    /// </summary>
    public event HourPassedEventHandler OnHourPassed;


    #endregion

    #region MONOBEHAVIOUR

    protected override void CustomAwake()
    {
        timeUntilNextMinute = timePerMinute;
    }

    private void Start()
    {
        OnMinutePassed?.Invoke(currHours, currMinutes);
    }

    private void FixedUpdate()
    {
        if (!PauseManager.Instance.Paused)
        {
            timeUntilNextMinute -= Time.fixedDeltaTime;
            if (timeUntilNextMinute <= 0)
            {
                timeUntilNextMinute = timePerMinute;
                OneMinutePassed();
            }
        }
    }

    #endregion

    #region EVENTHANDLERS

    private void OneMinutePassed()
    {
        if(currMinutes == 59)
        {
            currMinutes = 0;
            if(currHours == 23)
            {
                currHours = 0;
            }
            else
            {
                ++currHours;
            }
        }
        else
        {
            ++currMinutes;
        }

        OnMinutePassed(currHours, currMinutes);
    }

    #endregion
   
    #region Odin Validation

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
        if(!int.TryParse(hours, out hrs))
        {
            return false;
        }

        if(hrs > 23 || hrs < 0)
        {
            return false;
        }

        string minutes = _text.Substring(3, 2);
        int mins;
        if(!int.TryParse(minutes, out mins))
        {
            return false;
        }
        if(mins > 59 || mins < 00)
        {
            return false;
        }

        currMinutes = mins;
        currHours = hrs;

        return true;
    }

    #endregion Odin Validation
}
