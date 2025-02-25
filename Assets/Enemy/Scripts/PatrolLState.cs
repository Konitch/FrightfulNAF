using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine;

public class PatrolLState : StateMachineBehaviour
{
    Transform player;
    float distance;
    public static float chaseRange = 8;

    float timer;
    bool isIdle = false; // Novo estado para controlar Patrol/Idle
    List<Transform> waypoints = new List<Transform>();
    NavMeshAgent agent;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = animator.GetComponent<NavMeshAgent>();
        timer = 0;
        agent.speed = 1.5f;

        GameObject go = GameObject.FindGameObjectWithTag("Waypoints");
        if (go != null)
        {
            foreach (Transform t in go.transform)
                waypoints.Add(t);
        }

        if (waypoints.Count > 0)
            agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].position);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if (!isIdle && agent.remainingDistance <= agent.stoppingDistance && waypoints.Count > 0)
        {
            agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].position);
        }

        timer += Time.deltaTime;

        if (timer > 5)
        {
            Debug.Log("Transição vai começar");
            isIdle = !isIdle; // Alterna entre Patrol e Idle
            timer = 0;

            if (isIdle)
            {
                Debug.Log("Tá parado");
                animator.SetBool("IsPatrolling", false);
                agent.isStopped = true; // Para o inimigo no Idle
            }
            else
            {
                Debug.Log("Tá patrulhando");
                animator.SetBool("IsPatrolling", true);
                agent.isStopped = false;
                if (waypoints.Count > 0)
                    agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].position);
            }
        }

        distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance < chaseRange)
        {
            animator.SetBool("IsChasing", true);
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.isStopped = false;;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that processes and affects root motion
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
