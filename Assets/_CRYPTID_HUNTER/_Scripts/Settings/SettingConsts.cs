using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SettingKey
{
	CAMERA_SENS_X,
    CAMERA_SENS_Y,
	CAMERA_INVERT_X,
	CAMERA_INVERT_Y,

	VID_RESOLUTION_WIDTH,
	VID_RESOLUTION_HEIGHT,
	VID_FULLSCREEN,
	VID_BRIGHTNESS,

	AUDIO_VOLUME_MASTER,
	AUDIO_VOLUME_MUSIC,
	AUDIO_VOLUME_EFFECT,

    CONTROLLER_RUMBLE,
    MILITARY_TIME
}

public static class SettingConsts
{
	public static string PrefKey(SettingKey Key)
	{
		return SettingConsts.SETTING_NAME_ROOT + Key.ToString("G");
	}
	public static string SETTING_NAME_ROOT = "CryptidSetting_";
}