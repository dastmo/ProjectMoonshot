using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

    [Header("Game Settings")]
    [SerializeField] private int gameTimeMinutes = 30;

    private int gameTimeRemainingSeconds = 0;

    private CircularEdge container;
    private float playAreaRadius;

    private float maxHeight;
    private float minHeight;

    private float totalDebrisMass;
    private int totalDebrisNumber;
    private float totalDebrisCollectedMass = 0f;
    private int totalDebrisCollectedNumber = 0;
    private float materialsAvailable = 0f;

    private bool inititalDebrisSpawned = false;
    private bool timerStarted = false;
    private bool gameStarted { get { return inititalDebrisSpawned && timerStarted; } }

    private bool movementTutorialShown = false;

    private Dustbin dustbin;

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

    public static float MaterialsAvailable
    {
        get { return Instance.materialsAvailable; }
        set
        { 
            Instance.materialsAvailable = value;
            Instance.materialsAvailable = (float)Math.Round(Instance.materialsAvailable, 2);
        }
    }

    public static float SmallDebrisMinSize
    {
        get => Instance.smallDebrisMinSize;
    }

    public static float MediumDebrisMinSize
    {
        get => Instance.mediumDebrisMinSize;
    }

    public static float LargeDebrisMinSize
    {
        get => Instance.largeDebrisMinSize;
    }

    public static float DebrisMaxSize
    {
        get => Instance.debrisMaxSize;
    }

    public static GameObject FissurePrefab
    {
        get => Instance.fissurePrefab;
    }

    public static Vector2 DustbinPosition
    {
        get => Instance.dustbin.transform.position;
    }

    public static Quaternion DustbinRotation
    {
        get => Instance.dustbin.transform.rotation;
    }

    public BotHealth CurrentBotHealth { get; set; }

    private CinemachineVirtualCamera followCamera;

    public static Action<float> MaterialsAvailableChanged;

    private void Awake()
    {
        Instance = this;
        container = FindObjectOfType<CircularEdge>();
        playAreaRadius = container.Radius;
        maxHeight = playAreaRadius;
        minHeight = repulsorCollider.radius;

        dustbin = FindObjectOfType<Dustbin>();
        followCamera = (CinemachineVirtualCamera)Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;

        gameTimeRemainingSeconds = gameTimeMinutes * 60;

        TutorialController.TutorialClosed += OnTutorialClosed;
    }

    private void Update()
    {
        if (gameStarted && !movementTutorialShown)
        {
            TutorialController.ShowTutorial("Movement");
            movementTutorialShown = true;
            
        }

        if (TotalDebrisCount <= 0 && gameStarted)
        {
            GameUIController.ShowGameOverPanel(false, 1f);
        }
        else if (gameTimeRemainingSeconds <= 0f && gameStarted)
        {
            float percentage = totalDebrisCollectedMass / (totalDebrisCollectedMass + totalDebrisMass);
            GameUIController.ShowGameOverPanel(true, percentage);
        }
    }

    private void OnTutorialClosed(string key)
    {
        if (key == "Movement")
        {
            TutorialController.ShowTutorial("Dustbin");
        }
    }

    private void Start()
    {
        SpawnInitialDebris();
        
        Dustbin.DebrisCollected += OnDebrisCollected;

        StartCoroutine(TimerCoroutine());
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

        inititalDebrisSpawned = true;
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
        GameUIController.UpdateDebrisText(TotalDebrisCollectedMass, TotalDebrisCollectedNumber, MaterialsAvailable);
    }

    public static void CenterCameraOnTarget(Transform newCameraTarget)
    {
        if (Instance.followCamera == null)
        {
            Instance.followCamera = (CinemachineVirtualCamera)Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;
        }

        Instance.followCamera.Follow = newCameraTarget;
    }

    private void OnDestroy()
    {
        Dustbin.DebrisCollected -= OnDebrisCollected;
        TutorialController.TutorialClosed -= OnTutorialClosed;
    }

    private void OnDebrisCollected(float debrisSize)
    {
        totalDebrisCollectedMass += debrisSize;
        materialsAvailable += (float)Math.Round(debrisSize, 2);
        MaterialsAvailableChanged?.Invoke(materialsAvailable);
        totalDebrisCollectedNumber++;

        GameUIController.UpdateDebrisText(totalDebrisCollectedMass, totalDebrisCollectedNumber, materialsAvailable);
    }

    private IEnumerator TimerCoroutine()
    {
        timerStarted = true;

        while (gameTimeRemainingSeconds >= 0)
        {
            GameUIController.UpdateTimerText(gameTimeRemainingSeconds);
            yield return new WaitForSeconds(1f);
            gameTimeRemainingSeconds--;
        }
    }
}
