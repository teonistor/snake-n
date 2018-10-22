using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level {

    private string name;
    private LevelSection[,] level;

    public Level (int a, int b) {
        level = new LevelSection[a, b];
    }

    public LevelSection this[int a, int b] {
        get {
            return level[a, b];
        }
        set {
            level[a, b] = value;
        }
    }

    public int GetLength (int v) {
        return level.GetLength(v);
    }
}
