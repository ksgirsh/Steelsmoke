using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeshLine : MonoBehaviour
{
    [SerializeField] float thickness;

    [SerializeField] List<Transform> points;

    Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        DrawLine();
    }

    void DrawLine()
    {
        int count = points.Count;
        Vector3[] vertices = new Vector3[count * 2];
        int[] triangles = new int[(count - 1) * 6];

        for (int i = 0; i < count; i++)
        {
            Vector3 p = points[i].position;

            Vector3 dir;
            if (i == 0)
                dir = (points[i + 1].position - p).normalized;
            else if (i == count - 1)
                dir = (p - points[i - 1].position).normalized;
            else
                dir = (points[i + 1].position - points[i - 1].position).normalized;

            Vector3 normal = new Vector3(-dir.y, dir.x, 0f);
            Vector3 offset = normal * (thickness * 0.5f);

            vertices[i * 2] = p - offset; // left
            vertices[i * 2 + 1] = p + offset; // right;

            if (i < count - 1)
            {
                int t = i * 6;
                int v = i * 2;

                triangles[t] = v;
                triangles[t + 1] = v + 1;
                triangles[t + 2] = v + 3;

                triangles[t + 3] = v + 3;
                triangles[t + 4] = v + 2;
                triangles[t + 5] = v;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

    }
}
