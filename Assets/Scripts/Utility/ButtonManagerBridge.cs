using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManagerBridge : MonoBehaviour
{
    GameManager GM;

    void Start()
    {
        GM = GameManager.Instance;
    }

    public void GoToNextLevel() => GM?.LevelManager?.GoToNextLevel();
    public void GoToPreviousLevel() => GM?.LevelManager?.GoToPreviousLevel();
    public void GoToLevel(int level) => GM?.LevelManager?.GoToLevel(level);
}
