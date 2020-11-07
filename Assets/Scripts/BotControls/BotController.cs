using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    [SerializeField] private GameObject broomBotPrefab;
    [SerializeField] private GameObject vacuumBotPrefab;
    [SerializeField] private GameObject towBotPrefab;
    [SerializeField] private GameObject breakerBotPrefab;

    private List<PlayerControls> SpawnedBots = new List<PlayerControls>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectBot(BotType.Broom);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectBot(BotType.Vacuum);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectBot(BotType.Tow);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectBot(BotType.Breaker);
        }
    }

    public void RegisterBot(PlayerControls botControls)
    {
        SpawnedBots.Add(botControls);
    }

    public void UnregisterBot(PlayerControls botControls)
    {
        SpawnedBots.Remove(botControls);
    }

    public void SpawnBot(BotType botType)
    {
        GameObject prefabToUse = null;

        switch (botType)
        {
            case BotType.Broom:
                prefabToUse = broomBotPrefab;
                break;
            case BotType.Vacuum:
                prefabToUse = vacuumBotPrefab;
                break;
            case BotType.Tow:
                prefabToUse = towBotPrefab;
                break;
            case BotType.Breaker:
                prefabToUse = breakerBotPrefab;
                break;
            default:
                break;
        }

        if (prefabToUse == null) return;

        GameObject newBot = Instantiate(prefabToUse, GameController.DustbinPosition, GameController.DustbinRotation);
        newBot.GetComponent<PlayerControls>().IsEnabled = true;
        GameController.CenterCameraOnTarget(newBot.transform);
        GameController.OnBotSelect(newBot.gameObject);
    }

    public void SelectBot(BotType botType)
    {
        PlayerControls botOfType = SpawnedBots.Find(x => x.BotType == botType);

        if (botOfType == null) SpawnBot(botType);

        foreach (PlayerControls bot in SpawnedBots)
        {
            bot.IsEnabled = false;

            if (bot.BotType == botType)
            {
                bot.IsEnabled = true;
                GameController.CenterCameraOnTarget(bot.transform);
                GameController.OnBotSelect(bot.gameObject);
            }
        }
    }
}

public enum BotType
{
    Undefined,
    Broom,
    Vacuum,
    Tow,
    Breaker
}
