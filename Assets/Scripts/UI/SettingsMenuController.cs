using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private GameObject tutorialsResetHolder;

    [SerializeField] private List<TutorialData> tutorials;

    private void OnEnable()
    {
        musicSlider.value = AudioController.MusicVolume * 100;
        sfxSlider.value = AudioController.SFXVolume * 100;
    }

    public void MusicSliderChanged (float value)
    {
        AudioController.MusicVolume = (float)value / 100;
    }

    public void SfxSliderChanged(float value)
    {
        AudioController.SFXVolume = (float)value / 100;
    }

    public void ResetTutorials()
    {
        foreach (var item in tutorials)
        {
            PlayerPrefs.DeleteKey("tutorial_" + item.Key);
        }

        tutorialsResetHolder.SetActive(true);
    }
}
