using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string SpawnerID { get; private set; } = GLOBAL.UnnasignedString;

    int currentSceneIndex => SceneManager.GetActiveScene().buildIndex;
    bool isLoadingScene = false;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        isLoadingScene = false;

        GameManager.Instance.SceneWasLoaded(scene, loadSceneMode);
    }

    public void LoadSceneWithSpawnerID(string sceneName, string spawnerID)
    {
        if (isLoadingScene) return;

        if (GetSceneIsValid(sceneName) == false)
        { Debug.LogError("Scene by name " + sceneName + " can not be found."); return; }

        isLoadingScene = true;
        SpawnerID = spawnerID;
        SceneManager.LoadScene(sceneName);
    }

    public void GoToNextLevel()
    {
        if (isLoadingScene) return;

        if (!GetSceneIsValid(currentSceneIndex + 1))
        { Debug.LogError("Scene by build index " + (currentSceneIndex + 1) + " can not be found."); return; }

        SceneManager.LoadScene(currentSceneIndex + 1);
    }
    public void GoToPreviousLevel()
    {
        if (isLoadingScene) return;

        if (!GetSceneIsValid(currentSceneIndex - 1))
        { Debug.LogError("Scene by build index " + (currentSceneIndex - 1) + " can not be found."); return; }

        SceneManager.LoadScene(currentSceneIndex - 1);
    }
    public void GoToLevel(int index)
    {
        if (isLoadingScene) return;

        if (!GetSceneIsValid(index))
        { Debug.LogError("Scene by build index " + index + " can not be found."); return; }

        SceneManager.LoadScene(index);
    }

    bool GetSceneIsValid(int buildIndex) => -1 != SceneUtility.GetBuildIndexByScenePath(SceneUtility.GetScenePathByBuildIndex(buildIndex));
    bool GetSceneIsValid(string scenePath) => -1 != SceneUtility.GetBuildIndexByScenePath(scenePath);
}
