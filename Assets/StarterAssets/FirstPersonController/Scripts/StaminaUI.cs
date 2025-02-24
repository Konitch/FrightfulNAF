using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StaminaUI : MonoBehaviour
{
    public Slider StaminaSlider;
    private CanvasGroup canvasGroup; // Controla a opacidade
    private Image fillImage; // Referência à parte preenchida do slider
    private bool isBlinking = false;

    private float _maxStamina = 5.0f; 

    void Start()
    {
        if (StaminaSlider == null)
        {
            Debug.LogWarning("StaminaSlider não foi atribuído no Inspector.");
            return;
        }

        canvasGroup = StaminaSlider.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = StaminaSlider.gameObject.AddComponent<CanvasGroup>();
        }

        fillImage = StaminaSlider.fillRect.GetComponent<Image>(); // Obtém a parte preenchida

        StaminaSlider.maxValue = _maxStamina;
        StaminaSlider.value = _maxStamina;
        UpdateVisibility();
    }

    public void UpdateStamina(float currentStamina, float maxStamina, bool isTired)
    {
        if (StaminaSlider == null || fillImage == null) return;

        StaminaSlider.maxValue = maxStamina;
        StaminaSlider.value = Mathf.Clamp(currentStamina, 0, maxStamina);

        // Se a stamina zerar, fica vermelho e para de piscar
        if (isTired)
        {
            fillImage.enabled = true;
            fillImage.color = Color.red;
            isBlinking = false;
            StopAllCoroutines();
        }
        // Se a stamina estiver abaixo de 20%, ativa a piscada
        else if (currentStamina <= maxStamina * 0.2f)
        {
            if (!isBlinking)
            {
                isBlinking = true;
                StartCoroutine(BlinkEffect());
            }
        }
        // Caso contrário, volta ao normal
        else
        {
            fillImage.color = Color.white;
            isBlinking = false;
            StopAllCoroutines();
            fillImage.enabled = true;
        }

        UpdateVisibility();
    }

    private IEnumerator BlinkEffect()
    {
        while (StaminaSlider.value > 0 && StaminaSlider.value <= StaminaSlider.maxValue * 0.2f)
        {
            fillImage.enabled = !fillImage.enabled; // Alterna visibilidade
            yield return new WaitForSeconds(0.2f);
        }
        fillImage.enabled = true;
        isBlinking = false;
    }

    private void UpdateVisibility()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = (StaminaSlider.value < StaminaSlider.maxValue) ? 1 : 0; // Some quando cheia
        }
    }
}