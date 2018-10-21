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
        EndOfTile();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EndOfTile() {
        //print("End of tile");
        //animation.clip = clips[Random.Range(0, clips.Length)];

        if (ahead == null) {
            GenerateMovementCodeAndNextXZ();
            transform.parent = LocalDirector.Instance.SnakeHeadEnters(nextX, nextZ);
            animation.clip = GetAppropriateClip();
            currentX = nextX;
            currentZ = nextZ;
            
        } else {
            transform.parent = ahead.transform.parent;
            animation.clip = ahead.animation.clip;
        }

        animation.Play();
    }

    private void GenerateMovementCodeAndNextXZ() {
        // TODO use input
        int exitVia = (Random.Range(1, 4) + movementCode / 10 - 1) % 4 + 1;
        movementCode += exitVia;
        switch(exitVia) {
            case 1: nextZ++; break;
            case 2: nextX++; break;
            case 3: nextZ--; break;
            case 4: nextX--; break;
            default: Debug.LogWarning("Unexpected exitVia: " + exitVia); break;
        }
    }

    private AnimationClip GetAppropriateClip() {
        return clips[0];
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
