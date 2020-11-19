using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private Image healthSliderFill;
    [SerializeField] private Text debrisMassText;
    [SerializeField] private Text debrisNumberText;
    [SerializeField] private Text materialsText;
    [SerializeField] private Text timerText;
    [SerializeField] private GameObject pausePanel;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text gameOverHeading;
    [SerializeField] private Text gameOverParagraph;

    [Header("Cursor")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Texture2D menuCursor;

    private bool isPaused = false;

    private static GameUIController Instance;

    public static bool TutorialOpen = false;

    public static bool GamePaused { get => Instance.isPaused; }

    private void Awake()
    {
        Instance = this;

        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    private void Start()
    {
        BotHealth.HealthChanged += UpdateHealthSlider;
        UpdateDebrisText(0f, 0, 0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !TutorialOpen)
        {
            TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && TutorialOpen)
        {
            TutorialController.CloseTutorial();
        }
    }

    private void UpdateHealthSlider(float currentHealth, float maxHealth)
    {
        float scaleX = currentHealth / maxHealth;
        healthSliderFill.transform.localScale = new Vector3(scaleX, 1f, 1f);
    }

    public void GameOverRestart()
    {
        SceneLoadingController.SceneIndexToLoad = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(1);
    }

    public void GameOverToMenu()
    {
        SceneLoadingController.SceneIndexToLoad = 0;
        SceneManager.LoadScene(1);
    }

    public static void UpdateDebrisText(float massCollected, int numberCollected, float materialsAvailable)
    {
        Instance.debrisMassText.text = string.Format("Tons Collected: {0}", Math.Round(massCollected, 2));
        Instance.debrisNumberText.text = string.Format("Pieces: {0}", numberCollected);
        Instance.materialsText.text = materialsAvailable.ToString();
    }

    public static void UpdateTimerText(int timeRemainingSeconds)
    {
        int minutes = timeRemainingSeconds / 60;
        int seconds = timeRemainingSeconds % 60;

        bool useDots = false;

        if (seconds % 2 == 0)
        {
            useDots = true;
        }

        if (useDots)
        {
            Instance.timerText.text = string.Format("{0}:{1}", minutes.ToString("D2"), seconds.ToString("D2"));
        }
        else
        {
            Instance.timerText.text = string.Format("{0} {1}", minutes.ToString("D2"), seconds.ToString("D2"));
        }
    }

    public static void ShowGameOverPanel(bool timerExpired, float debrisPercentage)
    {
        Instance.gameOverPanel.SetActive(true);
        

        if (timerExpired)
        {
            Instance.gameOverHeading.text = "Time's Up!";
            Instance.gameOverParagraph.text = string.Format("You collected {0}% of the space junk.", (debrisPercentage * 100).ToString("0.00"));
        }
        else
        {
            Instance.gameOverHeading.text = "Job Done!";
            Instance.gameOverParagraph.text = "You collected all of the space junk! Incredible!";
        }
    }

    public static void TogglePause()
    {
        if (Instance.isPaused)
        {
            Instance.pausePanel.SetActive(false);
            Time.timeScale = 1f;
            Cursor.SetCursor(Instance.cursorTexture, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Instance.pausePanel.SetActive(true);
            Time.timeScale = 0f;
            Cursor.SetCursor(Instance.menuCursor, Vector2.zero, CursorMode.Auto);
        }

        Instance.isPaused = !Instance.isPaused;
    }

    private void OnDestroy()
    {
        BotHealth.HealthChanged -= UpdateHealthSlider;
    }
}
