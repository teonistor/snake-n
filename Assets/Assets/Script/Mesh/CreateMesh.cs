using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
[ExecuteInEditMode]
public class CreateMesh : MonoBehaviour {

    [SerializeField] private float r = 0.13f, d = 0.15f, l = 0.11f;

    private MeshFilter filter;
    //private float defer = 0;

    void Start () {
        filter = GetComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        mesh.name = "Snake Part";

        mesh.vertices = new Vector3[] {
            new Vector3(-l,  r, 0f),
            new Vector3(-l, 0f,  r),
            new Vector3(-l, -r, 0f),
            new Vector3(-l, 0f, -r),
            new Vector3(0f,  d, 0f),
            new Vector3(0f, 0f,  d),
            new Vector3(0f, -d, 0f),
            new Vector3(0f, 0f, -d),
            new Vector3( l,  r, 0f),
            new Vector3( l, 0f,  r),
            new Vector3( l, -r, 0f),
            new Vector3( l, 0f, -r)
        };
        mesh.uv = new Vector2[] {
            new Vector2(  0f, 0f),
            new Vector2(  0f, 1f),
            new Vector2(  0f, 0f),
            new Vector2(  0f, 1f),
            new Vector2(0.5f, 0f),
            new Vector2(0.5f, 1f),
            new Vector2(0.5f, 0f),
            new Vector2(0.5f, 1f),
            new Vector2(  1f, 0f),
            new Vector2(  1f, 1f),
            new Vector2(  1f, 0f),
            new Vector2(  1f, 1f)
        };
        mesh.triangles = new int[] {
             0,  4,  7,
             0,  7,  3,
             3,  7,  2,
             2,  7,  6,
             2,  6,  5,
             2,  5,  1,
             1,  5,  4,
             1,  4,  0,
             4,  8,  7,
             7,  8, 11,
             7, 11,  6,
             6, 11, 10,
             6, 10,  5,
             5, 10,  9,
             5,  9,  4,
             4,  9,  8
        };
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        filter.mesh = mesh;

        //SnakePart p = GetComponent<SnakePart>();
        //if (p != null) defer = p.partCount * 0.3f;
    }
	
    //void LateUpdate () {
    //    d = 0.1f + 0.03f * Mathf.Sin(Time.time * 2 + defer);
    //    D = 0.12f + 0.04f * Mathf.Sin(Time.time* 2 + defer);
    //}
}
