using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakerBotControls : PlayerControls
{
    [SerializeField] protected Transform turretPoint;
    [SerializeField] protected float reloadTime = 3f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] protected AudioClip shootSound;

    protected float cooldownTimer = 0f;
    protected bool onCooldown = false;
    protected GameObject loadedProjectile;

    protected override void Start()
    {
        base.Start();
        LoadProjectile();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        ControlCooldown();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShootMissile();
        }
    }

    protected void LoadProjectile()
    {
        loadedProjectile = Instantiate(projectilePrefab, transform);
        loadedProjectile.transform.localPosition = turretPoint.localPosition;
    }

    protected virtual void ShootMissile()
    {
        if (!IsEnabled) return;

        if (onCooldown) return;

        if (GameUIController.TutorialOpen || GameUIController.GamePaused) return;

        AudioSource.PlayClipAtPoint(shootSound, transform.position, AudioController.SFXVolume);

        loadedProjectile.transform.SetParent(null);
        Rigidbody2D projectileRb = loadedProjectile.GetComponent<Rigidbody2D>();
        projectileRb.isKinematic = false;
        projectileRb.velocity = (Utility.MouseToWorldPos() - (Vector2)transform.position).normalized * 25f;
        loadedProjectile.GetComponentInChildren<TrailRenderer>().emitting = true;
        loadedProjectile.GetComponent<BreakerMissile>().IsShot = true;

        loadedProjectile = null;
        cooldownTimer = reloadTime;
        onCooldown = true;
    }

    private void ControlCooldown()
    {
        if (!onCooldown) return;

        cooldownTimer -= Time.fixedDeltaTime;
        if (cooldownTimer <= 0f)
        {
            onCooldown = false;
            LoadProjectile();
        }
    }
}
