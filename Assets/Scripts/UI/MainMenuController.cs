using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuHolder;
    [SerializeField] private GameObject SettingsHolder;

    [Header("Settings")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button resetTutorialsButton;

    public void LoadGameScene()
    {
        SceneLoadingController.SceneIndexToLoad = 2;
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ToggleSettings(bool isOn)
    {
        if (isOn)
        {
            MainMenuHolder.SetActive(false);
            SettingsHolder.SetActive(true);
        }
        else
        {
            MainMenuHolder.SetActive(true);
            SettingsHolder.SetActive(false);
        }
    }
}
