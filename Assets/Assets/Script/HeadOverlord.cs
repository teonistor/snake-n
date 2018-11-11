﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadOverlord : AnimationOverlord {

    // Movement
    public int currentX { get; private set; }
    public int currentZ { get; private set; }
    public int nextX { get; private set; }
    public int nextZ { get; private set; }

    // Controls
    private int LeftTurn {
        get {
            if (Input.GetKey(KeyCode.A)) return -1;
            Rect r = new Rect(0, 0, Screen.width / 2, Screen.height);
            for (var i = 0; i < Input.touchCount; i++) {
                if (r.Contains(Input.GetTouch(i).position))
                    return -1;
            }
            return 0;
        }
    }
    private int RightTurn {
        get {
            if (Input.GetKey(KeyCode.D)) return 1;
            Rect r = new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height);
            for (var i = 0; i < Input.touchCount; i++) {
                if (r.Contains(Input.GetTouch(i).position))
                    return 1;
            }
            return 0;
        }
    }


    /******
     * Tens digit represents were we're entering
     * Units digit represents were we're headed
     * 1 = North
     * 2 = East
     * 3 = South
     * 4 = West
     */
    private int movementCode;

    void Start () {
        //Animation = GetComponent<Animation>();
        movementCode = 30; // Arbitrary?
        nextZ = 1; // As above
        NextTile ();
    }

    internal override void NextTile () {
        currentX = nextX;
        currentZ = nextZ;
        GenerateMovementCodeAndNextXZ();

        LevelSection section = World.SnakeHeadEnters(currentX, currentZ);
        section.Enter(true);
        Instruct(section, GetAppropriateClip());

        currentX = nextX;
        currentZ = nextZ;
        movementCode = ((movementCode % 10 + 1) % 4 + 1) * 10;
    }

    private AnimationClip GetAppropriateClip () {
        //print("Movement code " + movementCode);
        switch (movementCode) {
            case 21: return clips[0];
            case 23: return clips[1];
            case 24: return clips[2];
            case 12: return clips[3];
            case 13: return clips[4];
            case 14: return clips[5];
            case 32: return clips[6];
            case 31: return clips[7];
            case 34: return clips[8];
            case 42: return clips[9];
            case 41: return clips[10];
            case 43: return clips[11];
            default: Debug.LogWarning("Unexpected movementCode: " + movementCode); return clips[0];
        }
    }

    private void GenerateMovementCodeAndNextXZ () {
        // TODO pre-/post-level action
        //sthEnum.MoveNext();
        //int exitVia = sthEnum.Current;// (Random.Range(1, 4) + movementCode / 10 - 1) % 4 + 1;
        int exitVia = (movementCode / 10 + 1 + LeftTurn + RightTurn) % 4 + 1;

        movementCode += exitVia;
        switch (exitVia) {
            case 1: nextZ++; break;
            case 2: nextX++; break;
            case 3: nextZ--; break;
            case 4: nextX--; break;
            default: Debug.LogWarning("Unexpected exitVia: " + exitVia); break;
        }
    }

    //public override void QuarterTile () {
    //    base.QuarterTile();
    //}

    //IEnumerator<int> sthEnum = sth();
    //private static IEnumerator<int> sth () {
    //    yield return 1;
    //    yield return 1;
    //    yield return 1;
    //    yield return 2;
    //    yield return 2;
    //    yield return 2;
    //    yield return 2;
    //    yield return 3;
    //    yield return 3;
    //    yield return 3;
    //    yield return 2;
    //    yield return 2;
    //    yield return 2;
    //    yield return 3;
    //    yield return 3;
    //    yield return 3;
    //    while (true) yield return 4;
    //}
}
