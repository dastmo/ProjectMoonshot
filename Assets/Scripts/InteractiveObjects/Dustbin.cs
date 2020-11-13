using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dustbin : MonoBehaviour
{
    [SerializeField] private SpriteRenderer outsideSprite;
    [SerializeField] private Collider2D insideTrigger;
    [SerializeField] private Collider2D shipsTrigger;

    public static Action<float> DebrisCollected;

    /* private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.bounds.Intersects(insideTrigger.bounds))
        {
            Debris debris = collision.GetComponent<Debris>();

            if (debris)
            {
                DebrisCollected?.Invoke(debris.Size);
                Destroy(collision.gameObject);
                return;
            }
        }

        PlayerControls player = collision.GetComponent<PlayerControls>();

        if (player)
        {
            HideOutsideSprite();
            player.EnterDustbin(gameObject.layer);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerControls player = collision.GetComponent<PlayerControls>();

        if (player)
        {
            ShowOutsideSprite();
            player.ExitDustbin();
        }
    } */

    public void HideOutsideSprite()
    {
        if (outsideSprite == null) outsideSprite = GetComponent<SpriteRenderer>();

        outsideSprite.color = new Color(.5f, .5f, .5f, 0.25f);
    }

    public void ShowOutsideSprite()
    {
        if (outsideSprite == null) outsideSprite = GetComponent<SpriteRenderer>();

        outsideSprite.color = Color.white;
    }
}
