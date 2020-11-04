using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
    public float maxSize { get; set; } = 50f;
    public float minSize { get; set; } = 10f;

    private Rigidbody2D rb;

    private float size;

    public float Size { get => size; }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetInitialSize();
        rb.velocity = SetInitialVelocity();

        GameController.TotalDebrisCount += 1;
        GameController.TotalDebrisMass += rb.mass;
    }

    private void FixedUpdate()
    {
        if (GameController.IsOutsidePlayArea(transform)) Destroy(gameObject);
    }

    private Vector2 SetInitialVelocity()
    {
        float x = Random.Range(-5f, 5f);
        float y = Random.Range(-5f, 5f);

        return new Vector2(x, y);
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

    private void OnDestroy()
    {
        GameController.TotalDebrisCount -= 1;
        GameController.TotalDebrisMass -= rb.mass;
    }
}
