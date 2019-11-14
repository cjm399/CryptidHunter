using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Almanac : MonoBehaviour
{
    [SerializeField]
    private GameObject[] pages;
    private int i;
    // Start is called before the first frame update
    void Start()
    {
        i = 0;
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
}
