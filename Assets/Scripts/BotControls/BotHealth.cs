using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotHealth : MonoBehaviour
{
    [SerializeField] private float startingHealth = 100f;
    [SerializeField] private Collider2D bodyCollider;

    private bool canTakeDamage = true;

    private PlayerControls playerControls;

    public static Action<float, float> HealthChanged;

    public float CurrentHealth
    {
        get
        {
            float result = startingHealth - damage;
            if (result < 0f) result = 0f;
            return result;
        }
    }

    private float damage = 0f;

    private void Awake()
    {
        playerControls = GetComponent<PlayerControls>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude > 5f && canTakeDamage)
        {
            TakeDamage(collision);
        }
    }

    public void BotSelected()
    {
        HealthChanged?.Invoke(CurrentHealth, startingHealth);
    }

    private void TakeDamage(Collision2D collision)
    {
        if (collision.otherCollider != bodyCollider || !playerControls.IsEnabled) return;

        Debris debris = collision.gameObject.GetComponent<Debris>();

        if (debris)
        {
            damage += collision.relativeVelocity.magnitude;
        }

        HealthChanged?.Invoke(CurrentHealth, startingHealth);

        StartCoroutine(DamageCooldown());
    }

    private IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(3f);
        canTakeDamage = true;
    }
}
