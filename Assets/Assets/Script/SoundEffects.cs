using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour {

    [SerializeField]
    AudioClip collectOneEnergy, collectSpeedUp, collectSpeedDown,
        timeApproaching, win, lose, breakWall;

    static AudioSource source;
    static SoundEffects instance;

	void Start () {
		source = GetComponent<AudioSource> ();
        instance = this;
    }

    internal static void CollectOneEnergy () {
        source.PlayOneShot(instance.collectOneEnergy);
    }

    internal static void CollectSpeedUp () {
        source.PlayOneShot(instance.collectSpeedUp);
    }

    internal static void CollectSpeedDown () {
        source.PlayOneShot(instance.collectSpeedDown);
    }

    internal static void TimeApproaching () {
        source.clip = instance.timeApproaching;
        source.Play();
        //source.PlayOneShot(instance.timeApproaching);
    }

    internal static void Win () {
        source.PlayOneShot(instance.win);
    }

    internal static void Lose () {
        source.PlayOneShot(instance.lose);
    }

    internal static void BreakWall () {
        source.PlayOneShot(instance.breakWall);
    }
}
