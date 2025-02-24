using UnityEngine;
using UnityEngine.UI;

public class DoorRaycast : MonoBehaviour
{
    public Camera playerCamera; // A câmera do jogador
    public float maxDistance = 3f; // Distância máxima de interação
    public Text interactionText; // Texto UI de interação
    private Transform currentDoor; // Referência à porta atual

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("Door")) // A porta precisa ter a tag "Door"
            {
                ShowInteractionText(hit.point);
                currentDoor = hit.collider.transform;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    currentDoor.GetComponent<DoorInteraction>().UseDoor();
                }
            }
            else
            {
                HideInteractionText();
            }
        }
        else
        {
            HideInteractionText();
        }
    }

    void ShowInteractionText(Vector3 worldPosition)
    {
        if (interactionText == null) return;

        interactionText.gameObject.SetActive(true);

        // Converte a posição do mundo para a tela
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        interactionText.transform.position = screenPosition;
    }

    void HideInteractionText()
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }
}