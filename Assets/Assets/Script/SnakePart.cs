using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class SnakePart : MonoBehaviour {
    
    [SerializeField] internal AnimationClip[] clips;

    [SerializeField] internal GameObject another;

    LevelTile currentSection;

    protected int indexInSnake;
    SnakePart tail = null;

    public Animation Animation { get; internal set; }

    void Awake() {
        Animation = GetComponent<Animation>();
        World.OnSpeedChange += UpdateAnimationSpeed;
    }

    protected virtual void Start () {
        UpdateAnimationSpeed();
        NextTile ();
	}
	
	protected virtual void Update () {
        if (indexInSnake >= World.currentEnergy && tail != null && tail.tail == null) {
            tail.currentSection.Leave();
            Destroy(tail.gameObject);
            tail = null;
        }
    }

    void OnDisable () {
        World.OnSpeedChange -= UpdateAnimationSpeed;
    }

    void UpdateAnimationSpeed() {
        foreach(AnimationState state in Animation) {
            state.speed = World.CurrentTotalSpeed;
        }
    }

    internal void Instruct(LevelTile section, AnimationClip clip) {
        Animation.Stop();
        if (currentSection != null)
            currentSection.Leave();

        currentSection = section;
        transform.parent = section.transform;

        currentSection.Enter();
        Animation.clip = clip;
        Animation.Play();
    }

    protected virtual void NextTile () {}

    protected virtual void QuarterTile () { // TODO rename this
        if (indexInSnake < World.currentEnergy && tail == null) {
            tail = Instantiate(another, transform.parent).GetComponent<SnakePart>();
            tail.indexInSnake = indexInSnake + 1;
        }

        if (tail != null) {
            tail.Instruct(currentSection, Animation.clip);
        }
    }

    public void AppendHoverMeshVertices(List<Vector3> vertices) {
        Vector3 pos = transform.position;
        vertices.Add(pos + transform.right * -0.4f);
        vertices.Add(pos + transform.up * 0.4f);
        vertices.Add(pos + transform.right * 0.4f);
        if (tail != null)
            tail.AppendHoverMeshVertices(vertices);
    }
}
