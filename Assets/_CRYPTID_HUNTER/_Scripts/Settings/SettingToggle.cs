using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingToggle : BaseSettingComponent
{
	[SerializeField]
	private BoolSettingBinding _binding;
	public BoolSettingBinding Binding
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

	private Toggle _toggle;

	private void Start()
	{
		_toggle = GetComponentInChildren<Toggle>();
		_toggle.isOn = _binding.GetSettingValue();
		_toggle.onValueChanged.AddListener(HandleToggleValueChanged);
	}

	private void HandleToggleValueChanged(bool val)
	{
		_binding.SetSettingValue(val);
		Raise();
	}
}