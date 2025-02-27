using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Text clockText; // Relógio na UI
    public CanvasGroup endScreen; // Tela de "6 AM"
    
    public CanvasGroup gameOverScreen; // Tela de GAME OVER
    public Button retryButton; // Botão de tentar novamente
    public Text objective; // Texto do objetivo
    public Slider staminaBar;
    public Camera jumpscareCamera; // Câmera do jumpscare
    private Camera mainCamera; // Câmera principal original
    public MonoBehaviour FirstPersonController; // Referência ao script de controle do jogador

    public int currentHour = 12;
    public float timeElapsed = 0f;
    private float minuteDuration = 60f; // 5 minutos reais = 1 hora no jogo
    bool beginDone = false;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateClockUI();
        gameOverScreen.gameObject.SetActive(false);
        retryButton.onClick.AddListener(RestartGame);

        mainCamera = Camera.main;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > 2.5f && beginDone == false)
        {
            objective.gameObject.SetActive(true);
            clockText.gameObject.SetActive(true);
            beginDone = true;
        }

        if (timeElapsed >= minuteDuration)
        {
            timeElapsed = 0;
            if (currentHour == 12)
                currentHour = 1;
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
            Debug.Log($"Hora atual: {currentHour} AM");
        }
    }

    IEnumerator ShowEndScreen()
    {
        if (endScreen != null)
        {
            float fadeDuration = 2f;
            float timer = 0;

            endScreen.gameObject.SetActive(true);

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                endScreen.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                yield return null;
            }

            yield return new WaitForSeconds(3f);
            RestartGame();
        }
    }

    public void TriggerJumpscare()
{
    objective.gameObject.SetActive(false);
    clockText.gameObject.SetActive(false);
    staminaBar.gameObject.SetActive(false);

    if (FirstPersonController != null)
    {
        FirstPersonController.enabled = false; // Desativa o controle do jogador
    }

    Cursor.lockState = CursorLockMode.None; // Libera o cursor
    Cursor.visible = true; // Torna o cursor visível

    StartCoroutine(JumpscareSequence());
}

    IEnumerator JumpscareSequence()
    {
        if (jumpscareCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
            jumpscareCamera.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(2.5f); // Tempo da animação do jumpscare

        ShowGameOverScreen();
    }

    void ShowGameOverScreen()
    {
        gameOverScreen.gameObject.SetActive(true);
        gameOverScreen.alpha = 1;
    }

    public void RestartGame()
    {
        if (FirstPersonController != null)
        {
            FirstPersonController.enabled = true; // Ativa o controle do jogador
        }

        Cursor.lockState = CursorLockMode.Locked; // Bloqueia o cursor de novo
        Cursor.visible = false; // Oculta o cursor
        Debug.Log("Botão pressionado! Reiniciando o jogo...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}