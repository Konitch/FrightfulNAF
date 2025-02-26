using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// De preferência não alterar esse código pois ele já está definido com uma lógica americana,
// como é um sistema complexo de entender para brasileiros, isso pode dificultar a construção de uma lógica


public class GameClock : MonoBehaviour
{
    public Text clockText; // Referência ao texto do relógio na UI
    public CanvasGroup endScreen; // Referência ao CanvasGroup da tela "6 AM"

    private int currentHour = 12; // Começa às 12 AM
    private float timeElapsed = 0f;
    private float minuteDuration = 60f; // Alterar pra 1 / 5 minutos reais por hora no jogo

    void Start()
    {
        UpdateClockUI(); // Garante que a UI comece certa
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= minuteDuration)
        {
            timeElapsed = 0;
            if (currentHour==12)
                currentHour=1;
            else
                currentHour++;
            UpdateClockUI();
        }

        if (currentHour > 4 && currentHour < 6)
        {
            StartCoroutine(ShowEndScreen());
        }
    }

    void UpdateClockUI()
    {
        if (clockText != null)
        {
            clockText.text = $"{currentHour} AM";
            Debug.Log($"Hora atual: {currentHour} AM"); // Debug para ver se a hora está mudando
        }
    }

    IEnumerator ShowEndScreen()
    {
        if (endScreen != null)
        {
            float fadeDuration = 2f;
            float timer = 0;

            // Ativar o CanvasGroup e garantir que ele está visível
            endScreen.gameObject.SetActive(true);

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                endScreen.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                yield return null;
            }

            yield return new WaitForSeconds(3f); // Espera um tempo antes de reiniciar
            RestartGame();
        }
    }

    void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}