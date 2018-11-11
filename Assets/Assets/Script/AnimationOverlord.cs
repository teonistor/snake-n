using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimationOverlord : MonoBehaviour {
    
    [SerializeField] internal AnimationClip[] clips;

    [SerializeField] internal GameObject another;

    LevelSection currentSection;

    protected int indexInSnake;
    AnimationOverlord tail = null;

    public Animation Animation { get; internal set; }

    void Awake() {
        Animation = GetComponent<Animation>();
    }

    void Start () {
        NextTile ();
	}
	
	void Update () {
        if (indexInSnake >= World.currentEnergy && tail != null && tail.tail == null) {
            tail.currentSection.Leave();
            Destroy(tail.gameObject);
            tail = null;
        }
    }

    internal void Instruct(LevelSection section, AnimationClip clip) {
        Animation.Stop();
        if (currentSection != null)
            currentSection.Leave();

        currentSection = section;
        transform.parent = section.transform;

        currentSection.Enter();
        Animation.clip = clip;
        Animation.Play();
    }

    internal virtual void NextTile () {}


    public virtual void QuarterTile () { // TODO rename this
        if (indexInSnake < World.currentEnergy && tail == null) {
            tail = Instantiate(another, transform.parent).GetComponent<AnimationOverlord>();
            tail.indexInSnake = indexInSnake + 1;
        }

        if (tail != null) {
            tail.Instruct(currentSection, Animation.clip);
        }
    }
}
