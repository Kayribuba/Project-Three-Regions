using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionArea : MonoBehaviour
{
    public string SpawnerID => _spawnerID;

    [Header("Self")]
    [SerializeField] Transform SpawnPoint;
    [SerializeField] string _spawnerID = GLOBAL.UnnasignedString;

    [Header("Scene to Load")]
    [SerializeField] string sceneNameToLoad = GLOBAL.UnnasignedString;
    [SerializeField] string idToLoad = GLOBAL.UnnasignedString;

    LevelManager LM;
    bool isActive = false;

    void Start()
    {
        LM = GameManager.Instance?.LevelManager;
        isActive = LM != null;
    }

    public void SpawnHere(GameObject Player)
    {
        Player.transform.position = SpawnPoint.position;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive == false) return;
        if (sceneNameToLoad == GLOBAL.UnnasignedString) return;

        if (collision.tag == "Player")
        {
            LM.LoadSceneWithSpawnerID(sceneNameToLoad, idToLoad);
        }
    }
}
