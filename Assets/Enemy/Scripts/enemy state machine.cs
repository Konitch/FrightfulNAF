using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour
{
    enum State { Idle, Patrolling, Chasing }
    State currentState;

    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 5.0f;
    Transform targetPoint;
    public bool playerDetect;

    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Obtém o NavMeshAgent
        playerDetect = false;
        currentState = State.Patrolling;
        targetPoint = pointA;

        agent.speed = patrolSpeed; // Configura velocidade inicial
        agent.SetDestination(targetPoint.position); // Define primeiro destino
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Patrolling:
                Patrol();
                break;

            case State.Chasing:
                Chase();
                break;
        }
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = State.Idle; // Para o inimigo quando chega ao ponto
            return; // Sai da função
        }

        if (playerDetect)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 playerDirection = (player.transform.position - transform.position).normalized;

                if (playerDirection.x < 0)
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                else
                    transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            currentState = State.Chasing;
            agent.isStopped = false; // Retoma o movimento
            agent.speed = chaseSpeed;
            Debug.Log("Transition to Chasing");
        }
    }

    void Chase()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            agent.SetDestination(player.transform.position);

            Vector3 playerDirection = (player.transform.position - transform.position).normalized;
            if (playerDirection.x < 0)
                transform.rotation = Quaternion.Euler(0, 180, 0);
            else
                transform.rotation = Quaternion.Euler(0, 0, 0);

            if (Vector3.Distance(transform.position, player.transform.position) > 6.0f)
            {
                currentState = State.Idle;
                Debug.Log("Transition to Idle");
            }
        }
        else
        {
            currentState = State.Idle;
            Debug.Log("Player not found, back to Idle");
        }
    }

    void Idle()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero; // Garante que ele realmente pare
        agent.speed = 0f;

        if (playerDetect) // Se detectar o jogador, começa a perseguição
        {
            currentState = State.Chasing;
            agent.isStopped = false;
            agent.speed = chaseSpeed;
        }
    }
}