using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustbinShipsTrigger : MonoBehaviour
{
    private Dustbin dustbin;

    private Dustbin Dustbin
    {
        get
        {
            if (dustbin == null) dustbin = GetComponentInParent<Dustbin>();
            return dustbin;
        }
    }

    private void Awake()
    {
        dustbin = GetComponentInParent<Dustbin>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerControls player = collision.GetComponent<PlayerControls>();

        if (player)
        {
            Dustbin.HideOutsideSprite();
            player.EnterDustbin(gameObject.layer);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerControls player = collision.GetComponent<PlayerControls>();

        if (player)
        {
            Dustbin.ShowOutsideSprite();
            player.ExitDustbin();
        }
    }
}
