using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour {

    [SerializeField] private float energyRegeneration;
    [SerializeField] private Material m1, m2, m3;
    [SerializeField] private Renderer tile;
    [SerializeField] private GameObject penetrableWall, impenetrableWall, oneEnergy, speedUp, speedDown;

    internal event Action OnHeadEnter, OnCollectCollectible;

    private int snakePartsCurrentlyAbove = 0;
    private int i, j;
    private char type;
    private Color normal;
    private Material material;

    private GameObject item;

    void Awake () {
        OnHeadEnter = () => StartCoroutine(Glow());
    }

    public void Init(int i, int j, char type, int axisSum) {
        this.i = i;
        this.j = j;
        this.type = type;

        switch(type) {
            case 'X': item = Instantiate(impenetrableWall, transform, false); break;
            case 'x': item = Instantiate(penetrableWall, transform, false);
                      OnHeadEnter += HitPenetrableWall; break;
        }

        material = tile.material;
        normal = material.color.RotateHSV((i + j) * 2f / axisSum);
        material.color = normal;
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

            //if (type == 'o') {
            //    if (item) {
            //        Destroy(item);
            //        World.CollectOneEnergy();
            //        StartCoroutine(RegenerateEnergy(energyRegeneration));
            //    }
            //}

        // Since head extends part and Enter will be called twice by the head entering, once with the flag true and once false
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
                case '+':
                    item = Instantiate(speedUp, transform, false);
                    OnHeadEnter += CollectSpeedUp;
                    break;
                case '-':
                    item = Instantiate(speedDown, transform, false);
                    OnHeadEnter += CollectSpeedDown;
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

    private void CollectOneEnergy () {
        OnHeadEnter -= CollectOneEnergy;
        Destroy(item);
        item = null;
        OnCollectCollectible();
        World.CollectOneEnergy();
    }

    private void CollectSpeedUp () {
        OnHeadEnter -= CollectSpeedUp;
        Destroy(item);
        item = null;
        OnCollectCollectible();
        World.CollectSpeedUp();
    }

    private void CollectSpeedDown () {
        OnHeadEnter -= CollectSpeedDown;
        Destroy(item);
        item = null;
        OnCollectCollectible();
        World.CollectSpeedDown();
    }

    private IEnumerator Glow() {
        Color bright = Color.Lerp(normal, Color.white, 0.8f);
        WaitForSeconds wait = new WaitForSeconds(1f / 30);
        for (float t=0f; t<1f; t += 1f / 60) {
            material.color = Color.Lerp(bright, normal, t);
            yield return wait;
        }
        material.color = normal;
    }
}

internal static class ColorHsvRotate {
    internal static Color RotateHSV (this Color color, float t) {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        h += t;
        while (h > 1f) h -= 1f;
        Debug.Log("H=" + h);
        return Color.HSVToRGB(h, s, v);
    }
}
