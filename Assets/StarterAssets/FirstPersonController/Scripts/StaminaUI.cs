using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    public Slider StaminaSlider;
    private CanvasGroup canvasGroup; // Para controlar a opacidade

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

        StaminaSlider.maxValue = _maxStamina;
        StaminaSlider.value = _maxStamina;
        UpdateVisibility();
    }

    public void UpdateStamina(float currentStamina, float maxStamina)
    {
        if (StaminaSlider != null)
        {
            StaminaSlider.maxValue = maxStamina;
            StaminaSlider.value = Mathf.Clamp(currentStamina, 0, maxStamina);
            UpdateVisibility();
        }
    }

    private void UpdateVisibility()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = (StaminaSlider.value < StaminaSlider.maxValue) ? 1 : 0; // Some quando cheia
        }
    }
}