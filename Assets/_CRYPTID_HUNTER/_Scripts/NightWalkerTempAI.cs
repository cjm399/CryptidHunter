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
    private bool isMoving = false;

    public GameObject Player;

    public float EnemyRunDistance = 4.0f;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        randomSpot = Random.Range(0, movespots.Length);
        
    }
    private void Update()
    {
        randomSpot = Random.Range(0, movespots.Length);
        float distance = Vector3.Distance(transform.position, Player.transform.position);
        if (distance < EnemyRunDistance)
        {
            Vector3 dirToPlayer = transform.position - Player.transform.position;

            Vector3 newPos = movespots[randomSpot].position;
            Vector3 currentPos = _agent.gameObject.transform.position;

            _agent.SetDestination(newPos);

        }

    }

    // Update is called once per frame

}
