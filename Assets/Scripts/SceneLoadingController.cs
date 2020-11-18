using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingController : MonoBehaviour
{
    public static int SceneIndexToLoad { get; set; } = 2;

    [SerializeField] private Texture2D cursorTexture;

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        StartCoroutine(LoadScene(SceneIndexToLoad));
    }

    private IEnumerator LoadScene(int sceneIndex)
    {
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingOperation.allowSceneActivation = true;

        while (!loadingOperation.isDone)
        {
            yield return null;
        }
    }
}
