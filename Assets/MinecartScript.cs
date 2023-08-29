using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MinecartScript : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;
    public float cooldownDuration = 10f;
    public GameObject player;
    public float interactionRange = 5f;

    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private Transform passenger;
    private float cooldownTimer = 0;
    private bool isReturning = false; 

    private void Update()
    {
        if (isMoving && passenger)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                waypoints[currentWaypointIndex].position,
                speed * Time.deltaTime
            );
            passenger.transform.position = this.transform.position;
            if (
                Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position)
                < 0.1f
            )
            {
                if (currentWaypointIndex == waypoints.Length - 1 && !isReturning)
                {
                    StopMoving(); // nnnnplease
                }
                else if (currentWaypointIndex == 0 && isReturning)
                {
                    StopMoving(); 
                }
                else
                {
                  //fuck me in the ASS
                    currentWaypointIndex += isReturning ? -1 : 1;
                }
            }
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private void OnMouseDown()
    {
        if (cooldownTimer <= 0 && !passenger && IsPlayerCloseEnough())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Cart"))
                {
                    passenger = player.transform;
                    StartMoving();

                    // shitting my pants?
                    isReturning = (currentWaypointIndex == waypoints.Length - 1);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC") && cooldownTimer <= 0 && !passenger)
        {
            passenger = other.transform;
            StartMoving();

           
            isReturning = (currentWaypointIndex == waypoints.Length - 1);
        }
    }

    private void StartMoving()
    {
        isMoving = true;

        passenger.SetParent(transform); // Make the passenger a child of the mine-craft clan, they will not escape.
        NavMeshAgent agent = passenger.GetComponent<NavMeshAgent>();
        if (agent)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
    }

    private void StopMoving()
    {
        isMoving = false;

        
        NavMeshAgent agent = passenger.GetComponent<NavMeshAgent>();
        if (agent)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        if (passenger)
        {
            passenger.SetParent(null); 
            passenger = null;
        }

        cooldownTimer = cooldownDuration;
    }

    private bool IsPlayerCloseEnough()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= interactionRange;
    }
}
