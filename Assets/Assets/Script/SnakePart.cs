using System.Collections;
using System;
using UnityEngine;

public class SnakePart : MonoBehaviour {
    static readonly int NO_COLLISION_COUNT = 10;

    private SnakeHead head;
    private Vector3 destPos;
    private Quaternion destRot;
    private SnakePart next;
    private Transform previous;
    public int partCount { get; private set; }

    private int MaxParts { get {
        return head.MaxParts;
    } }

	public void Init (SnakeHead head, Transform previous, int partCount) {
        this.head = head;
        this.previous = previous;
        this.partCount = partCount;

        // Activate self-collision
        if (partCount > NO_COLLISION_COUNT) {
            gameObject.layer = 10;
        }
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
        Vector3 ea = new Vector3(0f, Vector3.SignedAngle(Vector3.right, previous.position - (next == null ? transform : next.transform).position, Vector3.up), 0f);
        //float t = (pos - transform.position).sqrMagnitude / (destPos - transform.position).sqrMagnitude;
        //transform.rotation = Quaternion.Lerp(transform.rotation, destRot, t);
        transform.position = pos;
        transform.eulerAngles = ea;
        //transform.position = destPos;
	}

    void OnTriggerEnter (Collider other) {
        print("Snake bit its tail");
        Debug.Break();
    }
}
