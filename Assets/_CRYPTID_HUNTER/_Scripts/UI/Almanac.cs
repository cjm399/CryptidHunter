using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
public class Almanac : MonoBehaviour
{
    [SerializeField]
    private GameObject[] pages;
    private int i;
    // Start is called before the first frame update

    [SerializeField]
    GameObject almanacObj;

    [SerializeField]
    PlayerLook playerLook;


    public bool almanacOn = false;

    [SerializeField]
    Menu menuManager;

    [SerializeField, Tooltip("The Rewired action name for toggling the almanac")]
    string almanacActionName = "Toggle Almanac";
    // Start is called before the first frame update

    private void OnEnable()
    {
        StartCoroutine(InputSubscribe());
        i = 0;
    }
    private void OnDisable()
    {
        GameManager.Instance?.RewiredPlayer?.RemoveInputEventDelegate(TryToggleAlmanac, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, almanacActionName);
    }


    private IEnumerator InputSubscribe()
    {
        while (GameManager.Instance?.RewiredPlayer == null)
        {
            yield return null;
        }

        GameManager.Instance.RewiredPlayer.AddInputEventDelegate(TryToggleAlmanac, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, almanacActionName);
    }
    private void TryToggleAlmanac(InputActionEventData _eventData)
    {
        
        if (_eventData.actionName == almanacActionName)
        {
			ToggleAlmanac();
            Debug.Log(almanacOn);

        }
    }

    public void ToggleAlmanac()
    {
		almanacOn = !almanacOn;

        if (almanacOn)
        {
			OpenAlmanac();

        }
        else
        {
			CloseAlmanac();
        }
    }

	public void OpenAlmanac()
	{
		almanacObj.SetActive(true);
		menuManager.ShowCursor();
		PlayerCharacter.Instance.LockPlayer(this);
	}

	public void CloseAlmanac()
	{
		almanacObj.SetActive(false);
		menuManager.HideCursor();
		PlayerCharacter.Instance.UnlockPlayer(this);
	}

	public void NextPage() 
    {
        pages[i].SetActive(false);
        i++;
        pages[i].SetActive(true);
    }

    public void PrevPage()
    {
        pages[i].SetActive(false);
        i--;
        pages[i].SetActive(true);
    }

    public void ExitAlmanac()
    {
        CloseAlmanac();
    }
}
