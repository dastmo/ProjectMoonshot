using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowHook : MonoBehaviour
{
    public Action<TowHook> HookDestroyed;

    private bool isAttached = false;

    public TowBotControls ParentBot { get; set; }

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ParentBot = GetComponentInParent<TowBotControls>();
    }

    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
        if (!isAttached)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, ParentBot.towPoint.position);
        }
        else
        {
            // TODO: Add rope simulation.
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, ParentBot.towPoint.position);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debris debris = collision.gameObject.GetComponent<Debris>();

        if (debris)
        {
            HandleDebris(debris, collision.contacts[0].point);
            return;
        }

        Destroy(gameObject);
    }

    private void HandleDebris(Debris debris, Vector2 contactPoint)
    {
        Destroy(GetComponent<Collider2D>());
        ParentBot.SetUpDistanceJoint(debris, contactPoint);
        GetComponent<Rigidbody2D>().isKinematic = true;
        transform.SetParent(debris.transform);
        Utility.LookAt2d(transform, debris.transform.position);
        isAttached = true;
    }

    private void OnDestroy()
    {
        HookDestroyed?.Invoke(this);
    }
}
