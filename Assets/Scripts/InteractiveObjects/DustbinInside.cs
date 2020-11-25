using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustbinInside : MonoBehaviour
{
    [SerializeField] private AudioClip hitSound;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debris debris = collision.GetComponent<Debris>();

        if (debris)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position, AudioController.SFXVolume);
            Dustbin.DebrisCollected?.Invoke(debris.Size);
            Destroy(collision.gameObject);
            return;
        }
    }
}
