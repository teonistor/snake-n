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

    void Start () {
        animation = GetComponent<Animation>();
        //fore
        //animation.AddClip()

        EndOfTile();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EndOfTile() {
        //print("End of tile");
        //animation.clip = clips[Random.Range(0, clips.Length)];

        if (ahead == null) {
            transform.parent = LocalDirector.Instance.SnakeHeadEnters(nextX, nextZ);
            currentX = nextX;
            currentZ = nextZ;
            nextZ++;
        } else {
            transform.parent = ahead.transform.parent;
            animation.clip = ahead.animation.clip;
        }

        animation.Play();
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
