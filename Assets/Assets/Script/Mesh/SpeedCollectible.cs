using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
[ExecuteInEditMode]
public class SpeedCollectible : MonoBehaviour {

    private float r = 0.35f, d = 0.18f;

    private MeshFilter filter;

    void Start () {
        filter = GetComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        mesh.name = "Speed Collectible";

        mesh.vertices = new Vector3[] {
            new Vector3(-r, 0f, 0f),
            new Vector3(0f,  r, 0f),
            new Vector3( r, 0f, 0f),
            new Vector3(0f, -r, 0f),
            new Vector3(0f, 0f,  d),
            new Vector3(0f, 0f, -d)
        };
        mesh.uv = new Vector2[] {
            new Vector2(  0f,   0f),
            new Vector2(  0f,   1f),
            new Vector2(  1f,   1f),
            new Vector2(  0f,   1f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f)
        };
        mesh.triangles = new int[] {
             0, 3, 4,
             1, 0, 4,
             2, 1, 4,
             3, 2, 4,
             0, 1, 5,
             1, 2, 5,
             2, 3, 5,
             3, 0, 5
        };
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        filter.mesh = mesh;
    }
}
