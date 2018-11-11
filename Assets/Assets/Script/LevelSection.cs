using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSection : MonoBehaviour {

    [SerializeField] private float energyRegeneration;
    [SerializeField] private Material m1, m2, m3;
    [SerializeField] private Renderer tile;
    [SerializeField] private GameObject penetrableWall, impenetrableWall, oneEnergy;

    private int snakePartsCurrentlyAbove = 0;
    private int i, j;
    private char type;

    private GameObject item;

    void Awake () {

    }

    public void Init(int i, int j, char type) {
        this.i = i;
        this.j = j;
        this.type = type;

        switch(type) {
            case 'X': item = Instantiate(impenetrableWall, transform, false); break;
            case 'x': item = Instantiate(penetrableWall, transform, false); break;
            case 'o': item = Instantiate(oneEnergy, transform, false); break;
                //default : tile.material = m1; break;
        }
    }
    
	void Start () {
		
	}
	
	void Update () {
		
	}

    internal void Enter (bool isHead = false) {
        if (isHead) {
            if (type=='X' || snakePartsCurrentlyAbove>0) {
                World.Die();
                return;
            }
            if (type == 'x') {
                if (item) {
                    Destroy(item);
                    World.HitPenetrableWall();
                }
            }
            StartCoroutine(Glow());

            if (type == 'o') {
                if (item) {
                    Destroy(item);
                    World.CollectOneEnergy();
                    StartCoroutine(RegenerateEnergy(energyRegeneration));
                }
            }
        } else {
            snakePartsCurrentlyAbove++;
        }
    }

    internal void Leave() {
        snakePartsCurrentlyAbove--;
    }

    private IEnumerator Glow() {
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

    private IEnumerator RegenerateEnergy(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        item = Instantiate(oneEnergy, transform, false);
    }
}
