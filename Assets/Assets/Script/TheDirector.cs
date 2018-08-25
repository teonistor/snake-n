using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheDirector : MonoBehaviour {

    [SerializeField] private GameObject tile;
    [SerializeField] private Material redTile, yellowTile;

	void Start () {
		for (float x = -20; x < 21; x++) {
            for (float z = -20; z<21; z++) {
                GameObject go = Instantiate(tile, transform);
                go.transform.position = new Vector3(x, 0f, z);
                if (x==0f && z == 0f) {
                    go.name += "(ORIG)";
                    go.GetComponent<Renderer>().material = yellowTile;
                } else if (Random.Range(0,10) > 6) {
                    go.name += "(red)";
                    go.GetComponent<Renderer>().material = redTile;
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
