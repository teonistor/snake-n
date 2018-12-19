using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientLight : MonoBehaviour {

    internal static readonly Color skyNormal = new Color(0.8f, 0.8f, 0.8f);
    internal static readonly Color skyNoControl = new Color(0.06574395f, 0.2440461f, 0.2794118f);
    internal static readonly Color skyTimeUp = new Color(0.4f, 0.2f, 0.3f);
    internal static readonly Color skyLevelComplete = new Color(0.25f, 0.5f, 0.25f);

    private static AmbientLight instance;
    private Coroutine mutex;

    void Awake () {
        instance = this;
	}
	
	void Refresh () {
        StopCoroutine(mutex);
	}
}
