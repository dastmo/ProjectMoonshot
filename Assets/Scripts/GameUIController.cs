using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private Image healthSliderFill;

    private void Start()
    {
        BotHealth.HealthChanged += UpdateHealthSlider;
    }

    private void UpdateHealthSlider(float currentHealth, float maxHealth)
    {
        float scaleX = currentHealth / maxHealth;
        healthSliderFill.transform.localScale = new Vector3(scaleX, 1f, 1f);
    }

    private void OnDestroy()
    {
        BotHealth.HealthChanged -= UpdateHealthSlider;
    }
}
