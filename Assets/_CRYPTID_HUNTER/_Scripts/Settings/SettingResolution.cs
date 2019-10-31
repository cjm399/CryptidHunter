using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SettingResolution : BaseSettingComponent
{
	public IntSettingBinding Width;
	public IntSettingBinding Height;

	private TraversableList _list;

	private void Awake()
	{
		_list = GetComponentInChildren<TraversableList>();
		PopulateListValues();
		InitializeToList();

        if(!Application.isEditor)
            _list.onValueChanged.AddListener(HandleValueChanged);
	}

	private int GetWidth()
	{
		return Width.GetSettingValue();
	}

	private int GetHeight()
	{
		return Height.GetSettingValue();
	}

	private void PopulateListValues()
	{
        // Get resolutions with one refresh rate
        var culledResolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct();

        // Store the resolutions in an array
        Resolution[] resolutions = culledResolutions.ToArray();
		List<string> resStrings = new List<string>();

		for (int i = 0; i < resolutions.Length; i++)
		{
			resStrings.Add(
				ResolutionString(resolutions[i].width, resolutions[i].height)
			);
		}
		_list.DisplayOpts = resStrings;
	}

	private void InitializeToList()
	{
		if (Width.IsSet() == false) // check if res is already saved, if not we handle it on our own here
		{
            //Set Switch Resolution
#if UNITY_SWITCH
            Width.SetSettingValue(1280);
            Height.SetSettingValue(720);
#else
            Width.SetSettingValue(Screen.width);
			Height.SetSettingValue(Screen.height); // make sure both are set here
#endif
		}
		string searchString = ResolutionString(GetWidth(), GetHeight());
		bool initList = _list.TrySetToString(searchString);
		if (initList == false)
		{
			print("Resolution not supported on this machine?");
		}
	}

	private string ResolutionString(int width, int height)
	{
		return string.Format("{0}x{1}", width, height);
	}

	private void HandleValueChanged(int val)
	{
		string Display = _list.DisplayOpts[val];
		string[] separated = Display.Split('x');
		int width = Int32.Parse(separated[0]);
		Width.SetSettingValue(width);

		int height = Int32.Parse(separated[1]);
		Height.SetSettingValue(height);
		Raise();
	}
}