using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private Image healthSliderFill;
    [SerializeField] private Text debrisMassText;
    [SerializeField] private Text debrisNumberText;

    private static GameUIController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BotHealth.HealthChanged += UpdateHealthSlider;
    }

    private void UpdateHealthSlider(float currentHealth, float maxHealth)
    {
        float scaleX = currentHealth / maxHealth;
        healthSliderFill.transform.localScale = new Vector3(scaleX, 1f, 1f);
    }

    public static void UpdateDebrisText(float massCollected, int numberCollected)
    {
        Instance.debrisMassText.text = string.Format("Tons Collected: {0}", Math.Round(massCollected, 2));
        Instance.debrisNumberText.text = string.Format("Pieces: {0}", numberCollected);
    }

    private void OnDestroy()
    {
        BotHealth.HealthChanged -= UpdateHealthSlider;
    }
}
