using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSection : MonoBehaviour {

    [SerializeField] private float energyRegeneration;
    [SerializeField] private Material m1, m2, m3;
    [SerializeField] private Renderer tile;
    [SerializeField] private GameObject penetrableWall, impenetrableWall, oneEnergy;

    internal event Action OnHeadEnter, OnCollectCollectible;

    private int snakePartsCurrentlyAbove = 0;
    private int i, j;
    private char type;

    private GameObject item;

    void Awake () {
        OnHeadEnter = () => { };
    }

    public void Init(int i, int j, char type) {
        this.i = i;
        this.j = j;
        this.type = type;

        switch(type) {
            case 'X': item = Instantiate(impenetrableWall, transform, false); break;
            case 'x': item = Instantiate(penetrableWall, transform, false);
                      OnHeadEnter += HitPenetrableWall; break;
            //case 'o': item = Instantiate(oneEnergy, transform, false); break;
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
                World.HitSelfOrImpenetrableWall();
                return;
            }

            OnHeadEnter();
            StartCoroutine(Glow());

            //if (type == 'o') {
            //    if (item) {
            //        Destroy(item);
            //        World.CollectOneEnergy();
            //        StartCoroutine(RegenerateEnergy(energyRegeneration));
            //    }
            //}

        // Since head extends part, Enter will be called twice by the head entering, once with the flag true and once false
        } else {
            snakePartsCurrentlyAbove++;
        }
    }

    internal void Leave() {
        snakePartsCurrentlyAbove--;
    }

    internal void SpawnCollectible() { // Which collectible?
        if (item == null) {
            switch(type) {
                case 'o':
                    item = Instantiate(oneEnergy, transform, false);
                    OnHeadEnter += CollectOneEnergy;
                    break;
                default:
                    Debug.LogWarningFormat("Cannot spawn collectible for section type {0} ({1:D}, {2:D})", type, i, j);
                    break;
            }
        }
    }

    internal Action SpawnCollectible (float waitTime) {
        return () => StartCoroutine(WaitThenSpawnCollectible(waitTime));
    }

    private IEnumerator WaitThenSpawnCollectible (float waitTime) {
        yield return new WaitForSeconds(waitTime);
        SpawnCollectible();
    }

    private void HitPenetrableWall() {
        OnHeadEnter -= HitPenetrableWall;
        Destroy(item);
        item = null;
        World.HitPenetrableWall();
    }

    private void CollectOneEnergy() {
        OnHeadEnter -= CollectOneEnergy;
        Destroy(item);
        item = null;
        OnCollectCollectible();
        World.CollectOneEnergy();
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
}
