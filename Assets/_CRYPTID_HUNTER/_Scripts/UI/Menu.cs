using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public void QuitButton() {
        Application.Quit();
        Debug.Log("Quit the Game");
    }
}
