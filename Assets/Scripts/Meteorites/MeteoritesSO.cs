using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeteoritesSO", menuName = "Scriptable Objects/MeteoritesSO")]
public class MeteoritesSO : ScriptableObject
{
    public List<MeteoriteData> meteorites = new();

    public MeteoriteData GetData(string id){
        foreach (var meteor in meteorites)
        {
            if(meteor.id == id) return meteor;
        }

        throw new ArgumentException(
            "Id doesn't exist in meteorites SO : " + id);
    }
}
