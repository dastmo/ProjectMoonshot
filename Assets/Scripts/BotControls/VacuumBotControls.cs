using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumBotControls : PlayerControls
{
    [SerializeField] private float maximumBagCapacity;
    [SerializeField] private Transform bagTransform;
    [SerializeField] private Collider2D vacuumAttachmentCollider;
    [SerializeField] private AreaEffector2D vacuumEffector;

    [Header("Particles")]
    [SerializeField] private TrailRenderer[] trailRenderers;
    [SerializeField] private ParticleSystem suckParticles;
    [SerializeField] private ParticleSystem blowParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource vacuumAudioSource;

    private float currentBagMass;
    private List<float> individualDebrisWeight = new List<float>();

    private bool isSucking = false;
    private bool isBlowing = false;

    private float debrisSpitCooldown = 0.5f;

    private enum VacuumStatus
    {
        Off,
        Sucking,
        Blowing
    }

    private VacuumStatus previousFrameStatus = VacuumStatus.Off;

    public override bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            ToggleTrails(value);
        }
    }

    protected override void Start()
    {
        base.Start();
        suckParticles.Stop();
        blowParticles.Stop();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Input.GetMouseButton(0))
        {
            isSucking = true;
        }
        else if (Input.GetMouseButton(1))
        {
            isBlowing = true;
        }
        else
        {
            isSucking = false;
            isBlowing = false;
        }

        debrisSpitCooldown -= Time.fixedDeltaTime;
        ControlEffectorForce();
        ControlParticles();
        SpitOutDebris();
    }

    private void Update()
    {
        ControlVacuumSound();
    }

    private void ControlVacuumSound()
    {
        if ((isSucking || isBlowing) && !vacuumAudioSource.isPlaying)
        {
            vacuumAudioSource.volume = AudioController.SFXVolume * 0.5f;
            vacuumAudioSource.Play();
        }
        else if ((!isSucking && !isBlowing) && vacuumAudioSource.isPlaying)
        {
            vacuumAudioSource.Stop();
        }
    }

    private void ControlEffectorForce()
    {
        if (isSucking) vacuumEffector.forceMagnitude = -10f;
        else if (isBlowing) vacuumEffector.forceMagnitude = 10f;
        else vacuumEffector.forceMagnitude = 0f;
    }

    private void ControlParticles()
    {
        VacuumStatus currentStatus;

        if (isSucking) currentStatus = VacuumStatus.Sucking;
        else if (isBlowing) currentStatus = VacuumStatus.Blowing;
        else currentStatus = VacuumStatus.Off;

        if (currentStatus == previousFrameStatus) return;

        switch (currentStatus)
        {
            case VacuumStatus.Off:
                blowParticles.Stop();
                suckParticles.Stop();
                break;
            case VacuumStatus.Sucking:
                suckParticles.Play();
                blowParticles.Stop();
                break;
            case VacuumStatus.Blowing:
                blowParticles.Play();
                suckParticles.Stop();
                break;
            default:
                break;
        }

        previousFrameStatus = currentStatus;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SuckUpDebris(collision);
    }

    private void SuckUpDebris(Collision2D collision)
    {
        if (collision.otherCollider != vacuumAttachmentCollider) return;

        if (!isSucking) return;

        Debris debris = collision.gameObject.GetComponent<Debris>();

        if (debris == null) return;
        if (debris.Size > 8f) return;

        currentBagMass += debris.Size;
        individualDebrisWeight.Add(debris.Size);
        Destroy(debris.gameObject);
        ControlBagSize();

        if (currentBagMass > maximumBagCapacity) BagOverflow();
    }

    private void SpitOutDebris()
    {
        if (!isBlowing) return;
        if (individualDebrisWeight.Count == 0) return;
        if (debrisSpitCooldown > 0f) return;

        GameObject newDebris = GameController.SpawnDebris(transform.position + (transform.right * 2f));
        Debris debrisComponent = newDebris.GetComponent<Debris>();
        debrisComponent.AutoSetValues = false;
        debrisComponent.SetSize(individualDebrisWeight[0]);
        debrisComponent.ApplyForce(transform.right * 100f);
        currentBagMass -= individualDebrisWeight[0];
        individualDebrisWeight.RemoveAt(0);
        debrisSpitCooldown = 0.5f;

        ControlBagSize();
    }

    private void ControlBagSize()
    {
        float scale = currentBagMass / maximumBagCapacity;
        if (scale < 0.25f) scale = 0.25f;
        bagTransform.localScale = new Vector3(scale, scale, scale);
    }

    private void BagOverflow()
    {
        for (int i = 0; i < individualDebrisWeight.Count; i++)
        {
            GameObject newDebris = GameController.SpawnDebris(transform.position + (-transform.right * 3f));
            Debris debrisComponent = newDebris.GetComponent<Debris>();
            debrisComponent.AutoSetValues = false;
            debrisComponent.SetSize(individualDebrisWeight[i]);

            
            debrisComponent.ApplyForce((Vector2)(-transform.right * 10f) + Utility.RandomVector2(-2f, 2f));
            currentBagMass -= individualDebrisWeight[i];
        }

        individualDebrisWeight.Clear();

        ControlBagSize();
    }

    private void ToggleTrails(bool isOn)
    {
        foreach (var trail in trailRenderers)
        {
            trail.emitting = isOn;
        }
    }

    protected override void OnDestroy()
    {
        BagOverflow();
        base.OnDestroy();
    }
}
