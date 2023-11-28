using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public LevelManager LevelManager { get { return _levelManager; } }
    [SerializeField] LevelManager _levelManager;

    public GameObject Player { get; private set; } = null;
    public bool GameIsOver { get; private set; }

    bool justInitalized = true;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SetPlayer();
    }
    public void SceneWasLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(justInitalized)
        {
            justInitalized = false;
            return;
        }

        GatherTransitors();
        SetPlayer();
    }
    public void EndGameInSeconds(float seconds)
    {
        Invoke(nameof(EndGame), seconds);
    }
    public void EndGame()
    {
        if (GameIsOver) return;

        GameIsOver = true;
    }
    void GatherTransitors()
    {
        if (LevelManager == null) return;
        string spawnerID = LevelManager.SpawnerID;

        if (spawnerID != GLOBAL.UnnasignedString)
        {
            LevelTransitionArea[] levelTransitors = FindObjectsOfType<LevelTransitionArea>();
            LevelTransitionArea wantedTransitor = null;

            foreach (LevelTransitionArea LTA in levelTransitors)
            {
                if (LTA.SpawnerID == spawnerID)
                {
                    wantedTransitor = LTA;
                    break;
                }
            }

            if (wantedTransitor != null)
            {
                wantedTransitor.SpawnHere(Player);
            }
            else
            {
                SpawnAtDefault();
            }
        }
        else
        {
            SpawnAtDefault();
        }
    }

    void SpawnAtDefault()
    {
        GameObject go = GameObject.FindGameObjectWithTag("DefaultSpawnPosition");
        Vector2 spawnAt = go != null ? (Vector2)go.transform.position : GLOBAL.DefaultSpawnPosition;

        if (Player.TryGetComponent(out PlayerController pc))
        {
            pc.SpawnPlayer(spawnAt);
        }
        else
        {
            Player.transform.position = spawnAt;
        }
    }

    void SetPlayer()
    {
        Player = null;
        PlayerController[] tempArray = FindObjectsOfType<PlayerController>();

        if (tempArray[0] != null)
        {
            foreach (PlayerController temp in tempArray)
            {
                if (temp.IsOld)
                {
                    Player = temp.gameObject;
                    break;
                }
            }

            if (Player == null) Player = tempArray[0].gameObject;
        }

        if (Player != null)
        {
            foreach (PlayerController pc in tempArray)
            {
                if (pc.gameObject != Player)
                {
                    Destroy(pc.gameObject);
                }
            }

            if(Player.TryGetComponent(out PlayerController pcont))
            {
                if(pcont.IsOld == false) DontDestroyOnLoad(Player);
                pcont.PlayerIsSelected();
            }
            
        }
    }
}
