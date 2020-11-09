﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private Image healthSliderFill;
    [SerializeField] private Text debrisMassText;
    [SerializeField] private Text debrisNumberText;
    [SerializeField] private Text materialsText;
    [SerializeField] private Text timerText;

    private static GameUIController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BotHealth.HealthChanged += UpdateHealthSlider;
        UpdateDebrisText(0f, 0, 0f);
    }

    private void UpdateHealthSlider(float currentHealth, float maxHealth)
    {
        float scaleX = currentHealth / maxHealth;
        healthSliderFill.transform.localScale = new Vector3(scaleX, 1f, 1f);
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

    private void OnDestroy()
    {
        BotHealth.HealthChanged -= UpdateHealthSlider;
    }
}
