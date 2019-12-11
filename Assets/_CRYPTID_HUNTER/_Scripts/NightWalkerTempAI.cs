
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NightWalkerTempAI : MonoBehaviour
{
    private NavMeshAgent _agent;
    //private ParticleSystem footsteps;
    public float speed;
    private float rspeed;
    public Transform[] movespots;
    private int randomSpot;
    private int state = 0; //state machine tracker 0 is idle, 1 is fleeing
    private float distance;
    public float fovAngle = 110f;
    private Vector3 newPos;
    private Vector3 currentPos;
    private bool executing = false;
    private bool forward = true;
    private bool right = false;
    public float timeInterval = 30f;
    private float last_time = 0f;
    private Animator anim;

    public GameObject Player;

    public float EnemyRunDistance = 4.0f;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = speed;
        //footsteps = GameObject.Find("Footsteps").GetComponent<ParticleSystem>();
        //footsteps.gameObject.SetActive(false);
        randomSpot = Random.Range(0, movespots.Length);
        currentPos = _agent.gameObject.transform.position;
        newPos = currentPos;
        anim = gameObject.GetComponentInChildren<Animator>();

    }
    private void Update()
    {
		// If the Nightwalker isn't paranoid, then don't show a paranoid animation
		if(state != 2)
		{
			anim.SetBool("isParanoid", false);
		}

        switch (state) //state machine
        {
            case 0:
                idle_state();
                break;
            case 1:
                patrol_state();
                break;
            case 2:
                paranoid_state();
                break;
            case 3:
                flee_state();
                break;
            case 4:
                wander();
                break;
        }

    }

    private void flee_state()
    {
        anim.SetBool("isMoving", true);
        //TODO Running animation

        Vector3 dirToPlayer = transform.position - Player.transform.position;

        newPos = movespots[randomSpot].position;
        currentPos = _agent.gameObject.transform.position;

        _agent.speed = speed;
        _agent.SetDestination(newPos);   //moves nightwalker to random way-point

        //if (!GetComponent<AudioSource>().isPlaying)
        //{
        //    GetComponent<AudioSource>().Play();             //plays nightwalker run audio
        //}

        //footsteps.gameObject.SetActive(true);

        if (Vector3.Distance(_agent.gameObject.transform.position,newPos)<2.0f) //checks if agent has (basically) made it to his destination
        {
            //footsteps.gameObject.SetActive(false);
            //GetComponent<AudioSource>().Stop();//stops audio and switches back to an idle state
            anim.SetBool("isMoving", false);
            _agent.speed = speed;
            state = 0;
        }

    }

    private void patrol_state()
    {
        _agent.speed = speed;
        if (executing == false)
        {
            executing = true;
            StartCoroutine(Patrol());
        }
        
        Vector3 direction = Player.transform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);
        if (angle < fovAngle * 0.5f) //checks if the player is within the enemy field of view
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, EnemyRunDistance))
            {
                if (hit.collider.gameObject == Player) //checks if there is anything in front of the player
                {
                    randomSpot = Random.Range(0, movespots.Length);
                    state = 3;  //activate flee state
                }
            }

        }

    }

    IEnumerator Patrol()
    {
        Vector3 patrol_pos = transform.position;
        if (forward == true)
        {
            patrol_pos.z+=2;
            patrol_pos.x += 2;
            _agent.SetDestination(patrol_pos);
            forward = false;
        }
        else if(forward == false)
        {
            patrol_pos.z -= 2;
            patrol_pos.x -= 2;
            _agent.SetDestination(patrol_pos);
            forward = true;

        }
        
        Debug.Log("In coroutine");
        yield return new WaitForSeconds(5);
        executing = false;
    }

    private void paranoid_state()
    {
        bool first = true;
        _agent.speed = speed;
        if (executing == false)
        {
            executing = true;
            
            StartCoroutine(Paranoid_Patrol(first));
            
        }

        Vector3 direction = Player.transform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);
        if (angle < fovAngle * 0.5f) //checks if the player is within the enemy field of view
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, EnemyRunDistance+8f))
            {
                if (hit.collider.gameObject == Player) //checks if there is anything in front of the player
                {
                    randomSpot = Random.Range(0, movespots.Length);
                    state = 3;  //activate flee state
                    first = true;
                }
            }

        }

    }
    IEnumerator Paranoid_Patrol(bool first)
	{
		anim.SetBool("isParanoid", true);
		_agent.isStopped = true;
		yield return new WaitForSeconds(5);

		Vector3 patrol_pos = transform.position;
        anim.SetBool("isMoving", true);
        if ((forward == true) && (right == false))
        {
            patrol_pos.z += 2;
            _agent.SetDestination(patrol_pos);
            right = true;
  

        }
        else if((forward == true) && (right == true))
        {

            patrol_pos.x += 2;
            _agent.SetDestination(patrol_pos);
            forward = false;


        }
        else if ((forward == false) && (right == true))
        {
 
            patrol_pos.z -= 2;
            _agent.SetDestination(patrol_pos);
            right = false;
        }
        else if ((forward == false) && (right == false))
        {

            patrol_pos.x -= 2;
            _agent.SetDestination(patrol_pos);
            forward = true;
        }
        RaycastHit check;
        if (Physics.Raycast(transform.position, transform.forward, out check, 2f))
        {
            if (check.collider.gameObject != Player)
            {
                yield return null;
            }
        }

        _agent.isStopped = false;
        
		while(Vector3.Distance(transform.position, patrol_pos) > .1f)
		{
			_agent.isStopped = PauseManager.Instance.Paused;

			yield return null;
		}
        anim.SetBool("isMoving", false);

        Debug.Log("In coroutine");
		executing = false;
    }

    private void idle_state()
    {
  
        float currentTime = Time.realtimeSinceStartup;
        
        distance = Vector3.Distance(transform.position, Player.transform.position);
        //TODO play idle animation
        bool camera = GameObject.Find("PhotoCamera").GetComponent<PhotoCamera>().CameraOut;
        bool sprinting = GameObject.Find("Player").GetComponent<PlayerWalk>().IsSprinting;
        
        if(currentTime > timeInterval + last_time)
        {
            randomSpot = Random.Range(0, movespots.Length);
            last_time = currentTime;
            state = 4;
            return;

        }
        
        if (distance < EnemyRunDistance)
        {
            if((!camera) || (sprinting)) //if the player is sprinting, or the camera is away activate paranoid
            {
                state = 2;

            }
            else if (distance < EnemyRunDistance/2) //if they are more quiet
            {
                state = 2;

            }
                
            
        }
        else
        {
            state = 0;
        }

    }

    private void wander()
    {
        Vector3 dirToPlayer = transform.position - Player.transform.position;

        newPos = movespots[randomSpot].position;
        currentPos = _agent.gameObject.transform.position;

        _agent.speed = speed * 0.1f;
        anim.SetBool("isMoving", true);
        _agent.SetDestination(newPos);   //moves nightwalker to random way-point

        GetComponent<AudioSource>().Play();             //plays nightwalker run audio
        distance = Vector3.Distance(transform.position, Player.transform.position);
        //TODO play idle animation
        bool camera = GameObject.Find("PhotoCamera").GetComponent<PhotoCamera>().CameraOut;
        bool sprinting = GameObject.Find("Player").GetComponent<PlayerWalk>().IsSprinting;

        Vector3 direction = Player.transform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);
        if (angle < fovAngle * 0.5f) //checks if the player is within the enemy field of view
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, EnemyRunDistance + 8f))
            {
                if (hit.collider.gameObject == Player) //checks if there is anything in front of the player
                {
                    randomSpot = Random.Range(0, movespots.Length);
                    state = 3;  //activate flee state
                }
            }

        }
        //footsteps.gameObject.SetActive(true);
        if (distance < EnemyRunDistance)
        {
            if ((!camera) || (sprinting)) //if the player is sprinting, or the camera is away activate paranoid
            {
                state = 2;

            }
            else //if they are more quiet, a patrol state is still activates, but not paranoid.
            {
                state = 1;

            }


        }
        else
        {
            state = 0;
        }
        if (Vector3.Distance(_agent.gameObject.transform.position, newPos) < 2.0f) //checks if agent has (basically) made it to his destination
        {
            anim.SetBool("isMoving", false);
            GetComponent<AudioSource>().Stop();//stops audio and switches back to an idle state
            _agent.speed = speed;
            state = 0;
        }

    }


}