using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SnakeHead : MonoBehaviour {

    static readonly float incr = 0.1f, deltaDistSq = 0.01f;
    static readonly Vector3 scale = new Vector3(0.41f, 0.41f, 0.06f);
    static readonly Quaternion rot = Quaternion.AngleAxis(45, Vector3.back);

    [SerializeField] private Material snake;
    [SerializeField] private GameObject snakeBodyPart;
    [SerializeField] private int _maxParts = 10;
    [SerializeField] private float speed = 1f;

    //private bool LeftTurn {
    //    get {
    //        return _leftTurn || Input.GetKey(KeyCode.A);
    //    } set {
    //        _leftTurn = value;
    //    }
    //}
    //private bool RightTurn {
    //    get {
    //        return _rightTurn || Input.GetKey(KeyCode.D);
    //    } set {
    //        _rightTurn = value;
    //    }
    //}

    private bool LeftTurn { get {
        if (Input.GetKey(KeyCode.A)) return true;
        Rect r = new Rect(0, 0, Screen.width / 2, Screen.height);
        for (var i=0; i< Input.touchCount; i++) {
            if (r.Contains(Input.GetTouch(i).position))
                return true;
        }
        return false;
    } }
    private bool RightTurn { get {
        if (Input.GetKey(KeyCode.D)) return true;
        Rect r = new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height);
        for (var i=0; i< Input.touchCount; i++) {
            if (r.Contains(Input.GetTouch(i).position))
                return true;
        }
        return false;
    } }

    public int MaxParts { get { return _maxParts; } }
    public float DeltaMove { get; private set; }

    //private List<Vector3> pts;
    private Vector3 lastPos, lastInstruction;
    private SnakePart next;
    private bool _rightTurn;
    private bool _leftTurn;

    public void TouchLeft () {
        //_leftTurn = true;
    }
    public void TouchRight () {
        //_rightTurn = true;
    }

    void Start () {
        //leftTurn = rightTurn = false;
        StartCoroutine(Move());

        //pts = new List<Vector3> {
        //    new Vector3(0, 0.6f, 0),
        //    new Vector3(0, 0.6f, -1),
        //    new Vector3(0, 0.6f, -2),
        //    new Vector3(1, 0.6f, -2),
        //    new Vector3(1, 0.6f, -3)
        //};
       
        lastPos = transform.position;
        next = MakePart();
        next.Init(this, transform, 1);
        lastInstruction = transform.position;
    }

    public SnakePart MakePart (Transform prevPart = null) {
        if (prevPart == null) prevPart = transform;
        return Instantiate(snakeBodyPart, prevPart.position, Quaternion.identity).GetComponent<SnakePart>();
    }

    void Update () {
        DeltaMove = (transform.position - lastPos).magnitude;
        lastPos = transform.position;

        if ((transform.position - lastInstruction).sqrMagnitude > deltaDistSq) {
            lastInstruction = transform.position;
            next.Instruct(transform.position, transform.rotation);
        }

        if (Input.GetKey(KeyCode.Escape)) {
            SceneManager.LoadSceneAsync(0);
        }
    }
    
    private IEnumerator Move () {
        Vector3 anchor, sinFactor, cosFactor, ea;
        float a, omega;
        int turnFactor;
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        yield return new WaitForSeconds(2f);

        while (true) {
            if (LeftTurn == RightTurn) {
                //LeftTurn = RightTurn = false;

                // Move forward
                anchor = transform.position;

                print("Anchor " + anchor);

                for (a = 0f; a < 1f; a += speed * Time.deltaTime) {
                    transform.position = anchor + a * transform.forward;
                    yield return wait;
                }
                transform.position = anchor + transform.forward;


            }
            else {
                omega = 2 * speed; // Angular velocity
                ea = transform.eulerAngles;
                sinFactor = transform.forward * 0.5f;

                if (LeftTurn) {
                    //LeftTurn = false;
                    turnFactor = -1;
                }
                else {
                    //RightTurn = false;
                    turnFactor = 1;
                }
                cosFactor = transform.right * -0.5f * turnFactor;
                anchor = transform.position - cosFactor;

                print("Anchor " + anchor + " sinF " + sinFactor + " cosF " + cosFactor + " pos " + transform.position);

                for (a = 0f; a < 0.5 * Mathf.PI; a += omega * Time.deltaTime) {
                    transform.position = anchor + Mathf.Cos(a) * cosFactor + Mathf.Sin(a) * sinFactor;
                    transform.eulerAngles = new Vector3(ea.x, ea.y + turnFactor * a * Mathf.Rad2Deg, ea.z);
                    yield return wait;
                }
                transform.position = anchor + sinFactor;
                transform.eulerAngles = new Vector3(ea.x, ea.y + turnFactor * 90f, ea.z);
            }
        }
    }

    public void Die() {
        StartCoroutine(DeathAnimation());
    }

    private IEnumerator DeathAnimation() {
        speed = 0f;
        WaitForSeconds wait = new WaitForSeconds(0.033333f);

        int originalPartCount = _maxParts;
        for (float t = 1f; t >= 0f; t -= 0.066666f) {
            _maxParts = (int)(t * originalPartCount);
            yield return wait;
        }
        _maxParts = 0;

        Vector3 originalScale = transform.localScale;
        for (float t = 1f; t >= 0f; t -= 0.066666f) {
            transform.localScale = t * originalScale;
            yield return wait;
        }
        Destroy(gameObject);
    }
}
