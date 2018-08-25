using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotate : MonoBehaviour {

    private Quaternion rot = Quaternion.AngleAxis(1f, Vector3.up);

	void Start () {
		
	}
	
	void Update () {
        //transform.position = rot * transform.position;
        //transform.rotation *= rot;
	}

    public void Instruct (Vector3 dest, Vector3 tangent) {
        transform.position = Vector3.MoveTowards(transform.position, dest, 2f * Time.deltaTime);
        Vector3 targetEA = transform.eulerAngles;
        targetEA.y = Mathf.Atan2(tangent.x, tangent.z) * Mathf.Rad2Deg;
        //print(targetEA);
        //transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles, targetEA, 200f * Time.deltaTime);
        transform.eulerAngles = targetEA;
    }
}
