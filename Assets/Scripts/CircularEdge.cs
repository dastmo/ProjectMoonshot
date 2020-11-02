using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularEdge : MonoBehaviour
{
    [SerializeField] private int resolution;
    [SerializeField] private float radius;

    public float Radius { get { return radius; } }

    private void Start()
    {
        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
        Vector2[] points = new Vector2[resolution + 1];

        for (int i = 0; i < resolution; i++)
        {
            float angle = 2 * Mathf.PI * i / resolution;
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            points[i] = new Vector2(x, y);
        }

        points[resolution] = points[0];
        edgeCollider.points = points;
    }
}
