using System.Collections;
using System;
using UnityEngine;

public class SnakePart : MonoBehaviour {

    private TestSnake head;
    private Rigidbody headRb;
    private Vector3 destPos;
    private Quaternion destRot;
    private SnakePart next;
    private int partCount;

    private int MaxParts { get {
        return head.MaxParts;
    } }

	public void Init (TestSnake head, int partCount) {
        this.head = head;
        this.partCount = partCount;
        headRb = head.GetComponent<Rigidbody>();

	}

    public void Instruct (Vector3 destPos, Quaternion destRot) {
        this.destPos = destPos;
        this.destRot = destRot;
        if (next) {
            next.Instruct(transform.position, transform.rotation);
        } else if (partCount < MaxParts) {
            next = head.MakePart(transform);
            next.Init(head, partCount + 1);
        }
    }

	void Update () {
        //print("SnakePart update moving from " + transform.position + " to " + destPos + " by " + head.DeltaMove);
        Vector3 pos = Vector3.MoveTowards(transform.position, destPos, head.DeltaMove);
        float t = (pos - transform.position).sqrMagnitude / (destPos - transform.position).sqrMagnitude;
        transform.rotation = Quaternion.Lerp(transform.rotation, destRot, t);
        transform.position = pos;
	}
}
