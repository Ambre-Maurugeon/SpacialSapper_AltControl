using System;
using UnityEngine;

[Serializable]
public class MeteoriteData
{
    [Header("Info")]
    public string name;
    public string id;

    [Space(5)]
    public GameObject prefab;

    [Header("Values")]
    public float speed;
    public int flames;
    public float radius;



}
