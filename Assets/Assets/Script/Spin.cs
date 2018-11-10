using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {

    [SerializeField] float speed;

    private Vector3 rotationAxis;

    void Start() {
        rotationAxis = transform.worldToLocalMatrix * Vector3.up;
    }

	void Update () {
        transform.rotation *= Quaternion.AngleAxis(speed * Time.deltaTime, rotationAxis);
	}
}
