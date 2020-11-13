using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotSelector : MonoBehaviour
{
    [SerializeField] private BotType botType;
    [SerializeField] private GameObject priceLabel;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Text priceText;
    [SerializeField] private Image healthBarFill;

    private BotHealth botHealth;

    private void Awake()
    {
        BotController.BotSpawned += OnBotSpawned;
        BotController.BotDestroyed += OnBotDestroyed;
    }

    private void Start()
    {
        healthBar.SetActive(false);
        priceLabel.SetActive(true);
        priceText.text = GetPriceText(botType).ToString();
    }

    private float GetPriceText(BotType botType)
    {
        switch (botType)
        {
            case BotType.Broom:
                return BotController.BroomBotCost;
                break;
            case BotType.Vacuum:
                return BotController.VacuumBotCost;
                break;
            case BotType.Tow:
                return BotController.TowBotCost;
                break;
            case BotType.Breaker:
                return BotController.BreakerBotCost;
                break;
            default:
                return -1;
                break;
        }
    }

    private void OnBotSpawned(PlayerControls bot)
    {
        if (bot.BotType != botType) return;

        priceLabel.SetActive(false);
        healthBar.SetActive(true);

        botHealth = bot.GetComponent<BotHealth>();
        botHealth.TakenDamage += OnDamageTaken;
        ToggleSelector();
        UpdateHealthbar();
    }

    private void OnBotDestroyed(PlayerControls bot)
    {
        if (bot.BotType != botType) return;

        gameObject.SetActive(false);
    }

    private void OnDamageTaken(BotHealth healthComponent)
    {
        UpdateHealthbar();
    }

    private void UpdateHealthbar()
    {
        float healthPercentage = botHealth.CurrentHealth / botHealth.MaxHealth;
        healthBarFill.transform.localScale = new Vector3(healthPercentage, 1f, 1f);
    }

    public void ToggleSelector(bool isOn = true)
    {
        if (!isOn && botHealth != null) return;

        gameObject.SetActive(isOn);
    }

    private void OnDisable()
    {
        if (botHealth != null) botHealth.TakenDamage -= OnDamageTaken;
    }

    private void OnDestroy()
    {
        BotController.BotDestroyed -= OnBotDestroyed;
        if (botHealth != null) botHealth.TakenDamage -= OnDamageTaken;
    }
}
