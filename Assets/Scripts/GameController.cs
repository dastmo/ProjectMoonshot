using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private GameObject fissurePrefab;
    [SerializeField] private CircleCollider2D repulsorCollider;

    [Header("Debris Spawn Settings")]
    [SerializeField] private float clearenceAngle = 30f;
    [SerializeField] private float endAngle = 330f;
    [SerializeField] private int smallDebrisNumber = 30;
    [SerializeField] private int mediumDebrisNumber = 15;
    [SerializeField] private int largeDebrisNumber = 7;
    [SerializeField] private float smallDebrisMinSize = 1f;
    [SerializeField] private float mediumDebrisMinSize = 10f;
    [SerializeField] private float largeDebrisMinSize = 35f;
    [SerializeField] private float debrisMaxSize = 50f;

    private CircularEdge container;
    private float playAreaRadius;

    private float maxHeight;
    private float minHeight;

    private float totalDebrisMass;
    private int totalDebrisNumber;
    private float totalDebrisCollectedMass = 0f;
    private int totalDebrisCollectedNumber = 0;

    private static GameController Instance;

    public static float MaxHeight { get => Instance.maxHeight; }
    public static float MinHeight { get => Instance.minHeight; }

    public static float TotalDebrisMass
    {
        get { return Instance.totalDebrisMass; }
        set { Instance.totalDebrisMass = value; }
    }

    public static int TotalDebrisCount
    {
        get { return Instance.totalDebrisNumber; }
        set { Instance.totalDebrisNumber = value; }
    }

    public static float TotalDebrisCollectedMass
    {
        get { return Instance.totalDebrisCollectedMass; }
        set { Instance.totalDebrisCollectedMass = value; }
    }

    public static int TotalDebrisCollectedNumber
    {
        get { return Instance.totalDebrisCollectedNumber; }
        set { Instance.totalDebrisCollectedNumber = value; }
    }

    public static GameObject FissurePrefab
    {
        get => Instance.fissurePrefab;
    }

    public BotHealth CurrentBotHealth { get; set; }

    private CinemachineVirtualCamera followCamera;

    private void Awake()
    {
        Instance = this;
        container = FindObjectOfType<CircularEdge>();
        playAreaRadius = container.Radius;
        maxHeight = playAreaRadius;
        minHeight = repulsorCollider.radius;
    }

    private void Start()
    {
        followCamera = (CinemachineVirtualCamera)Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;
        SpawnInitialDebris();

        Dustbin.DebrisCollected += OnDebrisCollected;
    }

    private void SpawnInitialDebris()
    {
        for (int i = 0; i < smallDebrisNumber; i++)
        {
            Vector2 randomDir = Utility.RandomVector2(-1f, 1f);
            if (Vector2.Angle(randomDir, Vector2.up) < clearenceAngle)
            {
                randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 0f));
            }

            float randomDistance = Random.Range(minHeight, maxHeight);

            Vector2 spawnPos = randomDir.normalized * randomDistance;

            GameObject newDebris = SpawnDebris(spawnPos);
            Debris debrisComponent = newDebris.GetComponent<Debris>();
            debrisComponent.AutoSetValues = false;
            debrisComponent.SetSize(Random.Range(smallDebrisMinSize, mediumDebrisMinSize - 0.1f));
            debrisComponent.SetInitialVelocity();
        }

        for (int i = 0; i < mediumDebrisNumber; i++)
        {
            Vector2 randomDir = Utility.RandomVector2(-1f, 1f);
            if (Vector2.Angle(randomDir, Vector2.up) < clearenceAngle)
            {
                randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 0f));
            }

            float randomDistance = Random.Range(minHeight, maxHeight);

            Vector2 spawnPos = randomDir.normalized * randomDistance;

            GameObject newDebris = SpawnDebris(spawnPos);
            Debris debrisComponent = newDebris.GetComponent<Debris>();
            debrisComponent.AutoSetValues = false;
            debrisComponent.SetSize(Random.Range(mediumDebrisMinSize, largeDebrisMinSize - 0.1f));
            debrisComponent.SetInitialVelocity();
        }

        for (int i = 0; i < largeDebrisNumber; i++)
        {
            Vector2 randomDir = Utility.RandomVector2(-1f, 1f);
            if (Vector2.Angle(randomDir, Vector2.up) < clearenceAngle)
            {
                randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 0f));
            }

            float randomDistance = Random.Range(minHeight, maxHeight);

            Vector2 spawnPos = randomDir.normalized * randomDistance;

            GameObject newDebris = SpawnDebris(spawnPos);
            Debris debrisComponent = newDebris.GetComponent<Debris>();
            debrisComponent.AutoSetValues = false;
            debrisComponent.SetSize(Random.Range(largeDebrisMinSize, debrisMaxSize));
            debrisComponent.SetInitialVelocity();
        }
    }

    public static GameObject SpawnDebris(Vector2 position)
    {
        return Instantiate(Instance.debrisPrefab, position, Quaternion.identity);
    }

    public static bool IsOutsidePlayArea(Transform checkTransform)
    {
        float absX = Mathf.Abs(checkTransform.position.x);
        float absY = Mathf.Abs(checkTransform.position.y);

        if (absX > Instance.playAreaRadius || absY > Instance.playAreaRadius)
        {
            return true;
        }

        return false;
    }

    public static void OnBotSelect(GameObject botSelected)
    {
        Instance.CurrentBotHealth = botSelected.GetComponent<BotHealth>();
        Instance.CurrentBotHealth.BotSelected();
    }

    public static void CenterCameraOnTarget(Transform newCameraTarget)
    {
        Instance.followCamera.Follow = newCameraTarget;
    }

    private void OnDestroy()
    {
        Dustbin.DebrisCollected -= OnDebrisCollected;
    }

    private void OnDebrisCollected(float debrisSize)
    {
        totalDebrisCollectedMass += debrisSize;
        totalDebrisCollectedNumber++;

        GameUIController.UpdateDebrisText(totalDebrisCollectedMass, totalDebrisCollectedNumber);
    }
}
