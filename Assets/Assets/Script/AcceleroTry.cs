using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class AcceleroTry : MonoBehaviour {

    public static float speed = 5f;
    static readonly Vector3 limits = new Vector3(10f, 80f, 0f);

    private Vector3 initialLea, maxLea, minLea;
    private bool useAcceleration;

    void Start () {
        Input.compensateSensors = true;
        initialLea = transform.localEulerAngles;
        maxLea = initialLea + limits;
        minLea = initialLea - limits;
        useAcceleration = Input.acceleration != Vector3.zero;
        if (!useAcceleration) {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
    }

    void OnDisable () {
        if (!useAcceleration) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

	void Update () {
        Vector3 lea = transform.localEulerAngles;

        if (useAcceleration) {
            Vector3 acc = Input.acceleration;
            lea.x += acc.y * speed * Time.deltaTime;
            lea.y += acc.x * speed * Time.deltaTime;
        } else {
            lea.x += Input.GetAxis("Mouse Y") * speed * Time.deltaTime;
            lea.y += Input.GetAxis("Mouse X") * speed * Time.deltaTime;
        }

        if (lea.x > maxLea.x) lea.x = maxLea.x;
        else if (lea.x < minLea.x) lea.x = minLea.x;
        if (lea.y > maxLea.y) lea.y = maxLea.y;
        else if (lea.y < minLea.y) lea.y = minLea.y;
        //float x = acc.y * speed * Time.deltaTime;

        //acc = new Vector3(acc.y, acc.x, 0f);
        //acc *= speed * Time.deltaTime;
        //Vector3.cl

        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(acc), Time.deltaTime * 5);

        transform.localEulerAngles = lea;
    }
}
