using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : Singleton<SettingsManager>
{
    public bool useMilitaryTime = false;
    public float lookSensitivityX = .5f;
    public float lookSensitivityY = .5f;
    public bool invertXAxis = false;
    public bool invertYAxis = false;
    public float brightness = 1f;
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float effectVolume = 1f;
    public bool fullScreen = true;
    public ushort resolutionWidth = 1920;
    public ushort resolutionHeight = 1080;
    public bool allowVibration = true;


}
