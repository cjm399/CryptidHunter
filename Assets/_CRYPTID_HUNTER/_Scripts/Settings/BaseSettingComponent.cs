using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class BaseSettingComponent : MonoBehaviour
{
    public static UnityEvent OnSettingChanged = new UnityEvent();

    protected virtual void Raise()
    {
        OnSettingChanged.Invoke();
    }
}