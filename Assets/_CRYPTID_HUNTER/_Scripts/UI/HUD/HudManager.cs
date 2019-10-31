using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudManager : MonoBehaviour
{
    [SerializeField] GameObject timeText;
    TextMeshProUGUI timeTMP;
    
    // Start is called before the first frame update
    private void Start()
    {
        if (TimeManager.Instance == null)
            Debug.Log("NULL TIME MANAGER INSTANCE");
        TimeManager.Instance.OnMinutePassed += MinutePassedHandler;
        timeTMP = timeText.GetComponent<TextMeshProUGUI>();
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
        timeTMP.text = TextHelper.Instance.FormatTime(hours, minutes, SettingsManager.Instance.useMilitaryTime);
    }


}
