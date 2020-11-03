using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public bool IsEnabled { get; set; } = true;


    [SerializeField] private float throttleForce = 4f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private BotType botType;

    public BotType BotType { get => botType; }

    private float currentThrottle = 0f;

    private Rigidbody2D rb;
    private float maxHeight;
    private float minHeight;

    private Camera mainCamera;
    private BotController botController;

    private void Awake()
    {
        botController = FindObjectOfType<BotController>();
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        botController.RegisterBot(this);
    }

    private void FixedUpdate()
    {
        RotateAroundEarth();
        ControlDistanceFromEarth();
        LookAtMouse();
    }

    private void RotateAroundEarth()
    {
        if (!IsEnabled) return;

        if (Input.GetKey(KeyCode.A))
        {
            currentThrottle += throttleForce * Time.deltaTime;
            if (currentThrottle > maxSpeed) currentThrottle = maxSpeed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            currentThrottle -= throttleForce * Time.deltaTime;
            if (currentThrottle < -maxSpeed) currentThrottle = -maxSpeed;
        }
        else
        {
            currentThrottle = Mathf.Lerp(currentThrottle, 0f, Time.deltaTime * throttleForce);
        }

        if (currentThrottle != 0)
        {
            ControlVelocityAlongArc(currentThrottle / 90f);
        }
    }

    private void ControlVelocityAlongArc(float angle)
    {
        float radius = Vector2.Distance(transform.position, Vector2.zero);
        float circ = 2 * Mathf.PI * radius;
        float arcLength = circ * angle / 360f;

        float angle2 = arcLength / radius;

        Vector2 vectorB = RotateByRadians(transform.position, angle2);

        Vector2 velocityVector = (vectorB - (Vector2)transform.position).normalized;

        if (currentThrottle > 0f) velocityVector *= -1;

        rb.velocity = velocityVector * currentThrottle;
    }

    private Vector2 RotateByRadians(Vector2 vectorA, float angle)
    {
        Vector2 v = vectorA - Vector2.zero;

        float x = v.x * Mathf.Cos(angle) + v.y * Mathf.Sin(angle);
        float y = v.y * Mathf.Cos(angle) - v.x * Mathf.Sin(angle);

        Vector2 vectorB = new Vector2(x, y) + Vector2.zero;

        return vectorB;
    }

    private void ControlDistanceFromEarth()
    {
        if (!IsEnabled) return;

        Vector3 delta = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            delta = (transform.position - Vector3.zero).normalized * throttleForce * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            delta = (Vector3.zero - transform.position).normalized * throttleForce * Time.deltaTime;
        }

        if (Vector3.Distance(transform.position + delta, Vector3.zero) < GameController.MaxHeight &&
            Vector3.Distance(transform.position + delta, Vector3.zero) > GameController.MinHeight)
        {
            transform.position += delta;
        }
    }

    private void LookAtMouse()
    {
        if (!IsEnabled) return;

        rb.SetRotation(Utility.GetAngle(transform.position, Utility.MouseToWorldPos(mainCamera)));
    }
}
