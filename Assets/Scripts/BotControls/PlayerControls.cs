using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    protected bool _isEnabled = true;
    public virtual bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (trailRenderer) trailRenderer.emitting = value;
            _isEnabled = value;
        }
    }


    [SerializeField] private float throttleForce = 4f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private BotType botType;

    [Header("Dustbin Collision Settings")]
    [SerializeField] private GameObject[] attachments;
    [SerializeField] private int attachmentsLayerId;

    protected TrailRenderer trailRenderer;

    public BotType BotType { get => botType; }

    private float currentThrottle = 0f;

    private Rigidbody2D rb;
    private float maxHeight;
    private float minHeight;

    private Camera mainCamera;
    private BotController botController;

    private void Awake()
    {
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        botController = FindObjectOfType<BotController>();
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    protected virtual void Start()
    {
        botController.RegisterBot(this);
    }

    protected virtual void FixedUpdate()
    {
        Vector2 rotationVector = RotateAroundEarth();
        Vector2 distanceVector = ControlDistanceFromEarth();
        LookAtMouse();

        rb.velocity = (rotationVector.normalized * currentThrottle) + (distanceVector.normalized * throttleForce);
    }

    private Vector2 RotateAroundEarth()
    {
        if (!IsEnabled) return Vector2.zero;

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
            return ControlVelocityAlongArc(currentThrottle / 90f);
        }
        else
        {
            return Vector2.zero;
        }
    }

    private Vector2 ControlVelocityAlongArc(float angle)
    {
        float radius = Vector2.Distance(transform.position, Vector2.zero);
        float circ = 2 * Mathf.PI * radius;
        float arcLength = circ * angle / 360f;

        float angle2 = arcLength / radius;

        Vector2 vectorB = RotateByRadians(transform.position, angle2);

        Vector2 velocityVector = (vectorB - (Vector2)transform.position).normalized;

        if (currentThrottle > 0f) velocityVector *= -1;

        return velocityVector;
    }

    private Vector2 RotateByRadians(Vector2 vectorA, float angle)
    {
        Vector2 v = vectorA - Vector2.zero;

        float x = v.x * Mathf.Cos(angle) + v.y * Mathf.Sin(angle);
        float y = v.y * Mathf.Cos(angle) - v.x * Mathf.Sin(angle);

        Vector2 vectorB = new Vector2(x, y) + Vector2.zero;

        return vectorB;
    }

    private Vector2 ControlDistanceFromEarth()
    {
        if (!IsEnabled) return Vector2.zero;

        Vector3 velocityVector = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            velocityVector = (transform.position - Vector3.zero).normalized * throttleForce * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            velocityVector = (Vector3.zero - transform.position).normalized * throttleForce * Time.deltaTime;
        }

        if (Vector3.Distance(transform.position + velocityVector, Vector3.zero) < GameController.MaxHeight + 2f &&
            Vector3.Distance(transform.position + velocityVector, Vector3.zero) > GameController.MinHeight)
        {
            return velocityVector.normalized;
        }

        return Vector2.zero;
    }

    public void ExitDustbin()
    {
        foreach (var item in attachments)
        {
            item.layer = attachmentsLayerId;
        }
    }

    public void EnterDustbin(int layerId)
    {
        foreach (var item in attachments)
        {
            item.layer = layerId;
        }
    }

    private void LookAtMouse()
    {
        if (!IsEnabled) return;

        rb.SetRotation(Utility.GetAngle(transform.position, Utility.MouseToWorldPos(mainCamera)));
    }

    protected virtual void OnDestroy()
    {
        botController.UnregisterBot(this);
    }
}
