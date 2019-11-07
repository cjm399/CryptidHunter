
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NightWalkerTempAI : MonoBehaviour
{
    private NavMeshAgent _agent;
    public float speed;
    public Transform[] movespots;
    private int randomSpot;
    private int state = 0; //state machine tracker 0 is idle, 1 is fleeing
    private float distance;
    public float fovAngle = 110f;
    private Vector3 newPos;
    private Vector3 currentPos;

    public GameObject Player;

    public float EnemyRunDistance = 4.0f;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        randomSpot = Random.Range(0, movespots.Length);
        currentPos = _agent.gameObject.transform.position;
        newPos = currentPos;

    }
    private void Update()
    {
        switch (state) //state machine
        {
            case 0:
                idle_state();
                break;
            case 1:
                flee_state();
                break;
        }

    }

    private void flee_state()
    {
        //TODO Running animation

        Vector3 dirToPlayer = transform.position - Player.transform.position;

        newPos = movespots[randomSpot].position;
        currentPos = _agent.gameObject.transform.position;

        _agent.SetDestination(newPos);                                          //moves nightwalker to random way-point

        GetComponent<AudioSource>().Play();             //plays nightwalker run audio

        if (Vector3.Distance(_agent.gameObject.transform.position,newPos)<2.0f) //checks if agent has (basically) made it to his destination
        {
            GetComponent<AudioSource>().Stop();         //stops audio and switches back to an idle state
            state = 0;
        }

    }

    private void idle_state()
    {
        distance = Vector3.Distance(transform.position, Player.transform.position);
        //TODO play idle animation
        if (distance < EnemyRunDistance)
        {
            Vector3 direction = Player.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);
            if(angle < fovAngle*0.5f) //checks if the player is within the enemy field of view
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, EnemyRunDistance))
                {
                    if(hit.collider.gameObject == Player) //checks if there is anything in front of the player
                    {
                        randomSpot = Random.Range(0, movespots.Length);
                        state = 1;  //activate flee state
                    }
                }
                
            }
            
        }
        else
        {
            state = 0;
        }

    }


}