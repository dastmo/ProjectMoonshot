using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    [SerializeField] private GameObject broomBotPrefab;
    [SerializeField] private GameObject vacuumBotPrefab;
    [SerializeField] private GameObject towBotPrefab;
    [SerializeField] private GameObject breakerBotPrefab;

    [SerializeField] private float broomBotCost = 0f;
    [SerializeField] private float vacuumBotCost = 25f;
    [SerializeField] private float breakerBotCost = 50f;
    [SerializeField] private float towBotCost = 100f;

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
            SelectBot(BotType.Breaker);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectBot(BotType.Tow);
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

    public bool SpawnBot(BotType botType)
    {
        GameObject prefabToUse = null;

        float materialsRequired = 0f;

        switch (botType)
        {
            case BotType.Broom:
                prefabToUse = broomBotPrefab;
                materialsRequired = broomBotCost;
                break;
            case BotType.Vacuum:
                prefabToUse = vacuumBotPrefab;
                materialsRequired = vacuumBotCost;
                break;
            case BotType.Tow:
                prefabToUse = towBotPrefab;
                materialsRequired = towBotCost;
                break;
            case BotType.Breaker:
                prefabToUse = breakerBotPrefab;
                materialsRequired = breakerBotCost;
                break;
            default:
                break;
        }

        if (prefabToUse == null || materialsRequired > GameController.MaterialsAvailable) return false;

        GameObject newBot = Instantiate(prefabToUse, GameController.DustbinPosition, GameController.DustbinRotation);
        newBot.GetComponent<PlayerControls>().IsEnabled = true;
        GameController.MaterialsAvailable -= materialsRequired;
        GameController.CenterCameraOnTarget(newBot.transform);
        GameController.OnBotSelect(newBot.gameObject);
        return true;
    }

    public void SelectBot(BotType botType)
    {
        PlayerControls botOfType = SpawnedBots.Find(x => x.BotType == botType);

        if (botOfType == null)
        {
            if (!SpawnBot(botType))
            {
                return;
            }
        }

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
