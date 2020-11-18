using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
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
}
