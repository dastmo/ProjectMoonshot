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

    [Header("Cursor")]
    [SerializeField] private Texture2D cursorTexture;

    private void Awake()
    {
        SetCursor(cursorTexture);
    }

    private void SetCursor(Texture2D cursor)
    {
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
    }

    public void LoadGameScene()
    {
        AudioController.PlayUIClick();
        SceneLoadingController.SceneIndexToLoad = 2;
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        AudioController.PlayUIClick();
        Application.Quit();
    }

    public void ToggleSettings(bool isOn)
    {
        AudioController.PlayUIClick();
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
