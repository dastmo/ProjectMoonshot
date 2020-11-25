using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip vacuum;
    [SerializeField] private AudioClip uiClick; //

    [Header("Source Prefab")]
    [SerializeField] private GameObject audioSourcePrefab;

    private static AudioController Instance;

    public static float MusicVolume
    {
        get
        {
            int pref = PlayerPrefs.GetInt("MusicVolume", 100);
            return (float)pref / 100;
        }

        set
        {
            PlayerPrefs.SetInt("MusicVolume", Mathf.RoundToInt(value * 100));
        }
    }

    public static float SFXVolume
    {
        get
        {
            int pref = PlayerPrefs.GetInt("SFXVolume", 100);
            return (float)pref / 100;
        }

        set
        {
            PlayerPrefs.SetInt("SFXVolume", Mathf.RoundToInt(value * 100));
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartBackgroundMusic();
    }

    private void StartBackgroundMusic()
    {
        AudioSource musicSource = Camera.main.GetComponent<AudioSource>();

        musicSource.clip = backgroundMusic;
        musicSource.volume = MusicVolume * 0.25f;

        if (MusicVolume > 0f)
        {
            musicSource.Play();
        }
    }

    public static void PlayUIClick()
    {
        AudioSource.PlayClipAtPoint(Instance.uiClick, Camera.main.transform.position, SFXVolume * 0.5f);
    }
}

public class ContinuousAudio
{
    public bool IsBackgroundMusic { get; set; } = false;
    public AudioClip AudioClip { get; set; }

    public GameObject AudioSourceObject { get; set; }

    private AudioSource source;

    public void StartAudio()
    {
        if (source == null) source = AudioSourceObject.GetComponent<AudioSource>();
        source.clip = AudioClip;
        source.loop = true;
        source.volume = IsBackgroundMusic ? AudioController.MusicVolume : AudioController.SFXVolume;
        source.Play();
    }

    public void StopAudio()
    {
        if (source == null) source = AudioSourceObject.GetComponent<AudioSource>();
        source.Stop();
    }

    private void SetSourceVolume(float volume)
    {
        if (source == null) source = AudioSourceObject.GetComponent<AudioSource>();
        source.volume = volume;
    }
}
