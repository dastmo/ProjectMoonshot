using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private CircleCollider2D repulsorCollider;
    private CircularEdge container;
    private float playAreaRadius;

    private float maxHeight;
    private float minHeight;

    private float totalDebrisMass;
    private int totalDebrisNumber;

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
}
