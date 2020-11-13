using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    private static string tutorialPrefsKey = "tutorialEnabled";
    public static bool TutorialEnabled
    {
        get
        {
            return Utility.GetPlayerPrefsBool(tutorialPrefsKey, true);
        }

        set
        {
            Utility.SetPlayerPrefsBool(tutorialPrefsKey, value);
        }
    }

    public static float MusicVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("musicVolume", 1f);
        }

        set
        {
            PlayerPrefs.SetFloat("musicVolume", value);
        }
    }
}
