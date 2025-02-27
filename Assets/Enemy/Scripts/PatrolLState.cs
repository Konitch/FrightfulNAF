using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine;

public class PatrolLState : StateMachineBehaviour
{
    Transform player;
    float distance;
    public static float chaseRange = 8;

    float timer = 0;
    int stage=0;
    List<Transform> waypoints = new List<Transform>();
    NavMeshAgent agent; 

    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = 1.5f;
        GameObject go = GameObject.FindGameObjectWithTag("Waypoints");
        foreach (Transform t in go.transform)
            waypoints.Add(t);
        agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].position);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if (waypoints.Count > 0 && agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log("Número de waypoints: " + waypoints.Count);
            agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].position);
        }
        else if (waypoints.Count == 0)
        {
            Debug.LogWarning("Lista de waypoints vazia, patrulha não pode continuar!");
        }
                
        timer += Time.deltaTime;
        animator.SetFloat("Timer", timer); // Envia o valor para o Animator
        if ((int)timer != 0)
        {
            if ((int)timer % 60 == 0)
            {
                animator.SetBool("IsBerserk", true);
                animator.SetBool("IsPatrolling", false);
                stage = 3;
                timer++;
            }
            if ((int)timer % 45 == 0)
            {
                animator.SetBool("IsLooking", true);
                stage = 2;
                timer++;
            }
            if ((int)timer % 15 == 0)
            {
                animator.SetBool("IsAgonizing", true);
                stage = 1;
                timer++;
            }
            else if ((int)timer % 5 == 0) // falta configurar o timer pra que ele continue mesmo depois de chegar em 5
            {
                animator.SetBool("IsPatrolling", false);
                animator.SetBool("InIdle2", true);
                timer++;
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
        agent.SetDestination(agent.transform.position);
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
