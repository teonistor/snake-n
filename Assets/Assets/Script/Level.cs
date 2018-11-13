using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level {
    internal static readonly char[] OneEmpty = { ' ' };

    internal string Name { get; private set; }
    internal float Timeout { get; private set; }
    internal int RequiredPoints { get; private set; }
    internal int[] OpeningMoves { get; private set; }

    private IList<string> levelDef;
    private LevelSection[,] level;

    public Level (string definition) {
        definition.Replace("\r", ""); // Protect ourselves from Windows-style line endings

        string[] defRows = definition.Split('\n');
        Name = defRows[0];
        Timeout = float.Parse(defRows[1]);
        RequiredPoints = int.Parse(defRows[2]);

        // TODO order definitions? Other?
        int i = 3;
        while (defRows[i].Length > 0) i++; // Forwards compatibility 
        while (defRows[i].Length == 0) i++; // Reach beginning of defs

        levelDef = new List<string>();
        for (; defRows[i].Length > 0; i++) {
            levelDef.Add(defRows[i]);
        }

        level = new LevelSection[levelDef.Count, levelDef[0].Length];
        Debug.Log("Initialising level " + Name + " of size " + levelDef.Count + " x " + levelDef[0].Length);

        while (defRows[i].Length == 0) i++; // Reach beginning of opening moves
        string[] openingMovesStr = defRows[i].Split(OneEmpty, StringSplitOptions.RemoveEmptyEntries);
        OpeningMoves = new int[openingMovesStr.Length];
        for (int j = 0; j < OpeningMoves.Length; j++) {
            OpeningMoves[j] = int.Parse(openingMovesStr[j]);
        }
    }

    public LevelSection this[int i, int j] {
        get {
            if (level[i, j] == null) {
                level[i, j] = World.InstantiateLevelSection();
                level[i, j].Init(i, j, levelDef[i][j]);
            }
            return level[i, j];
        }
    }

    public int GetLength (int v) {
        return level.GetLength(v);
    }
}
