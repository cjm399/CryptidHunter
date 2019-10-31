using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TraversableListEvent : UnityEvent<int> { }

public class TraversableList : Selectable
{
    public TraversableListEvent onValueChanged = new TraversableListEvent();
    public Button leftButton, rightButton;

    public int Index = 0;

    public List<string> DisplayOpts;

    public TextMeshProUGUI displayText;

    protected override void Awake()
    {
        base.Awake();
        leftButton.onClick.AddListener(Decrement);
        rightButton.onClick.AddListener(Increment);
        ChangeVal();
    }

    void ChangeVal()
    {
        if (Index >= DisplayOpts.Count)
            Index = DisplayOpts.Count - 1;
        if (Index >= 0)
            displayText.text = DisplayOpts[Index];
        onValueChanged.Invoke(Index);
    }

    // Go through opts and look for target and set the index if it exists
    public bool TrySetToString(string target)
    {
        for (int i = 0; i < DisplayOpts.Count; i++)
        {
            if (DisplayOpts[i].Equals(target))
            {
                Index = i;
                ChangeVal();
                return true;
            }
        }

        return false;
    }

    public void OverrideIndex(int val, bool clamp = true)
    {
        int actual = val;
        if (clamp)
        {
            actual = Mathf.Clamp(val, 0, DisplayOpts.Count);
        }
        Index = actual;
        ChangeVal();
    }

    void Decrement()
    {
        Index--;
        if (Index < 0)
        {
            Index = DisplayOpts.Count - 1;
        }
        ChangeVal();
    }

    void Increment()
    {
        Index++;
        if (Index >= DisplayOpts.Count)
        {
            Index = 0;
        }
        ChangeVal();
    }

    private void Update()
    {
        if (this.currentSelectionState == SelectionState.Highlighted)
        {
            if (GameManager.Instance.RewiredPlayer.GetButtonDown(RewiredConsts.Action.UI_Horizontal))
            {
                Increment();
            }
            else if (GameManager.Instance.RewiredPlayer.GetNegativeButtonDown(RewiredConsts.Action.UI_Horizontal))
            {
                Decrement();
            }
        }
    }
}