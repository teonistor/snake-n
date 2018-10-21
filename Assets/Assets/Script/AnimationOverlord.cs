using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimationOverlord : MonoBehaviour {

    [SerializeField] private AnimationClip[] clips;

    private new Animation animation;

	void Start () {
        animation = GetComponent<Animation>();
        //fore
        //animation.AddClip()
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EndOfTile() {
        //print("End of tile");
        animation.clip = clips[Random.Range(0, clips.Length)];
        animation.Play();
    }
}
