using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowHook : MonoBehaviour
{
    public Action<TowHook> HookDestroyed;

    private bool isAttached = false;

    public TowBotControls ParentBot { get; set; }
    public bool IsShot { get; set; } = false;

    private LineRenderer lineRenderer;

    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private float ropeSegmentLength = 0f;
    private int ropeSegmentCount = 50;

    [SerializeField] private AudioClip towHitSound;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ParentBot = GetComponentInParent<TowBotControls>();
    }

    private void FixedUpdate()
    {
        if (isAttached)
        {
            SimulateRope();
            DrawRope();
        }
    }

    private void Update()
    {
        if (!isAttached)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, ParentBot.towPoint.position);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsShot) return;

        Debris debris = collision.gameObject.GetComponent<Debris>();
        PlayerControls playerBot = collision.gameObject.GetComponent<PlayerControls>();
        Dustbin dustbin = collision.gameObject.GetComponent<Dustbin>();

        if (debris || playerBot || dustbin)
        {
            AudioSource.PlayClipAtPoint(towHitSound, transform.position, AudioController.SFXVolume);
            HandleContact(collision.gameObject, collision.contacts[0].point);
            return;
        }

        Destroy(gameObject);
    }

    private void HandleContact(GameObject objectHit, Vector2 contactPoint)
    {
        Destroy(GetComponent<Collider2D>());
        ParentBot.SetUpDistanceJoint(objectHit, contactPoint);
        GetComponent<Rigidbody2D>().isKinematic = true;
        transform.SetParent(objectHit.transform);
        Utility.LookAt2d(transform, objectHit.transform.position);
        AttachRope();

        isAttached = true;
    }

    private void AttachRope()
    {
        ropeSegmentLength = Vector2.Distance(transform.position, ParentBot.towPoint.position) / ropeSegmentCount;
        Vector3 ropeStartPoint = transform.position;
        for (int i = 0; i < ropeSegmentCount; i++)
        {
            ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegmentLength;
        }
    }

    private void DrawRope()
    {
        Vector3[] ropePositions = new Vector3[ropeSegmentCount];
        for (int i = 0; i < ropeSegmentCount; i++)
        {
            ropePositions[i] = ropeSegments[i].posNow;
        }

        ropePositions[ropeSegmentCount - 1] = ParentBot.towPoint.position;

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    private void SimulateRope()
    {
        for (int i = 1; i < ropeSegmentCount; i++)
        {
            RopeSegment firstSegment = ropeSegments[i];
            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            ropeSegments[i] = firstSegment;
        }

        for (int i = 0; i < 20; i++)
        {
            ApplyConstraints();
        }
    }

    private void ApplyConstraints()
    {
        RopeSegment firstSegment = ropeSegments[0];
        firstSegment.posNow = transform.position;
        ropeSegments[0] = firstSegment;

        RopeSegment endSegment = ropeSegments[ropeSegments.Count - 1];
        endSegment.posNow = ParentBot.towPoint.position;
        ropeSegments[ropeSegments.Count - 1] = endSegment;

        for (int i = 0; i < ropeSegmentCount - 1; i++)
        {
            RopeSegment currentSegment = ropeSegments[i];
            RopeSegment nextSegment = ropeSegments[i + 1];

            float dist = (currentSegment.posNow - nextSegment.posNow).magnitude;
            float error = Mathf.Abs(dist - ropeSegmentLength);
            Vector2 changeDir = Vector2.zero;

            if (dist > ropeSegmentLength)
            {
                changeDir = (currentSegment.posNow - nextSegment.posNow).normalized;
            }
            else if (dist < ropeSegmentLength)
            {
                changeDir = (nextSegment.posNow - currentSegment.posNow).normalized;
            }

            Vector2 changeAmount = changeDir * error;

            if (i != 0)
            {
                currentSegment.posNow -= changeAmount * 0.5f;
                ropeSegments[i] = currentSegment;
                nextSegment.posNow += changeAmount * 0.5f;
                ropeSegments[i + 1] = nextSegment;
            }
            else
            {
                nextSegment.posNow += changeAmount;
                ropeSegments[i + 1] = nextSegment;
            }
        }
    }

    private void OnDestroy()
    {
        HookDestroyed?.Invoke(this);
    }

    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            posNow = pos;
            posOld = pos;
        }
    }

}
