using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Patrol : MonoBehaviour
{
    public float speed;
    private float waitTime;
    public float startWaitTime;

    public Transform[] movespots;
    private int randomSpot;

    void Start()
    {
        waitTime=startWaitTime;
        randomSpot = Random.Range(0, movespots.Length);
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, movespots[randomSpot].position, speed * Time.deltaTime);
        if(Vector2.Distance(transform.position, movespots[randomSpot].position) < 0.2f)
        {
            if(waitTime<=0)
            {
                randomSpot = Random.Range(0, movespots.Length);
                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
        
    }

}
