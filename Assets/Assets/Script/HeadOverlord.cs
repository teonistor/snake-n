using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadOverlord : AnimationOverlord {

    // Movement
    public int currentX { get; private set; }
    public int currentZ { get; private set; }
    public int nextX { get; private set; }
    public int nextZ { get; private set; }

    private IEnumerator<int> ongoingOpeningMoves = null;

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
     * Tens digit represents were we're entering from
     * Units digit represents were we're headed
     * 1 = North
     * 2 = East
     * 3 = South
     * 4 = West
     */
    private int movementCode;

    void Start () {
        indexInSnake = 0;
        //Animation = GetComponent<Animation>();
        movementCode = 30; // Arbitrary?
        nextZ = 1; // As above
        NextTile ();
    }

    internal override void NextTile () {
        currentX = nextX;
        currentZ = nextZ;
        GenerateMovementCodeAndNextXZ();

        LevelSection section = World.SnakeHeadEnters(currentX, currentZ, movementCode);
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
        int anyTurn = 0;

        if (World.GameState == GameState.Prologue) {
            if (ongoingOpeningMoves == null)
                ongoingOpeningMoves = OpeningMoves(World.CurrentLevelOpeningMoves);

            if (ongoingOpeningMoves.MoveNext())
                anyTurn = ongoingOpeningMoves.Current;
            else
                World.OpeningMovesFinished();
        }
        if (World.GameState == GameState.Playing) {
            anyTurn = LeftTurn + RightTurn;
        }
        if (World.GameState == GameState.GameOver) {
            anyTurn = Random.Range(-1, 2);
        }

        int exitVia = (movementCode / 10 + 1 + anyTurn) % 4 + 1;

        movementCode += exitVia;
        switch (exitVia) {
            case 1: nextZ++; break;
            case 2: nextX++; break;
            case 3: nextZ--; break;
            case 4: nextX--; break;
            default: Debug.LogWarning("Unexpected exitVia: " + exitVia); break;
        }
    }

    private static IEnumerator<int> OpeningMoves (int[] openingMoves) {
        foreach (int move in openingMoves)
            yield return move;
    }
}
