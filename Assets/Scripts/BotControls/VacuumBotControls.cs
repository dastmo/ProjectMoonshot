using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumBotControls : PlayerControls
{
    [SerializeField] private float maximumBagCapacity;
    [SerializeField] private Transform bagTransform;
    [SerializeField] private Collider2D vacuumAttachmentCollider;
    [SerializeField] private AreaEffector2D vacuumEffector;

    private float currentBagMass;
    private List<float> individualDebrisWeight = new List<float>();

    private bool isSucking = false;
    private bool isBlowing = false;

    private float debrisSpitCooldown = 0.5f;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Input.GetMouseButton(0))
        {
            isSucking = true;
        }
        else if (Input.GetMouseButton(1))
        {
            isBlowing = true;
        }
        else
        {
            isSucking = false;
            isBlowing = false;
        }

        debrisSpitCooldown -= Time.fixedDeltaTime;
        ControlEffectorForce();
        SpitOutDebris();
    }

    private void ControlEffectorForce()
    {
        if (isSucking) vacuumEffector.forceMagnitude = -10f;
        else if (isBlowing) vacuumEffector.forceMagnitude = 10f;
        else vacuumEffector.forceMagnitude = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SuckUpDebris(collision);
    }

    private void SuckUpDebris(Collision2D collision)
    {
        if (collision.otherCollider != vacuumAttachmentCollider) return;

        if (!isSucking) return;

        Debris debris = collision.gameObject.GetComponent<Debris>();

        if (debris == null) return;
        if (debris.Size > 8f) return;

        currentBagMass += debris.Size;
        individualDebrisWeight.Add(debris.Size);
        Destroy(debris.gameObject);
        ControlBagSize();
    }

    private void SpitOutDebris()
    {
        if (!isBlowing) return;
        if (individualDebrisWeight.Count == 0) return;
        if (debrisSpitCooldown > 0f) return;

        GameObject newDebris = GameController.SpawnDebris(transform.position + (transform.right * 2f));
        Debris debrisComponent = newDebris.GetComponent<Debris>();
        debrisComponent.AutoSetValues = false;
        debrisComponent.SetSize(individualDebrisWeight[0]);
        debrisComponent.ApplyForce(transform.right * 100f);
        currentBagMass -= individualDebrisWeight[0];
        individualDebrisWeight.RemoveAt(0);
        debrisSpitCooldown = 0.5f;

        ControlBagSize();
    }

    private void ControlBagSize()
    {
        float scale = currentBagMass / maximumBagCapacity;
        if (scale < 0.25f) scale = 0.25f;
        bagTransform.localScale = new Vector3(scale, scale, scale);
    }
}
