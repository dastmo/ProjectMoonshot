using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotHealth : MonoBehaviour
{
    [SerializeField] private float startingHealth = 100f;
    [SerializeField] private Collider2D bodyCollider;

    private PlayerControls playerControls;

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
        TakeDamage(collision);
    }

    private void TakeDamage(Collision2D collision)
    {
        if (collision.otherCollider != bodyCollider || !playerControls.IsEnabled) return;

        Debris debris = collision.gameObject.GetComponent<Debris>();

        if (debris)
        {
            damage += collision.relativeVelocity.magnitude;
            Debug.Log("Current Health: " + CurrentHealth);
        }
    }
}
