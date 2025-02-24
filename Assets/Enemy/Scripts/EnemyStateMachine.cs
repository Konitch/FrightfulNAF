using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    enum State { Patrolling, Chasing }
    State currentState;

    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 5.0f;
    Transform targetPoint;
    public bool playerDetect;
    [HideInInspector]


    void Start()
    {
        playerDetect = false;
        currentState = State.Patrolling;
        targetPoint = pointA;
    }

    void Update()
    {
        switch (currentState)
        {
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
        // Move o oponente na direção do ponto alvo
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        // Verifica se o oponente chegou ao ponto alvo
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            // Alterna o ponto alvo entre pointA e pointB (Utilização de operador ternário)
            // variável = (condição) ? valorSeVerdadeiro : valorSeFalso;
            targetPoint = (targetPoint == pointA) ? pointB : pointA;

            // Vira o oponente na direção do próximo ponto alvo
            Vector3 direction = (targetPoint.position - transform.position).normalized;
            if (direction.x < 0)
            {
                // Virar para a esquerda
                transform.Rotate(0,180,0);
            }
            else
            {
                // Virar para a direita
                transform.Rotate(0, 180, 0);
            }
        }

        // Verificar se o jogador está próximo
        if (playerDetect)
        {
            // Vira o oponente na direção do jogador
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            //verificar se o player foi realmente encontrado e não deixou de existir - pessoas correndo com o cara ainda mort
            if (player != null)
            {
                Vector3 playerDirection = (player.transform.position - transform.position).normalized;
                if (playerDirection.x < 0)
                {
                    // Virar para a esquerda
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    // Virar para a direita
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }

            // Transição para o estado de perseguição
            currentState = State.Chasing;
            Debug.Log("Transition to Chasing");
        }
    }


    void Chase()
    {
        // Encontra o GameObject do jogador utilizando a tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Obtém a posição atual do jogador
            Vector3 playerPosition = player.transform.position;

            // Obtém a posição atual do oponente
            Vector3 currentPosition = transform.position;

            // Cria um novo vetor de destino para o oponente que mantém a posição Y constante
            Vector3 targetPosition = new Vector3(playerPosition.x, currentPosition.y, playerPosition.z);

            // Rotaciona o oponente na direção do jogador
            //RotateTowards(playerPosition);

            // Move o oponente na direção do jogador apenas nas coordenadas X e Z
            transform.position = Vector3.MoveTowards(currentPosition, targetPosition, chaseSpeed * Time.deltaTime);

            // Verifica se a distância entre o oponente e o jogador é maior que um limite
            // Se for, retorna ao estado de patrulha
            if (Vector3.Distance(currentPosition, playerPosition) > 12.0f)
            {
                currentState = State.Patrolling;
                Debug.Log("Transition to Patrolling");
            }
        }
        else
        {
            // Se o jogador não for encontrado, retorna ao estado de patrulha
            currentState = State.Patrolling;
            Debug.Log("Player not found, back to Patrolling");
        }
    }










}