using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSelectorDisplay : MonoBehaviour
{
    [SerializeField] private GameObject broomBotSelector;
    [SerializeField] private GameObject vacuumBotSelector;
    [SerializeField] private GameObject breakerBotSelector;
    [SerializeField] private GameObject towBotSelector;

    private Dictionary<BotType, GameObject> botSelectors = new Dictionary<BotType, GameObject>();

    private void Awake()
    {
        botSelectors.Add(BotType.Broom, broomBotSelector);
        botSelectors.Add(BotType.Vacuum, vacuumBotSelector);
        botSelectors.Add(BotType.Breaker, breakerBotSelector);
        botSelectors.Add(BotType.Tow, towBotSelector);

        GameController.MaterialsAvailableChanged += OnMaterialsChanged;
    }

    private void Start()
    {
        foreach (var item in botSelectors)
        {
            item.Value.GetComponent<BotSelector>().ToggleSelector(false);
        }
    }

    public void ToggleBotSelector(PlayerControls bot, bool isOn = true)
    {
        botSelectors[bot.BotType].SetActive(isOn);
    }

    public void ToggleBotSelector(BotType botType, bool isOn = true)
    {
        botSelectors[botType].GetComponent<BotSelector>().ToggleSelector(isOn);
    }

    private void OnMaterialsChanged(float currentMaterials)
    {
        ToggleBotSelector(BotType.Broom, currentMaterials >= BotController.BroomBotCost);
        ToggleBotSelector(BotType.Vacuum, currentMaterials >= BotController.VacuumBotCost);
        ToggleBotSelector(BotType.Breaker, currentMaterials >= BotController.BreakerBotCost);
        ToggleBotSelector(BotType.Tow, currentMaterials >= BotController.TowBotCost);
    }

    private void OnDestroy()
    {
        GameController.MaterialsAvailableChanged -= OnMaterialsChanged;
    }
}
