using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public bool IsEnabled { get; set; } = true;

    [SerializeField] private float throttleForce = 4f;
    [SerializeField] private float maxSpeed = 10f;

    private float currentThrottle = 0f;

    private Rigidbody2D rb;
    private float maxHeight;
    private float minHeight;

    private Camera mainCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        RotateAroundEarth();
        ControlDistanceFromEarth();
        LookAtMouse();
    }

    private void RotateAroundEarth()
    {
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

        if (currentThrottle != 0f)
        {
            transform.RotateAround(Vector3.zero, Vector3.forward, currentThrottle / 90f);
        }
    }

    private void ControlDistanceFromEarth()
    {
        Vector3 delta = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            delta = transform.up * throttleForce * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            delta = -transform.up * throttleForce * Time.deltaTime;
        }

        if (Vector3.Distance(transform.position + delta, Vector3.zero) < GameController.MaxHeight &&
            Vector3.Distance(transform.position + delta, Vector3.zero) > GameController.MinHeight)
        {
            transform.position += delta;
        }
    }

    private void LookAtMouse()
    {
        Utility.LookAt2d(transform, Utility.MouseToWorldPos(mainCamera));
    }
}
