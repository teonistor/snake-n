using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level {

    private Func<LevelSection> InstantiateLevelSection;
    private string name;
    private float timeout;
    private IList<string> levelDef;
    private LevelSection[,] level;

    public Level (string definition, Func<LevelSection> InstantiateLevelSection) {
        this.InstantiateLevelSection = InstantiateLevelSection;

        definition.Replace("\r", ""); // Protect ourselves from Windows-style line endings

        string[] defRows = definition.Split('\n');
        name = defRows[0];
        timeout = float.Parse(defRows[1]);

        // TODO order definitions? Other?
        int i = 2;
        while (defRows[i].Length > 0) i++;
        while (defRows[i].Length == 0) i++; // Reach beginning of defs

        levelDef = new List<string>();
        for (; defRows[i].Length > 0; i++) {
            levelDef.Add(defRows[i]);
        }

        level = new LevelSection[levelDef.Count, levelDef[0].Length];
        Debug.Log("Initialising level " + name + " of size " + levelDef.Count + " x " + levelDef[0].Length);

        // TODO sequence actions
        // ...
    }

    public LevelSection this[int i, int j] {
        get {
            if (level[i, j] == null) {
                level[i, j] = InstantiateLevelSection();
                level[i, j].Init(i, j, levelDef[i][j]);
            }
            return level[i, j];
        }
    }

    public int GetLength (int v) {
        return level.GetLength(v);
    }
}
