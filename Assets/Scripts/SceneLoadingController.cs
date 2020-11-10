using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingController : MonoBehaviour
{
    public static int SceneIndexToLoad { get; set; } = 2;

    private void Start()
    {
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
