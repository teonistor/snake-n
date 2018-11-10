using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSection : MonoBehaviour {

    [SerializeField] private Material m1, m2, m3;

    private int snakePartsCurrentlyAbove = 0;
    private int i, j;

    void Awake () {

    }

    public void Init(int i, int j, char d) {
        this.i = i;
        this.j = j;

        switch(d) {
            case 'o': GetComponentInChildren<Renderer>().material = m2; break;
            default : GetComponentInChildren<Renderer>().material = m1; break;
        }
    }
    
	void Start () {
		
	}
	
	void Update () {
		
	}

    internal void Enter(bool isHead=false) {
        if (isHead) {
            // TODO Perform checks for powerups, death etc;
        }
        snakePartsCurrentlyAbove++;
    }

    internal void Leave() {
        snakePartsCurrentlyAbove--;
    }
}
