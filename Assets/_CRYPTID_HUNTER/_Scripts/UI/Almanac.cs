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


    bool almanacOn = false;

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
            if (!almanacOn)
            {
                almanacOn = true;
                almanacObj.SetActive(true);
                playerLook.AddLookBlock(this);

            }
            else
            {
                almanacOn = false;
                almanacObj.SetActive(false);
                playerLook.RemoveLookBlock(this);
            }
            Debug.Log(almanacOn);

        }
    }

    public void ToggleAlmanac()
    {

        
        if (!almanacOn)
        {
            almanacOn = true;
            almanacObj.SetActive(true);
            playerLook.AddLookBlock(this);

        }
        else
        {
            almanacOn = false;
            almanacObj.SetActive(false);
            playerLook.RemoveLookBlock(this);
        }
        Debug.Log(almanacOn);

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
        ToggleAlmanac();
    }


}
