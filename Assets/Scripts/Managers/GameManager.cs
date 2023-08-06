using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public PlayerController Player { get; private set; } = null;

    public override void Init()
    {
        base.Init();
        TryFindPlayer();
    }

    bool TryFindPlayer()
    {
        var p = FindObjectOfType<PlayerController>();
        if (p == null) return false;

        Player = p;
        return true;
    }
}
