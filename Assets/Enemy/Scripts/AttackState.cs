using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : StateMachineBehaviour
{
    // float distance;
    // public static float chaseRange = 8;

    // NavMeshAgent agent;
    // Transform player;

    // // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    // override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     agent = animator.GetComponent<NavMeshAgent>();
    //     player = GameObject.FindGameObjectWithTag("Player").transform;
    // }

    // // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    // override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     animator.transform.LookAt(player);

    //     distance = Vector3.Distance(player.position, animator.transform.position);
    //     if (distance > 3.5f)
    //     {
    //         animator.SetBool("IsBiting", false);
    //     }
    //     if (distance > 6f)
    //     {
    //         animator.SetBool("IsAdvancing", false);
    //     }
    //     if (distance > 8.5f)
    //     {
    //         animator.SetBool("IsFiring", false);
    //     }
    // }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     animator.SetBool("IsBiting", false);
    // }

    // // OnStateMove is called right after Animator.OnAnimatorMove()
    // override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //    // Implement code that processes and affects root motion
    // }

    // // OnStateIK is called right after Animator.OnAnimatorIK()
    // override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     // Implement code that sets up animation IK (inverse kinematics)
    // }
}
