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
    private LevelTile[,] level;

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

        level = new LevelTile[levelDef.Count, levelDef[0].Length];
        Debug.Log("Initialising level " + Name + " of size " + levelDef.Count + " x " + levelDef[0].Length);

        while (defRows[i].Length == 0) i++; // Reach beginning of opening moves
        string[] openingMovesStr = defRows[i].Split(OneEmpty, StringSplitOptions.RemoveEmptyEntries);
        OpeningMoves = new int[openingMovesStr.Length];
        for (int j = 0; j < OpeningMoves.Length; j++) {
            OpeningMoves[j] = int.Parse(openingMovesStr[j]);
        }
        i++;


        while (defRows[i].Length == 0) i++; // Reach beginning of collectible chain defs
        for (; defRows[i].Length > 0; i++) {
            string[] chain = defRows[i].Split(OneEmpty, StringSplitOptions.RemoveEmptyEntries);

            // A chain is defined
            if (chain[0].Contains(",")) {
                for (int j=0; j<chain.Length; j++) {
                    this[chain[j]].OnCollectCollectible += this[chain[(j + 1) % chain.Length]].SpawnCollectible;
                }
                this[chain[0]].SpawnCollectible();

            // A timer is defined
            } else {
                float waitTime = float.Parse(chain[0]);
                for (int j = 1; j < chain.Length; j++) {
                    this[chain[j]].OnCollectCollectible += this[chain[j]].SpawnCollectible(waitTime);
                    this[chain[j]].SpawnCollectible();
                }
            }
        }
    }

    public LevelTile this[int i, int j] {
        get {
            if (level[i, j] == null) {
                level[i, j] = World.InstantiateLevelSection();
                level[i, j].Init(i, j, levelDef[i][j]);
            }
            return level[i, j];
        }
    }

    private LevelTile this[string ij] { get {
        string[] s = ij.Split(',');
        return this[int.Parse(s[0]), int.Parse(s[1])];
    }}

    public int GetLength (int v) {
        return level.GetLength(v);
    }
}
