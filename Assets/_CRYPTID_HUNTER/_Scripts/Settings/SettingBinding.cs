using UnityEngine;
using UnityEngine.Events;


public class SettingBindingEvents
{
    public class SettingBindingEvent : UnityEvent<SettingKey> { }
    public static SettingBindingEvent OnSettingChanged = new SettingBindingEvent();
}

[System.Serializable]
public abstract class SettingBinding<T>
{
    public SettingKey Key;
    public T ActualValue { get; set; }
    public string PrefKey
    {
        get
        {
            return SettingConsts.PrefKey(Key);
        }
    }

    public bool IsSet()
    {
        return true;
    }

    public abstract T GetSettingValue();
    public virtual void SetSettingValue(T value)
    {
        SettingBindingEvents.OnSettingChanged.Invoke(Key);
    }
}

[System.Serializable]
public class FloatSettingBinding : SettingBinding<float>
{
    public override float GetSettingValue()
    {
        switch (Key)
        {
            case SettingKey.CAMERA_SENS_X:
                return SettingsManager.Instance.lookSensitivityX;
                break;
            case SettingKey.CAMERA_SENS_Y:
                return SettingsManager.Instance.lookSensitivityY;
                break;
            case SettingKey.VID_BRIGHTNESS:
                return SettingsManager.Instance.brightness;
                break;
            case SettingKey.AUDIO_VOLUME_EFFECT:
                return SettingsManager.Instance.effectVolume;
                break;
            case SettingKey.AUDIO_VOLUME_MASTER:
                return SettingsManager.Instance.masterVolume;
                break;
            case SettingKey.AUDIO_VOLUME_MUSIC:
                return SettingsManager.Instance.musicVolume;
                break;
            default:
                Debug.Log("Key not found: " + Key);
                break;
        }
        return .5f;
    }

    public override void SetSettingValue(float value)
    {
        base.SetSettingValue(value);
        SetValue(value);

    }
    void SetValue(float value)
    {
        SettingsManager SettingsManager = SettingsManager.Instance;
        switch (Key)
        {
            case SettingKey.CAMERA_SENS_X:
                SettingsManager.Instance.lookSensitivityX = value;
                break;
            case SettingKey.CAMERA_SENS_Y:
                SettingsManager.Instance.lookSensitivityY = value;
                break;
            case SettingKey.VID_BRIGHTNESS:
                SettingsManager.Instance.brightness = value;
                break;
            case SettingKey.AUDIO_VOLUME_EFFECT:
                SettingsManager.Instance.effectVolume = value;
                break;
            case SettingKey.AUDIO_VOLUME_MASTER:
                SettingsManager.Instance.masterVolume = value;
                break;
            case SettingKey.AUDIO_VOLUME_MUSIC:
                SettingsManager.Instance.musicVolume = value;
                break;
            default:
                Debug.Log("Key not found: " + Key);
                break;
        }
    }
}

[System.Serializable]
public class StringSettingBinding : SettingBinding<string>
{
    public override string GetSettingValue()
    {
        Debug.Log("Get Setting String!");
        return "";
    }

    public override void SetSettingValue(string value)
    {
        Debug.Log("Set Setting String!");
        base.SetSettingValue(value);

    }
}

[System.Serializable]
public class IntSettingBinding : SettingBinding<int>
{
    public override int GetSettingValue()
    {
        if (SettingsManager.Instance == null)
            Debug.LogError("NULL SAVE MANAGER");
        switch (Key)
        {
            case SettingKey.CAMERA_INVERT_X:
                return (SettingsManager.Instance.invertXAxis == true) ? 1 : 0;
                break;
            case SettingKey.CAMERA_INVERT_Y:
                return (SettingsManager.Instance.invertYAxis == true) ? 1 : 0;
                break;
            case SettingKey.CONTROLLER_RUMBLE:
                return (SettingsManager.Instance.allowVibration == true) ? 1 : 0;
                break;
            case SettingKey.MILITARY_TIME:
                return (SettingsManager.Instance.useMilitaryTime == true) ? 1 : 0;
                break;
            case SettingKey.VID_FULLSCREEN:
                return (SettingsManager.Instance.fullScreen == true) ? 1 : 0;
                break;
            case SettingKey.VID_RESOLUTION_HEIGHT:
                return SettingsManager.Instance.resolutionHeight;
                break;
            case SettingKey.VID_RESOLUTION_WIDTH:
                return SettingsManager.Instance.resolutionWidth;
                break;
            default:
                Debug.Log("Key not found: " + Key);
                break;
        }
        return 1;
    }

    public override void SetSettingValue(int value)
    {
        base.SetSettingValue(value);
        SetValue(value);
    }

    void SetValue(int value)
    {
        switch (Key)
        {
            case SettingKey.CAMERA_INVERT_X:
                SettingsManager.Instance.invertXAxis = (value == 1) ? true : false;
                break;
            case SettingKey.CAMERA_INVERT_Y:
                SettingsManager.Instance.invertYAxis = (value == 1) ? true : false;
                break;
            case SettingKey.CONTROLLER_RUMBLE:
                SettingsManager.Instance.allowVibration = (value == 1) ? true : false;
                break;
            case SettingKey.MILITARY_TIME:
                SettingsManager.Instance.useMilitaryTime = (value == 1) ? true : false;
                break;
            case SettingKey.VID_FULLSCREEN:
                SettingsManager.Instance.fullScreen = (value == 1) ? true : false;
                break;
            case SettingKey.VID_RESOLUTION_HEIGHT:
                SettingsManager.Instance.resolutionHeight = (ushort)value;
                break;
            case SettingKey.VID_RESOLUTION_WIDTH:
                SettingsManager.Instance.resolutionWidth = (ushort)value;
                break;
            default:
                Debug.Log("Key not found: " + Key);
                break;
        }
    }
}

[System.Serializable]
public class BoolSettingBinding : SettingBinding<bool>
{
    public override bool GetSettingValue()
    {
        if (SettingsManager.Instance == null)
            Debug.LogError("NULL SAVE MNGR");

        switch (Key)
        {
            case SettingKey.CAMERA_INVERT_X:
                return SettingsManager.Instance.invertXAxis;
                break;
            case SettingKey.CAMERA_INVERT_Y:
                return SettingsManager.Instance.invertYAxis;
                break;
            case SettingKey.CONTROLLER_RUMBLE:
                return SettingsManager.Instance.allowVibration;
                break;
            case SettingKey.MILITARY_TIME:
                return SettingsManager.Instance.useMilitaryTime;
                break;
            case SettingKey.VID_FULLSCREEN:
                return SettingsManager.Instance.fullScreen;
                break;
            default:
                Debug.Log("Key not found: " + Key);
                break;
        }
        return true;
    }

    public override void SetSettingValue(bool value)
    {
        base.SetSettingValue(value);
        SetValue(value);
    }

    private static int BoolToInt(bool value)
    {
        return value == true ? 1 : 0;
    }

    void SetValue(bool value)
    {
        switch (Key)
        {
            case SettingKey.CAMERA_INVERT_X:
                SettingsManager.Instance.invertXAxis = value;
                break;
            case SettingKey.CAMERA_INVERT_Y:
                SettingsManager.Instance.invertYAxis = value;
                break;
            case SettingKey.CONTROLLER_RUMBLE:
                SettingsManager.Instance.allowVibration = value;
                break;
            case SettingKey.MILITARY_TIME:
                SettingsManager.Instance.useMilitaryTime = value;
                break;
            case SettingKey.VID_FULLSCREEN:
                SettingsManager.Instance.fullScreen = value;
                break;
            default:
                Debug.Log("Key not found: " + Key);
                break;
        }
    }
}