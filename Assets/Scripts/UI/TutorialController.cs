using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private List<TutorialData> tutorials;

    [Header("Panel")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Text titleText;
    [SerializeField] private Text paragraph;
    [SerializeField] private Image tutorialImage;

    [Header("Cursors")]
    [SerializeField] private Texture2D crosshair;
    [SerializeField] private Texture2D cursorTexture;

    private static TutorialController Instance;

    public static Action<string> TutorialClosed;
    public static string CurrentTutorialKey;

    private void Awake()
    {
        Instance = this;

        BotController.BotDestroyed += OnBotDestroyed;
    }

    private void OnBotDestroyed(PlayerControls botControls)
    {
        ShowTutorial("SpawningBots");
    }

    public static void ShowTutorial(string key)
    {
        if (TutorialShown(key)) return;

        Debug.Log(Instance.tutorials.Count);

        TutorialData data = Instance.tutorials.Find(x => x.Key == key);

        if (data == null)
        {
            Debug.LogError(string.Format("Tutorial with key {0} not found.", key));
            return;
        }

        Cursor.SetCursor(Instance.cursorTexture, Vector2.zero, CursorMode.Auto);

        Instance.titleText.text = data.Title;
        Instance.tutorialImage.sprite = data.Image;
        Instance.tutorialImage.preserveAspect = true;
        Instance.paragraph.text = data.Paragraph;
        Instance.tutorialPanel.SetActive(true);

        GameUIController.TutorialOpen = true;

        Utility.SetPlayerPrefsBool("tutorial_" + key, true);

        CurrentTutorialKey = key;

        Time.timeScale = 0f;
    }

    public static void CloseTutorial()
    {
        Instance.tutorialPanel.SetActive(false);

        Time.timeScale = 1f;

        GameUIController.TutorialOpen = false;

        Cursor.SetCursor(Instance.crosshair, Vector2.zero, CursorMode.Auto);

        TutorialClosed?.Invoke(CurrentTutorialKey);
    }

    public static void ShowBotTutorial(BotType botType)
    {
        switch (botType)
        {
            case BotType.Vacuum:
                ShowTutorial("VacuumBot");
                break;
            case BotType.Tow:
                ShowTutorial("TowBot");
                break;
            case BotType.Breaker:
                ShowTutorial("BreakerBot");
                break;
            default:
                break;
        }
    }

    public static bool TutorialShown(string key)
    {
        return Utility.GetPlayerPrefsBool("tutorial_" + key);
    }

    private void OnDestroy()
    {
        BotController.BotDestroyed -= OnBotDestroyed;
    }
}
