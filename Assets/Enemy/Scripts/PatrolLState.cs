using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : StateMachineBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0;
    private float waitTime = 2f; // Tempo que o inimigo espera antes de ir para o próximo waypoint
    private float waitCounter;
    private bool waiting;

    public static float chaseRange = 8f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = 2f;
        agent.isStopped = false;

        animator.SetBool("IsPatrolling", true);

        waypoints.Clear();
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("Waypoint");

        foreach (GameObject waypoint in waypointObjects)
        {
            waypoints.Add(waypoint.transform);
        }

        if (waypoints.Count > 0)
        {
            currentWaypointIndex = Random.Range(0, waypoints.Count);
            MoveToNextWaypoint();
        }

        waiting = false;
        waitCounter = waitTime;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent == null || waypoints.Count == 0) return;

        // Checa a distância do jogador em todo frame
        float distanceToPlayer = Vector3.Distance(player.position, animator.transform.position);

        if (distanceToPlayer < chaseRange)
        {
            animator.SetBool("IsChasing", true);
            return; // Sai do update para evitar que o inimigo continue patrulhando enquanto persegue
        }

        // Patrulha normal
        if (waiting)
        {
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0)
            {
                waiting = false;
                MoveToNextWaypoint();
            }
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                waiting = true;
                waitCounter = waitTime;
            }
        }
    }

    private void MoveToNextWaypoint()
    {
        if (waypoints.Count == 0) return;

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent != null)
        {
            agent.isStopped = false;
        }
        animator.SetBool("IsPatrolling", false); // Garante que a animação de patrulha seja desligada ao sair
    }
}