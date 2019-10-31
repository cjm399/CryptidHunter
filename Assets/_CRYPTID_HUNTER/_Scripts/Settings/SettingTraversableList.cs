using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingTraversableList : BaseSettingComponent
{
    [SerializeField]
    private IntSettingBinding _binding;
    public IntSettingBinding Binding
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

    private TraversableList _list;

    private void Start()
    {
        _list = GetComponentInChildren<TraversableList>();
        _list.OverrideIndex(_binding.GetSettingValue());
        _list.onValueChanged.AddListener(HandleValueChanged);
    }

    private void HandleValueChanged(int val)
    {
        _binding.SetSettingValue(val);
        Raise();
    }
}