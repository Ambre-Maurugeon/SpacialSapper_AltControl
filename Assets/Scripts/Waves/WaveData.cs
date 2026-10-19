
using System;
using System.Collections.Generic;

[Serializable]
public class WaveData
{
    [Serializable]
    public struct Obstacle
        {
            public OBSTACLE type;
            public string id;
            public float time;
        }
    public string name;
    public string id;
    public List<Obstacle> obstacles = new();

}
