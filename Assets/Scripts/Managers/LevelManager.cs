using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string SpawnerID { get; private set; } = GLOBAL.UnnasignedString;

    int currentSceneIndex => SceneManager.GetActiveScene().buildIndex;

    public void LoadSceneWithSpawnerID(string sceneName, string spawnerID)
    {
        if (GetSceneIsValid(sceneName) == false)
        { Debug.LogError("Scene by name " + sceneName + " can not be found."); return; }

        SpawnerID = spawnerID;
        SceneManager.LoadScene(sceneName);
    }

    public void GoToNextLevel()
    {
        if (!GetSceneIsValid(currentSceneIndex + 1))
        { Debug.LogError("Scene by build index " + (currentSceneIndex + 1) + " can not be found."); return; }

        SceneManager.LoadScene(currentSceneIndex + 1);
    }
    public void GoToPreviousLevel()
    {
        if (!GetSceneIsValid(currentSceneIndex - 1))
        { Debug.LogError("Scene by build index " + (currentSceneIndex - 1) + " can not be found."); return; }

        SceneManager.LoadScene(currentSceneIndex - 1);
    }
    public void GoToLevel(int index)
    {
        if (!GetSceneIsValid(index))
        { Debug.LogError("Scene by build index " + index + " can not be found."); return; }

        SceneManager.LoadScene(index);
    }

    bool GetSceneIsValid(int buildIndex) => -1 != SceneUtility.GetBuildIndexByScenePath(SceneUtility.GetScenePathByBuildIndex(buildIndex));
    bool GetSceneIsValid(string scenePath) => -1 != SceneUtility.GetBuildIndexByScenePath(scenePath);
}
