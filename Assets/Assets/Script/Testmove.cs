using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testmove : MonoBehaviour {

    [SerializeField] private float speed;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 p = transform.position;
        p.x += Input.GetAxis("Vertical") * speed;
        p.z -= Input.GetAxis("Horizontal") * speed;

        if (p.z > 7f) p.z -= 10f;
        else if (p.z < -3f) p.z += 10f;

        transform.position = p;
	}
}
