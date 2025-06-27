using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshController : MonoBehaviour
{
    public List<Transform> points;
    public PolygonCollider2D polygonCollider;
    MeshFilter mf;
    Transform parent;
    
    void OnEnable()
    {
        mf = GetComponent<MeshFilter>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        parent = transform.GetComponentInParent<Transform>();

        
        

    }

    private void Update()
    {
        PolyMesh();
    }
    private void PolyMesh()
    {
        Mesh mesh = new Mesh();
        // vertices
        Vector3[] vertices = new Vector3[points.Count];

        // Normals 
        Vector3[] normals = new Vector3[points.Count]; 
        

        // uvs
        Vector2[] uvs = new Vector2[points.Count];

        // polycollider
        polygonCollider.pathCount = 1;
        Vector2[] path = new Vector2[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 vertex = points[i].localPosition;
            Vector2 towardsCenter = -vertex.normalized;
            float colliderRadius = points[i].localScale.x * points[i].gameObject.GetComponent<CircleCollider2D>().radius;
            vertex = vertex - (towardsCenter * colliderRadius);
            vertices[i] = (new Vector3(vertex.x, vertex.y, 0f));

            // normals
            normals[i] = -Vector3.forward;

            // uvs
            //float radius = ((vertex - (towardsCenter * colliderRadius)).magnitude * 2 + .5f);
            //float radius = ((vertex - (towardsCenter * colliderRadius)).magnitude);
            float radius = ((vertex - (towardsCenter)).magnitude);
            //uvs[i] = new Vector2(vertices[i].x / radius, vertices[i].y / radius);

            // polygon collider
            path[i] = vertex;

        }

        polygonCollider.SetPath(0, path);
        List<int> trianglesList = new(); 
        
        for (int i = 0; i < (points.Count-2); i++)
        {
            trianglesList.Add(0);
            trianglesList.Add(i + 1);
            trianglesList.Add(i + 2);
        }
        int[] triangles = trianglesList.ToArray();


        // init
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        //mesh.uv = uvs;

        mf.mesh = mesh;

       


    }

    
}
