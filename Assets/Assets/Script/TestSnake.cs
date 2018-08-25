using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSnake : MonoBehaviour {

    static readonly float incr = 0.1f, deltaDistSq = 0.16f;
    static readonly Vector3 scale = new Vector3(0.41f, 0.41f, 0.06f);
    static readonly Quaternion rot = Quaternion.AngleAxis(45, Vector3.back);

    [SerializeField] private Material snake;
    [SerializeField] private int _maxParts = 100;
    [SerializeField] private float speed = 0.9f;

    private bool LeftTurn { get { return Input.GetKey(KeyCode.A); } }
    private bool RightTurn { get { return Input.GetKey(KeyCode.D); } }

    public int MaxParts { get { return _maxParts; } }
    public float DeltaMove { get; private set; }

    private List<Vector3> pts;
    private List<TestRotate> body;
    private Vector3 lastPos;
    private SnakePart next;
    
	void Start () {
        //leftTurn = rightTurn = false;
        StartCoroutine(Move());

        pts = new List<Vector3> {
            new Vector3(0, 0.6f, 0),
            new Vector3(0, 0.6f, -1),
            new Vector3(0, 0.6f, -2),
            new Vector3(1, 0.6f, -2),
            new Vector3(1, 0.6f, -3)
        };
        body = new List<TestRotate>();
        lastPos = transform.position;
        //float x, z, y = 0.6f;

        //Vector3 test = rot * scale;

        //for (float f=0; f<2f; f +=0.037f) {
        //          x = 2f * Mathf.Cos(f);
        //          z = 2f * Mathf.Sin(f);
        //          GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //          go.transform.parent = transform;
        //          go.transform.position = new Vector3(x, y, z);
        //          go.transform.localScale = scale;
        //          go.transform.localRotation *= Quaternion.AngleAxis(90 + f * Mathf.Rad2Deg, Vector3.down) * rot;
        //          go.GetComponent<Renderer>().material = snake;
        //          go.AddComponent(typeof(TestRotate));
        //      }

        next = MakePart();
        next.Init(this, 1);

    }
	
    public SnakePart MakePart(Transform prevPart = null) {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.parent = transform.parent;
        if (prevPart == null) {
            go.transform.position = transform.position;
            go.transform.localScale = scale;
            go.transform.localRotation = rot;
        } else {
            go.transform.position = prevPart.position;
            go.transform.localScale = prevPart.localScale;
            go.transform.localRotation = prevPart.rotation;
        }
        go.GetComponent<Renderer>().material = snake;
        return go.AddComponent<SnakePart>();
    }

    void Update () {
        DeltaMove = (transform.position - lastPos).magnitude;
        lastPos = transform.position;

        //transform.position += new Vector3(speed * Input.GetAxis("Horizontal"), 0f, speed * Input.GetAxis("Vertical"));
        if ((transform.position - next.transform.position).sqrMagnitude > deltaDistSq)
            next.Instruct(transform.position, transform.rotation);
    }

	void Update1 () {

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            print("Left");
            for (int j=0; j<pts.Count; j++) {
                pts[j] = new Vector3(pts[j].z, pts[j].y, -pts[j].x);
            }
        }



        int i = 0;
        for (int j=1; j<pts.Count; j++) {
            if (j == pts.Count - 1 || Vector3.Cross(pts[j - 1] - pts[j], pts[j + 1] - pts[j]) == Vector3.zero) {
                for (float t = 0f; t <= 1f; t += incr) {
                    getBodyPart(i).Instruct(
                        (1 - t) * pts[j - 1] + t * pts[j],
                        pts[j] - pts[j-1]
                    );
                    i++;
                }
                //getBodyPart(i).Instruct(Vector3.Lerp)
            }
            else {
                for (float t = 0f; t <= 1f; t += incr * 0.5f) {
                    getBodyPart(i).Instruct(
                        (1 - t) * (1 - t) * pts[j - 1] + 2 * t * (1 - t) * pts[j] + t * t * pts[j + 1],
                        (1 - t) * (pts[j] - pts[j - 1]) + t * (pts[j + 1] - pts[j])
                    );
                    i++;
                }
                j++;
            }
        }
	}

    private TestRotate getBodyPart(int i) {
        if (i < body.Count)
            return body[i];
        else {
            //x = 2f * Mathf.Cos(f);
            //z = 2f * Mathf.Sin(f);
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.parent = transform;
            //go.transform.position = new Vector3(x, y, z);
            go.transform.localScale = scale;
            go.transform.localRotation *= /*Quaternion.AngleAxis(90 + f * Mathf.Rad2Deg, Vector3.down) * */ rot;
            go.GetComponent<Renderer>().material = snake;
            TestRotate r = go.AddComponent<TestRotate>();
            body.Add(r);
            return r;
        }
    }

    private IEnumerator Move () {
        Vector3 anchor, sinFactor, cosFactor, ea;
        float a, omega;
        int turnFactor;

        while (true) {
            if (LeftTurn == RightTurn) {
                //leftTurn = rightTurn = false;

                // Move forward
                anchor = transform.position;

                print("Anchor " + anchor);

                for (a = 0f; a <1f; a += speed * Time.deltaTime) {
                    transform.position = anchor + a * transform.forward;
                    yield return new WaitForEndOfFrame();
                }
                transform.position = anchor + transform.forward;


            } else {
                omega = 2 * speed; // Angular velocity
                ea = transform.eulerAngles;
                sinFactor = transform.forward * 0.5f;

                if (LeftTurn) {
                    //leftTurn = false;
                    turnFactor = -1;
                } else {
                    turnFactor = 1;
                }
                cosFactor = transform.right * -0.5f * turnFactor;
                anchor = transform.position - cosFactor;

                print("Anchor " + anchor + " sinF " + sinFactor + " cosF " + cosFactor + " pos " + transform.position);


                for (a = 0f; a < 0.5 * Mathf.PI; a += omega * Time.deltaTime) {
                    transform.position = anchor + Mathf.Cos(a) * cosFactor + Mathf.Sin(a) * sinFactor;
                    transform.eulerAngles = new Vector3(ea.x, ea.y + turnFactor * a * Mathf.Rad2Deg, ea.z);
                    yield return new WaitForEndOfFrame();
                }
                transform.position = anchor + sinFactor;
                transform.eulerAngles = new Vector3(ea.x, ea.y + turnFactor * 90f, ea.z);
            }
        }
    }
}
