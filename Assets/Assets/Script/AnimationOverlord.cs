using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimationOverlord : MonoBehaviour {

    //[SerializeField] private LocalDirector world;
    [SerializeField] internal AnimationClip[] clips;

    [SerializeField] internal GameObject another;

    // TODO hack
    static int howMany = 30;
    bool couldMakeAnother = true;
    AnimationOverlord tail = null;

    public Animation Animation { get; internal set; }

    void Awake() {
        Animation = GetComponent<Animation>();
    }
    
    void Start () {
        NextTile ();
	}
	
	void Update () {}

    internal void Instruct(Transform parent, AnimationClip clip) {
        Animation.Stop();
        transform.parent = parent;
        Animation.clip = clip;
        Animation.Play();
    }

    internal virtual void NextTile () {}


    public void QuarterTile () { // TODO rename this
        if (tail == null && couldMakeAnother && howMany > 0) {
            couldMakeAnother = false; // Redundant?
            howMany--;
            tail = Instantiate(another, transform.parent).GetComponent<AnimationOverlord>();
        }

        if (tail != null) {
            tail.Instruct(transform.parent, Animation.clip);
        }
    }
}
