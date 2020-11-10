using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Debris : MonoBehaviour
{
    [SerializeField] private Sprite[] possibleSprites;
    [SerializeField] private PhysicsMaterial2D physicsMaterial;
    [SerializeField] private GameObject breakParticles;

    [Header("Debug")]
    [SerializeField] private bool isDebug = false;
    [SerializeField] private float debugSize = 1f;
    [SerializeField] private Vector2 debugInitialVelocity = Vector2.zero;

    public float maxSize { get; set; } = 50f;
    public float minSize { get; set; } = 10f;

    private Rigidbody2D rb;

    private float size;

    public float Size { get => size; }

    public bool AutoSetValues { get; set; } = true;

    private bool hasSwitchedColliders = false;
    private int fixedUpdateCount = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (AutoSetValues && !isDebug)
        {
            SetInitialSize();
            SetInitialVelocity();
        }

        if (isDebug)
        {
            SetSize(debugSize);
            SetVelocity(debugInitialVelocity);
        }

        GameController.TotalDebrisCount += 1;
        GameController.TotalDebrisMass += rb.mass;

        ChooseSprite();

        gameObject.AddComponent<CircleCollider2D>();

        
    }

    public void SetInitialVelocity(float minVelocity = -5f, float maxVelocity = 5f)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.velocity = Utility.RandomVector2(minVelocity, maxVelocity);
    }

    private void FixedUpdate()
    {
        if (GameController.IsOutsidePlayArea(transform)) Destroy(gameObject);

        if (!hasSwitchedColliders && fixedUpdateCount > 2)
        {
            CreateEdgeCollider();
            CreateFissures();
            hasSwitchedColliders = true;
        }

        if (!hasSwitchedColliders) fixedUpdateCount++;
    }

    private void ChooseSprite()
    {
        int index = Random.Range(0, possibleSprites.Length);
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = possibleSprites[index];
        GetComponent<SpriteMask>().sprite = renderer.sprite;
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
    }

    private void CreateFissures()
    {
        if (size < 10f)
        {
            Destroy(GetComponent<EdgeCollider2D>());
            GetComponent<PolygonCollider2D>().enabled = true;
            return;
        }

        int numberOfFissures = Random.Range(1, 5);

        for (int i = 0; i < numberOfFissures; i++)
        {
            Vector2 fissureDir = Utility.RandomVector2(-1f, 1f).normalized;
            LayerMask layerMask = LayerMask.GetMask("Debris");
            RaycastHit2D rayHit = Physics2D.Raycast(transform.position, fissureDir, 100f, layerMask);

            if (rayHit)
            {
                GameObject newFissure = Instantiate(GameController.FissurePrefab, transform);
                newFissure.transform.position = rayHit.point;
                Utility.LookAt2d(newFissure.transform, (Vector2)transform.position);
            }
        }

        Destroy(GetComponent<EdgeCollider2D>());
        PolygonCollider2D polygonCollider = GetComponent<PolygonCollider2D>();
        polygonCollider.enabled = true;
        polygonCollider.sharedMaterial = physicsMaterial;
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

            Vector2 randomForce = new Vector2(Random.Range(-20f, 20f), Random.Range(-20f, 20f));

            newPiece.GetComponent<Rigidbody2D>().AddForce(randomForce, ForceMode2D.Impulse);
        }

        GameObject particles = Instantiate(breakParticles, transform.position, Quaternion.identity);
        particles.transform.localScale = new Vector3(size / 10f, size / 10f, size / 10f);

        Destroy(gameObject);
    }

    public void SetSize(float newSize)
    {
        size = (float)Math.Round(newSize, 2);
        transform.localScale = new Vector2(size, size);

        if (rb == null) rb = GetComponent<Rigidbody2D>();

        rb.mass = size;
    }

    public void SetSize(float minSize, float maxSize)
    {
        size = (float)Math.Round(Random.Range(minSize, maxSize), 2);
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

    private void CreateEdgeCollider()
    {
        Destroy(GetComponent<CircleCollider2D>());

        PolygonCollider2D polygonCollider = gameObject.AddComponent<PolygonCollider2D>();

        EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();

        Vector2[] points = polygonCollider.points;
        List<Vector2> edgePoints = new List<Vector2>();

        foreach (var point in points)
        {
            edgePoints.Add(point);
        }

        edgePoints.Add(points[0]);
        edgeCollider.points = edgePoints.ToArray();

        polygonCollider.enabled = false;
    }
}
