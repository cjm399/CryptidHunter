using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingSlider : BaseSettingComponent
{
	[SerializeField]
	private FloatSettingBinding _binding;
	public FloatSettingBinding Binding
	{
		get
		{
			return _binding;
		}
		protected set
		{
			_binding = value;
		}
	}

	private Slider _slider;

	private void Start()
	{
		_slider = GetComponentInChildren<Slider>();
		_slider.onValueChanged.AddListener(HandleOnSliderValueChanged);
        _slider.value = Mathf.Lerp(_slider.minValue, _slider.maxValue, _binding.GetSettingValue());
	}

	private void HandleOnSliderValueChanged(float val)
	{
		float normalizedValue = Map(_slider.minValue, _slider.maxValue, _slider.minValue, _slider.maxValue, val);
		_binding.SetSettingValue(normalizedValue);
		Raise();
	}

    public float Map(float currentMin, float currentMax, float targetMin, float targetMax, float value)
    {
        float t = Mathf.InverseLerp(currentMin, currentMax, value);
        float ret = Mathf.Lerp(targetMin, targetMax, t);
        return ret;
    }
}