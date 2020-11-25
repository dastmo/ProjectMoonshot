using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakerMissile : MonoBehaviour
{
    [SerializeField] private Collider2D explosionCollider;
    [SerializeField] private GameObject explosionParticles;
    [SerializeField] private AudioClip redBeep;

    private Coroutine flashCoroutine;

    public bool IsShot { get; set; } = false;
    private bool isPrimed = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsShot) return;

        Debris debris = collision.gameObject.GetComponent<Debris>();

        if (debris)
        {
            HandleDebris(debris);
            return;
        }

        PrimeOnContact();
    }

    private void HandleDebris(Debris debris)
    {
        Utility.LookAt2d(transform, debris.transform.position);
        GetComponent<Rigidbody2D>().isKinematic = true;
        transform.SetParent(debris.transform);
        Destroy(GetComponent<Collider2D>());
        flashCoroutine = StartCoroutine(FlashAndExplode(3f));
    }

    private void PrimeOnContact()
    {
        if (!isPrimed)
        {
            flashCoroutine = StartCoroutine(FlashAndExplode(3f));
            isPrimed = true;
        }
    }

    private void Explode()
    {
        HashSet<Debris> contactDebris = new HashSet<Debris>();
        List<Collider2D> overlappingColliders = new List<Collider2D>();
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Fissures"));
        contactFilter.useTriggers = true;
        explosionCollider.OverlapCollider(contactFilter, overlappingColliders);

        foreach (var item in overlappingColliders)
        {
            Debug.Log(item.gameObject.name);
            contactDebris.Add(item.GetComponentInParent<Debris>());
        }

        foreach (var item in contactDebris)
        {
            if (item == null) continue;
            item.BreakDown();
        }

        Instantiate(explosionParticles, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private IEnumerator FlashAndExplode(float time)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float currentTimer = 0f;
        bool isRed = false;
        Color originalColor = spriteRenderer.color;

        while(currentTimer < time)
        {
            yield return new WaitForSeconds(0.25f);
            currentTimer += 0.25f;
            
            if (isRed)
            {
                spriteRenderer.color = originalColor;
                isRed = false;
                AudioSource.PlayClipAtPoint(redBeep, transform.position, AudioController.SFXVolume);
            }
            else
            {
                spriteRenderer.color = Color.red;
                isRed = true;
            }
        }

        Explode();
    }

    private void OnDestroy()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
    }
}
