using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualManager : MonoBehaviour
{
    [SerializeField] Fader GameOverFader;

    public void SetGameOver(bool setTo)
    {
        GameOverFader.SetFade(setTo);
    }
}
