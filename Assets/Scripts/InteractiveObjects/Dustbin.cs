using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dustbin : MonoBehaviour
{
    [SerializeField] private SpriteRenderer outsideSprite;
    [SerializeField] private Collider2D insideTrigger;
    [SerializeField] private Collider2D shipsTrigger;

    [SerializeField] private AudioClip hitSound;

    public static Action<float> DebrisCollected;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioSource.PlayClipAtPoint(hitSound, transform.position, AudioController.SFXVolume);
    }
}
