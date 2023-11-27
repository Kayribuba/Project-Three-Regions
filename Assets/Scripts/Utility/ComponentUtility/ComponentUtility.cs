using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComponentUtility<T> : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] internal T component;
    [Header("Config")]
    [Tooltip("Should the script try to find the component in this object")]
    [SerializeField] internal bool AutoDetectIfNull = true;

    internal bool isInitialized = false;

    private void Start()
    {
        TryInitialize();
    }

    public void TryInitialize()
    {
        if (isInitialized) return;

        if (component == null && AutoDetectIfNull)
        {
            if (TryGetComponent(out T outComponent))
            {
                component = outComponent;
            }
        }

        isInitialized = component != null;
    }
}
