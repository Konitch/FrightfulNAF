using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : StateMachineBehaviour
{

    
    System.Random random = new System.Random();
    float timer;
    int condition = 5;
    float distance;
    public static float chaseRange = 8;

    BlackRabbit blackRabbit;

    NavMeshAgent agent;
    Transform player;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        agent = animator.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.speed = 3.5f;
        blackRabbit = animator.GetComponent<BlackRabbit>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        agent.SetDestination(player.position);


        distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance > 40)
        {
            animator.SetBool("IsChasing", false);
        } else if(timer > condition)
        {
            condition = random.Next(5, 11);
            animator.SetBool("IsRoaring", true);
            animator.SetBool("IsPatrolling", false);
            animator.SetBool("IsChasing", false);
        }

        if (distance < 2.5f)
        {
            blackRabbit.StartCoroutine(blackRabbit.PlayBiteAnimation(animator));
        }
    }

    

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(animator.transform.position);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that processes and affects root motion
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that sets up animation IK (inverse kinematics)
    }
}
