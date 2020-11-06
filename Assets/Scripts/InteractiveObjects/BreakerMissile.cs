using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakerMissile : MonoBehaviour
{
    [SerializeField] private Collider2D explosionCollider;
    [SerializeField] private GameObject explosionParticles;

    private Coroutine flashCoroutine;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debris debris = collision.gameObject.GetComponent<Debris>();

        if (debris)
        {
            HandleDebris(debris);
            return;
        }

        Explode();
    }

    private void HandleDebris(Debris debris)
    {
        Utility.LookAt2d(transform, debris.transform.position);
        GetComponent<Rigidbody2D>().isKinematic = true;
        transform.SetParent(debris.transform);
        Destroy(GetComponent<Collider2D>());
        flashCoroutine = StartCoroutine(FlashAndExplode(3f));
    }

    private void Explode()
    {
        // TODO: Add explosion effect.

        HashSet<Debris> contactDebris = new HashSet<Debris>();
        List<Collider2D> overlappingColliders = new List<Collider2D>();
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = LayerMask.GetMask("Fissures");
        explosionCollider.OverlapCollider(contactFilter, overlappingColliders);

        foreach (var item in overlappingColliders)
        {
            contactDebris.Add(item.GetComponentInParent<Debris>());
        }

        foreach (var item in contactDebris)
        {
            item.BreakDown();
        }

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
