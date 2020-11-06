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

        loadedProjectile.transform.SetParent(null);
        Rigidbody2D projectileRb = loadedProjectile.GetComponent<Rigidbody2D>();
        projectileRb.isKinematic = false;
        projectileRb.velocity = (Utility.MouseToWorldPos() - (Vector2)transform.position).normalized * 25f;

        loadedProjectile = null;
        cooldownTimer = reloadTime;
        onCooldown = true;
    }
}
