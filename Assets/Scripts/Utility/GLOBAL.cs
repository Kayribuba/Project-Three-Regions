using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GLOBAL
{
    public static Vector2 DefaultSpawnPosition = Vector2.zero;
    public static float ProjectileSpeed = 15;
    public static string UnnasignedString = "[UNASSIGNED]";
    public static int GetLayerAt(int layerIndex) => 1 << layerIndex;
}
