using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimationOverlord : MonoBehaviour {

    //[SerializeField] private LocalDirector world;
    [SerializeField] private AnimationClip[] clips;

    [SerializeField] private GameObject another;

    // TODO hack
    static int howMany = 10;
    bool couldMakeAnother = true;
    AnimationOverlord ahead = null;

    public new Animation animation { get; private set; }

    // Movement
    public int currentX { get; private set; }
    public int currentZ { get; private set; }
    public int nextX { get; private set; }
    public int nextZ { get; private set; }


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
        animation = GetComponent<Animation>();
        //fore
        //animation.AddClip()

        movementCode = 30; // Arbitrary?
        nextZ = 1; // As above
        EndOfTile();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EndOfTile() {
        //print("End of tile");
        //animation.clip = clips[Random.Range(0, clips.Length)];

        if (ahead == null) {
            currentX = nextX;
            currentZ = nextZ;
            GenerateMovementCodeAndNextXZ();
            transform.parent = LocalDirector.Instance.SnakeHeadEnters(currentX, currentZ);
            animation.clip = GetAppropriateClip();
            currentX = nextX;
            currentZ = nextZ;
            movementCode = ((movementCode % 10 + 1) % 4 + 1) * 10;
        } else {
            transform.parent = ahead.transform.parent;
            animation.clip = ahead.animation.clip;
        }

        animation.Play();
    }

    private void GenerateMovementCodeAndNextXZ() {
        // TODO use input
        sthEnum.MoveNext();
        int exitVia = sthEnum.Current;// (Random.Range(1, 4) + movementCode / 10 - 1) % 4 + 1;

        movementCode += exitVia;
        switch(exitVia) {
            case 1: nextZ++; break;
            case 2: nextX++; break;
            case 3: nextZ--; break;
            case 4: nextX--; break;
            default: Debug.LogWarning("Unexpected exitVia: " + exitVia); break;
        }
    }

    IEnumerator<int> sthEnum = sth();

    private static IEnumerator<int> sth() {
        yield return 1;
        yield return 1;
        yield return 1;
        yield return 2;
        yield return 2;
        yield return 2;
        yield return 2;
        yield return 3;
        yield return 3;
        yield return 3;
        yield return 2;
        yield return 2;
        yield return 2;
        yield return 3;
        yield return 3;
        yield return 3;
        while (true) yield return 4;
    }

    private AnimationClip GetAppropriateClip() {
        //print("Movement code " + movementCode);
        switch(movementCode) {
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

    public void CouldMakeAnother () {
        if (couldMakeAnother && howMany > 0) {
            couldMakeAnother = false;
            howMany--;
            AnimationOverlord anotherInstance = Instantiate(another, transform.parent).GetComponent<AnimationOverlord>();
            anotherInstance.ahead = this;
        }
    }
}
