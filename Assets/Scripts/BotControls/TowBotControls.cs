using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowBotControls : BreakerBotControls
{
    public Transform towPoint { get => turretPoint; }

    [SerializeField] private DistanceJoint2D distanceJoint;
    public DistanceJoint2D DistanceJoint { get => distanceJoint; }

    private TowHook shotTowHook;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShootMissile();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DetachHook();
        }
    }

    public void SetUpDistanceJoint(Debris debris, Vector2 contactPoint)
    {
        distanceJoint.connectedBody = debris.GetComponent<Rigidbody2D>();
        distanceJoint.connectedAnchor = debris.transform.InverseTransformPoint(contactPoint);
        distanceJoint.distance = Vector2.Distance(transform.position, debris.transform.position);
        distanceJoint.enabled = true;
    }

    protected override void ShootMissile()
    {
        if (!IsEnabled) return;

        if (onCooldown) return;

        if (loadedProjectile == null) return;

        loadedProjectile.transform.SetParent(null);
        Rigidbody2D projectileRb = loadedProjectile.GetComponent<Rigidbody2D>();
        projectileRb.isKinematic = false;
        projectileRb.velocity = (Utility.MouseToWorldPos() - (Vector2)transform.position).normalized * 25f;
        shotTowHook = loadedProjectile.GetComponent<TowHook>();
        shotTowHook.HookDestroyed += OnHookDestroyed;

        loadedProjectile = null;
    }

    private void DetachHook()
    {
        if (shotTowHook != null)
        {
            Destroy(shotTowHook.gameObject);
        }

        LoadProjectile();
    }

    private void OnHookDestroyed(TowHook hook)
    {
        if (hook != shotTowHook) return;

        shotTowHook.HookDestroyed -= OnHookDestroyed;

        shotTowHook = null;
        distanceJoint.enabled = false;
    }
}
