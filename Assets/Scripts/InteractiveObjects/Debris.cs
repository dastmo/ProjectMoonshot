﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
    public float maxSize { get; set; } = 50f;
    public float minSize { get; set; } = 10f;

    private Rigidbody2D rb;

    private float size;

    public float Size { get => size; }

    public bool AutoSetValues { get; set; } = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (AutoSetValues)
        {
            SetInitialSize();
            rb.velocity = Utility.RandomVector2(-5f, 5f);
        }

        GameController.TotalDebrisCount += 1;
        GameController.TotalDebrisMass += rb.mass;

        CreateFissures();
    }

    private void FixedUpdate()
    {
        if (GameController.IsOutsidePlayArea(transform)) Destroy(gameObject);
    }

    private void SetInitialSize()
    {
        size = Random.Range(minSize, maxSize);
        transform.localScale = new Vector2(size, size);
        rb.mass = size;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;

        Debris debris = collision.gameObject.GetComponent<Debris>();

        if (debris && impactForce > 5f)
        {
            BreakDown();
        }
    }

    private void CreateFissures()
    {
        if (size < 10f) return;

        int numberOfFissures = Random.Range(1, 5);

        for (int i = 0; i < numberOfFissures; i++)
        {
            Vector2 fissureDir = Utility.RandomVector2(-1f, 1f).normalized;
            GameObject newFissure = Instantiate(GameController.FissurePrefab, transform);
            newFissure.transform.localPosition = fissureDir * 0.1f; // Magic number here. Link to collider radius.
            Utility.LookAt2d(newFissure.transform, (Vector2) transform.position);
        }
    }

    public void BreakDown()
    {
        if (size < 10f) return;

        int numberOfPieces = Random.Range(2, 8);
        for (int i = 0; i < numberOfPieces; i++)
        {
            GameObject newPiece = GameController.SpawnDebris(transform.position);
            Debris newPieceComp = newPiece.GetComponent<Debris>();
            newPieceComp.maxSize = size / numberOfPieces;
            newPieceComp.minSize = newPieceComp.maxSize;

            Vector2 randomForce = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));

            newPiece.GetComponent<Rigidbody2D>().AddForce(randomForce, ForceMode2D.Impulse);
        }

        Destroy(gameObject);
    }

    public void SetSize(float newSize)
    {
        size = newSize;
        transform.localScale = new Vector2(size, size);

        if (rb == null) rb = GetComponent<Rigidbody2D>();

        rb.mass = size;
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.velocity = newVelocity;
    }

    public void ApplyForce(Vector2 forceToApply)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.AddForce(forceToApply, ForceMode2D.Impulse);
    }

    private void OnDestroy()
    {
        GameController.TotalDebrisCount -= 1;
        GameController.TotalDebrisMass -= rb.mass;
    }
}