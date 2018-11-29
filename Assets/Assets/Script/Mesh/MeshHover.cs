using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHover : MonoBehaviour {
    [SerializeField] SnakePart head;

    MeshFilter filter;

    void Start() {
        filter = GetComponent<MeshFilter>();
    }

	void Update () {
        List<Vector3> vertices = new List<Vector3>();
        head.AppendHoverMeshVertices(vertices);

        int n = vertices.Count / 3 - 1;
        int[] triangles = new int[n * 12];
        for (int i = 0; i < n; i++) {
            triangles[i * 12    ] = i * 3;
            triangles[i * 12 + 1] = i * 3 + 1;
            triangles[i * 12 + 2] = i * 3 + 3;
            triangles[i * 12 + 3] = i * 3 + 1;
            triangles[i * 12 + 4] = i * 3 + 4;
            triangles[i * 12 + 5] = i * 3 + 3;
            triangles[i * 12 + 6] = i * 3 + 1;
            triangles[i * 12 + 7] = i * 3 + 2;
            triangles[i * 12 + 8] = i * 3 + 4;
            triangles[i * 12 + 9] = i * 3 + 2;
            triangles[i * 12 +10] = i * 3 + 5;
            triangles[i * 12 +11] = i * 3 + 4;
        }

        Mesh mesh = new Mesh();
        mesh.MarkDynamic();
        mesh.name = "Hover";

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        //...

        filter.mesh = mesh;
	}
}
