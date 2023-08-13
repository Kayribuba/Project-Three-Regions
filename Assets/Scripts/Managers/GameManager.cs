using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public LevelManager LevelManager { get { return _levelManager; } }
    [SerializeField] LevelManager _levelManager;


    public static GameObject Player { get; private set; } = null;

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
    void OnLevelWasLoaded(int level)
    {
        GatherTransitors();
    }

    private void GatherTransitors()
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

        Player.transform.position = spawnAt;
    }

    void SetPlayer()
    {
        PlayerController[] tempArray = FindObjectsOfType<PlayerController>();

        if (Player == null)
        {
            PlayerController temp = tempArray[0];
            if(temp != null)
            {
                Player = temp.gameObject;
            }
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

            DontDestroyOnLoad(Player);
        }
    }
}
