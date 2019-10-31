
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
    private int state = 0;
    private float distance;
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
        switch (state)
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

        _agent.SetDestination(newPos);
        FindObjectOfType<AudioManager>().Play("cryptidRun");
        if (Vector3.Distance(_agent.gameObject.transform.position,newPos)<2.0f)
        {
            FindObjectOfType<AudioManager>().Stop("cryptidRun");
            state = 0;
        }

    }

    private void idle_state()
    {
        distance = Vector3.Distance(transform.position, Player.transform.position);
        //TODO play idle animation
        if (distance < EnemyRunDistance)
        {
            randomSpot = Random.Range(0, movespots.Length);
            state = 1;
        }
        else
        {
            state = 0;
        }

    }


}