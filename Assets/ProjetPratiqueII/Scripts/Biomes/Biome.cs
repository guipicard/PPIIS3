using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Biome : ScriptableObject
{
    public string name;
    public Vector3 entrancePosition;
    public Vector3 entranceRotation;
    public Vector3 exitPosition;
    public Vector3 exitRotation;
    public Vector3 StartRoad;
    public Vector3 EndRoad;
    public bool unlocked = false;
}
