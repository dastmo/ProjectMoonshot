using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dustbin : MonoBehaviour
{
    [SerializeField] private SpriteRenderer outsideSprite;

    public static Action<float> DebrisCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debris debris = collision.GetComponent<Debris>();

        if (debris)
        {
            DebrisCollected?.Invoke(debris.Size);
            Destroy(collision.gameObject);
            return;
        }

        PlayerControls player = collision.GetComponent<PlayerControls>();

        if (player)
        {
            HideOutsideSprite();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerControls player = collision.GetComponent<PlayerControls>();

        if (player)
        {
            ShowOutsideSprite();
        }
    }

    private void HideOutsideSprite()
    {
        if (outsideSprite == null) outsideSprite = GetComponent<SpriteRenderer>();

        outsideSprite.color = new Color(.5f, .5f, .5f, 0.25f);
    }

    private void ShowOutsideSprite()
    {
        if (outsideSprite == null) outsideSprite = GetComponent<SpriteRenderer>();

        outsideSprite.color = Color.white;
    }
}
