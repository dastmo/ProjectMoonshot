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
    [SerializeField] private Text gameOverLetterGrade;
    [SerializeField] private Text gameOverTime;
    [SerializeField] private Text gameOverDebris;
    [SerializeField] private Text gameOverTotalScore;

    [Header("Cursor")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Texture2D menuCursor;

    private bool isPaused = false;

    private static GameUIController Instance;

    public static bool TutorialOpen = false;

    public static bool GamePaused { get => Instance.isPaused; }

    private ScoreController scoreController;

    private void Awake()
    {
        Instance = this;

        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        scoreController = FindObjectOfType<ScoreController>();
        scoreController.ScoreCalculated += ShowGameOverPanel;
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

    public static void ShowGameOverPanel()
    {
        Instance.gameOverPanel.SetActive(true);

        string letterGrade = Instance.scoreController.FinalLetterGrade;
        int score = Instance.scoreController.FinalScore;
        int timeRemainingSeconds = Instance.scoreController.SecondsRemaining;
        int timeRemainingScore = timeRemainingSeconds * 1000;
        float debrisMassPercentage = Instance.scoreController.DebrisCollectedPercentage;
        int debrisScore = Instance.scoreController.DebrisScore;

        Instance.gameOverLetterGrade.text = letterGrade;
        Instance.gameOverTotalScore.text = string.Format("Total Score: {0:n0}", score);

        int minutesRemaining = timeRemainingSeconds / 60;
        int secondsRemaining = timeRemainingSeconds % 60;
        Instance.gameOverTime.text = string.Format("Time Remaining: {0:n0} ({1}:{2})", timeRemainingScore, minutesRemaining.ToString("D2"), secondsRemaining.ToString("D2"));

        Instance.gameOverDebris.text = string.Format("Debris Collected: {0:n0} ({1}%)", debrisScore, (debrisMassPercentage * 100).ToString("0.00"));

        if (timeRemainingScore <= 0)
        {
            Instance.gameOverHeading.text = "Time's Up!";
        }
        else
        {
            Instance.gameOverHeading.text = "Job Done!";
        }

        Cursor.SetCursor(Instance.menuCursor, Vector2.zero, CursorMode.Auto);
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
        scoreController.ScoreCalculated -= ShowGameOverPanel;
    }
}
