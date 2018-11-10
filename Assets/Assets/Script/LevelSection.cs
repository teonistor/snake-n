using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSection : MonoBehaviour {

    [SerializeField] private Material m1, m2, m3;
    [SerializeField] private Renderer tile;

    private int snakePartsCurrentlyAbove = 0;
    private int i, j;

    void Awake () {

    }

    public void Init(int i, int j, char d) {
        this.i = i;
        this.j = j;

        switch(d) {
            case 'o': tile.material = m2; break;
            default : tile.material = m1; break;
        }
    }
    
	void Start () {
		
	}
	
	void Update () {
		
	}

    internal void Enter (bool isHead = false) {
        if (isHead) {
            StartCoroutine(Glow());
            // TODO Perform checks for powerups, death etc;
        } else {
            snakePartsCurrentlyAbove++;
        }
    }

    internal void Leave() {
        snakePartsCurrentlyAbove--;
    }

    private IEnumerator Glow() {
        print("Spark started");
        Material m = tile.material;
        Color end = m.color;
        Color start = Color.Lerp(end, Color.white, 0.8f);
        WaitForSeconds wait = new WaitForSeconds(1f / 30);
        for (float t=0f; t<1f; t += 1f / 60) {
            m.color = Color.Lerp(start, end, t);
            yield return wait;
        }
        m.color = end;
    }
}
