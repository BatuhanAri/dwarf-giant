using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GiantAI : MonoBehaviour
{
    public enum GiantState { Idle, Patrol, Chase, Attack }
    
    [Header("AI Settings")]
    public GiantState currentState = GiantState.Idle;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float detectionRadius = 15f;
    public float catchDistance = 2.5f;
    public float fieldOfViewAngle = 110f;

    [Header("References")]
    public Transform playerTarget;
    public Transform[] patrolPoints;
    private NavMeshAgent agent;
    private int currentPatrolIndex;

    private GameManager gameManager;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindFirstObjectByType<GameManager>();
        
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTarget = player.transform;
        }
        
        SetState(GiantState.Patrol);
    }

    void Update()
    {
        switch (currentState)
        {
            case GiantState.Idle:
                // Look around, transition back to patrol
                break;
            case GiantState.Patrol:
                UpdatePatrol();
                CheckForPlayer();
                break;
            case GiantState.Chase:
                UpdateChase();
                CheckCatchDistance();
                break;
            case GiantState.Attack:
                // Playing attack animation/logic
                break;
        }
    }

    private void SetState(GiantState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case GiantState.Patrol:
                agent.speed = patrolSpeed;
                GoToNextPatrolPoint();
                break;
            case GiantState.Chase:
                agent.speed = chaseSpeed;
                break;
            case GiantState.Attack:
                agent.ResetPath();
                if (gameManager != null) gameManager.OnPlayerCaught();
                break;
        }
    }

    private void UpdatePatrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private void UpdateChase()
    {
        if (playerTarget != null)
        {
            agent.SetDestination(playerTarget.position);
            
            // If lost line of sight for a while, could return to patrol
            // Keeping it simple: always chase once seen, unless distance is too huge
            if (Vector3.Distance(transform.position, playerTarget.position) > detectionRadius * 2)
            {
                SetState(GiantState.Patrol);
            }
        }
    }

    private void CheckForPlayer()
    {
        if (playerTarget == null) return;

        Vector3 directionToPlayer = playerTarget.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Check if within detection radius
        if (distanceToPlayer <= detectionRadius)
        {
            // Check Field of View
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer.normalized);
            if (angleToPlayer < fieldOfViewAngle / 2f)
            {
                // Raycast to check line of sight (avoid seeing through walls)
                if (Physics.Raycast(transform.position + Vector3.up * 1f, directionToPlayer.normalized, out RaycastHit hit, detectionRadius))
                {
                    if (hit.transform == playerTarget || hit.transform.CompareTag("Player"))
                    {
                        // Player detected
                        SetState(GiantState.Chase);
                    }
                }
            }
        }
    }

    private void CheckCatchDistance()
    {
        if (playerTarget == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer <= catchDistance)
        {
            SetState(GiantState.Attack);
        }
    }
}
