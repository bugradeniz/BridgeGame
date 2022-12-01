using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    // Level yuklemek icin kullanilan sinif .
    // ana Game sahnesinde calisiyor ve ana sahneyi kapatmadan levelleri 2. sahne olarak kapatip aciyor.
    // Game sahnesi herzaman acik oluyor

    public static LevelLoader Current;
    private Scene _lastLoadedScene;

    void Start()
    {
        Current = this;
        changeLevel("Level " + PlayerPrefs.GetInt("currentLevel"));

    }


    public void changeLevel(string sceneName)
    {
        StartCoroutine(ChangeScene(sceneName));

    }
    IEnumerator ChangeScene(string sceneName)
    {

        if (_lastLoadedScene.IsValid())
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(_lastLoadedScene);
            bool sceneUnloaded = false;
            while (!sceneUnloaded)
            {
                sceneUnloaded = unloadOperation.isDone;
                yield return new WaitForEndOfFrame();
            }
        }
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        bool sceneLoaded = false;
        while (!sceneLoaded)
        {

            _lastLoadedScene = SceneManager.GetSceneByName(sceneName);
            sceneLoaded = _lastLoadedScene != null && loadOperation.isDone;
            yield return new WaitForEndOfFrame();
        }
    }

}
