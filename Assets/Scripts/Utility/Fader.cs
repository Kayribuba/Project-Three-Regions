using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Fader : MonoBehaviour
{
    [SerializeField][Min(0)] float duration = 1f;
    [SerializeField][Min(0)] int iterationsPerSecond = 10;
    [SerializeField] Image image;
    [SerializeField] GameObject[] EnableDisable;
    
    bool isInitalized;
    bool isEnabled;

    void Start()
    {
        if (image != null) isInitalized = true;
    }

    public void ToggleFade() => SetFade(!isEnabled);
    public void SetFade(bool setTo)
    {
        if (isInitalized == false) return;
        if (isOnGoing) return;
        if (setTo == enabled) return;

        StartCoroutine(FadeIEnum());
    }

    bool isOnGoing = false;
    IEnumerator FadeIEnum()
    {
        isOnGoing = true;

        Color color = image.color;
        color.a = isEnabled ? 1 : 0;

        float iterations = Mathf.FloorToInt(iterationsPerSecond * duration);
        float change = isEnabled ? 1 / (iterations) * -1 : 1 / (iterations);

        if (isEnabled)
        {
            foreach (GameObject go in EnableDisable)
            {
                go.SetActive(false);
            }
        }

        for (int i = 0; i < iterations; i++)
        {
            color.a += change;
            image.color = color;
            yield return new WaitForSeconds(1 / iterationsPerSecond);
        }

        isEnabled = !isEnabled;
        color.a = isEnabled ? 1 : 0;
        image.color = color;

        if(isEnabled)
        {
            foreach (GameObject go in EnableDisable)
            {
                go.SetActive(true);
            }
        }

        isOnGoing = false;
    }
}
