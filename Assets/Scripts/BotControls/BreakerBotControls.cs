﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakerBotControls : PlayerControls
{
    [SerializeField] private Transform turretPoint;
    [SerializeField] private float reloadTime = 3f;
    [SerializeField] private GameObject projectilePrefab;

    private float cooldownTimer = 0f;
    private bool onCooldown = false;
    private GameObject loadedProjectile;

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

    private void LoadProjectile()
    {
        Debug.Log("Loading projectile.");
        loadedProjectile = Instantiate(projectilePrefab, transform);
        loadedProjectile.transform.localPosition = turretPoint.localPosition;
    }

    private void ShootMissile()
    {
        Debug.Log("Shooting!");
        if (onCooldown) return;

        loadedProjectile.transform.SetParent(null);
        Rigidbody2D projectileRb = loadedProjectile.GetComponent<Rigidbody2D>();
        projectileRb.isKinematic = false;
        projectileRb.velocity = (Utility.MouseToWorldPos() - (Vector2)transform.position).normalized * 25f;
        loadedProjectile.GetComponentInChildren<TrailRenderer>().emitting = true;

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