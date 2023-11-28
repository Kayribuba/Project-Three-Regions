using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public struct BulletGroup
{
    public Bullet[] Bullets;
    public float Cooldown;
}
[System.Serializable] public struct Bullet
{
    public GameObject Projectile;
    [Tooltip("How many degrees will the projectile deviate from its direction anti-clockwise")]
    public float TurnDegree;
    public Vector2 Offset;
    public float Speed;
}

public abstract class GLOBAL
{
    public static string[] EnemyIgnoreTags = new string[] { "Enemy" };
    public static string[] PlayerIgnoreTags = new string[] { "Player" };
    public static Vector2 DefaultSpawnPosition = Vector2.zero;
    public static float ProjectileSpeed = 15;
    public static string UnnasignedString = "[UNASSIGNED]";
    public static int GetLayerAt(int layerIndex) => 1 << layerIndex;
}
