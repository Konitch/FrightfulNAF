using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public Animator animator;
    public AudioSource chaseMusic;
    public float fadeDuration = 3f; // Tempo do fade (segundos)
    private Coroutine fadeCoroutine; // Para evitar múltiplas transições ao mesmo tempo

    enum State { Patrolling, Chasing, Berserk }
    State currentState;

    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 5.0f;

    float timer;
    Transform targetPoint;
    public bool playerDetect;
    [HideInInspector]


    void Start()
    {
        playerDetect = false;
        currentState = State.Patrolling;
        
        //targetPoint = pointA;
    }

    void Update()
    {
        timer = animator.GetFloat("Timer"); // Obtendo o valor do Animator
        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                break;

            case State.Chasing:
                Chase();
                break;

            case State.Berserk:
                Berserk();
                break;
        }
    }

    void Patrol()
    {
        // Move o oponente na direção do ponto alvo
        //transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        // Verifica se o oponente chegou ao ponto alvo
        // if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        // {
        //     // Alterna o ponto alvo entre pointA e pointB (Utilização de operador ternário)
        //     // variável = (condição) ? valorSeVerdadeiro : valorSeFalso;
        //     targetPoint = (targetPoint == pointA) ? pointB : pointA;

        //     // Vira o oponente na direção do próximo ponto alvo
        //     Vector3 direction = (targetPoint.position - transform.position).normalized;
        //     if (direction.x < 0)
        //     {
        //         // Virar para a esquerda
        //         transform.Rotate(0,180,0);
        //     }
        //     else
        //     {
        //         // Virar para a direita
        //         transform.Rotate(0, 180, 0);
        //     }
        // }

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
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    // Virar para a direita
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }

            // Transição para o estado de perseguição
            currentState = State.Chasing;
            Debug.Log("Transition to Chasing");
        }

        if ((int)timer % 60 == 0 && (int)timer != 0)
        {
            currentState = State.Berserk;
        }
    }

    IEnumerator FadeAudio(AudioSource audioSource, float targetVolume, bool stopAfterFade = false)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;

        if (stopAfterFade && targetVolume == 0f)
        {
            audioSource.Stop();
        }
    }

    void Chase()
    {
        // Encontra o GameObject do jogador utilizando a tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (chaseMusic != null && !chaseMusic.isPlaying)
            {
                if (fadeCoroutine != null)
                    StopCoroutine(fadeCoroutine);
                
                chaseMusic.Play(); // Garante que o áudio está tocando antes do fade-in
                fadeCoroutine = StartCoroutine(FadeAudio(chaseMusic, 0.6f)); // Fade-in para volume 1
            }
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
            if (Vector3.Distance(currentPosition, playerPosition) > 40.0f)
            {
                if (chaseMusic != null && chaseMusic.isPlaying)
                {
                    if (fadeCoroutine != null)
                        StopCoroutine(fadeCoroutine);

                    fadeCoroutine = StartCoroutine(FadeAudio(chaseMusic, 0f, stopAfterFade: true)); // Fade-out para volume 0
                }
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
    void Berserk()
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
            if (Vector3.Distance(currentPosition, playerPosition) < 5.0f)
            {
                currentState = State.Chasing;
                Debug.Log("Transition to Chasing");
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