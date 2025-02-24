using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DoorInteraction : MonoBehaviour
{
    public float teleportOffset = 2.0f; // Distância para teleporte além da porta
    public CanvasGroup fadeCanvas;
    private Transform player;

    public void UseDoor()
    {
        if (player == null) return;
        StartCoroutine(UseDoorCoroutine());
    }

    private IEnumerator UseDoorCoroutine()
    {
        if (fadeCanvas != null)
        {
            yield return StartCoroutine(Fade(1)); // Fade para preto
        }

        if (player != null)
        {
            // Desativa a colisão para evitar que o jogador fique preso
            CharacterController characterController = player.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
            }

            // Calcula a nova posição e teleporta o jogador
            Vector3 newPosition = GetTeleportPosition();
            player.position = newPosition;

            // Reativa a colisão após um pequeno atraso
            yield return new WaitForSeconds(0.1f);
            if (characterController != null)
            {
                characterController.enabled = true;
            }
        }

        yield return new WaitForSeconds(0.2f);

        if (fadeCanvas != null)
        {
            yield return StartCoroutine(Fade(0)); // Fade para voltar
        }
    }

    private Vector3 GetTeleportPosition()
    {
        // Obtém a direção para a frente da porta (eixo Z local)
        Vector3 doorForward = transform.forward;

        // Descobre qual lado da porta o jogador está (frente ou trás)
        Vector3 toPlayer = player.position - transform.position;
        bool isInFront = Vector3.Dot(toPlayer, doorForward) > 0;

        // Define a direção de teleporte: para frente ou para trás da porta
        Vector3 teleportDirection = isInFront ? -doorForward : doorForward;

        // Calcula a nova posição para onde o jogador será enviado
        return transform.position + (teleportDirection * teleportOffset);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float duration = 0.5f;
        float startAlpha = fadeCanvas.alpha;
        float time = 0;

        while (time < duration)
        {
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        fadeCanvas.alpha = targetAlpha;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }
}