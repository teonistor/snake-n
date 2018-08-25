using System.Collections;
using System;
using UnityEngine;

public class SnakePart : MonoBehaviour {

    private TestSnake head;
    private Vector3 destPos;
    private Quaternion destRot;
    private SnakePart next;
    private Transform previous;
    private int partCount;

    private int MaxParts { get {
        return head.MaxParts;
    } }

	public void Init (TestSnake head, Transform previous, int partCount) {
        this.head = head;
        this.previous = previous;
        this.partCount = partCount;
	}

    public void Instruct (Vector3 destPos, Quaternion destRot) {
        //print("Instruct snak p " + partCount + " to go to " + destPos);
        if (next) {
            next.Instruct(this.destPos, this.destRot);
        } else if (partCount < MaxParts) {
            next = head.MakePart(transform);
            next.Init(head, transform, partCount + 1);
        }

        this.destPos = destPos;
        this.destRot = destRot;
    }

	void Update () {
        //print("SnakePart update moving from " + transform.position + " to " + destPos + " by " + head.DeltaMove);
        Vector3 pos = Vector3.MoveTowards(transform.position, destPos, head.DeltaMove);
        Vector3 ea = new Vector3(0f, Vector3.SignedAngle(Vector3.forward, previous.position - transform.position, Vector3.up), 0f);
        //float t = (pos - transform.position).sqrMagnitude / (destPos - transform.position).sqrMagnitude;
        //transform.rotation = Quaternion.Lerp(transform.rotation, destRot, t);
        transform.position = pos;
        transform.eulerAngles = ea;
        //transform.position = destPos;
	}
}
