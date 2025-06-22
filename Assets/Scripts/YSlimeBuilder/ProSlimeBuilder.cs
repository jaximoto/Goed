using UnityEngine;
using UnityEngine.ProBuilder;
using System.Collections.Generic;
public class ProSlimeBuilder : MonoBehaviour
{
    public SpriteRenderer rend;
    public List<Vector2> vertices;
    public List<Vector3> sortedVerts;

    public GameObject core;
    public GameObject particle;
    public List<GameObject> points;

    [SerializeField]
    public SortedDictionary <float, Vector3> vertDict;



    #region builder functions


    [ContextMenu("Get Mesh")]
    void GetMesh() 
    {   
        rend = GetComponent<SpriteRenderer>();
        
        Debug.Log(rend.sprite.vertices.Length);
        vertices = new List<Vector2>(rend.sprite.vertices);
    }

    [ContextMenu("Sort Vertices")]
    void SortVertices()
    {
        vertDict = new SortedDictionary<float, Vector3>();
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 vert3 = new Vector3(vertices[i].x, vertices[i].y, gameObject.transform.position.z);
            float theta = Vector3.Angle(gameObject.transform.position + Vector3.right, vert3) * Mathf.Sign(vertices[i].y);
            vertDict.Add(theta, vert3);
        }

        Debug.Log($"dictLength={vertDict.Count}");
        sortedVerts = new List<Vector3>(vertDict.Values);

    }


    [ContextMenu("Populate Particles")]
    void PopulateParticles()
    {   
        //SpawnCore at gameobject center
        GameObject coreAtom = Instantiate(core, gameObject.transform.position, Quaternion.identity);
        coreAtom.transform.parent = gameObject.transform;

        //spawn at each mesh vertex 
        for (int i = 0; i < sortedVerts.Count; i++)
        {
            //Vector3 vertice3 = new Vector3(vertices[i].x, vertices[i].y, gameObject.transform.position.z);
            
            
            float offset = particle.transform.localScale.x/2;
            Vector3 towardCenter = -sortedVerts[i].normalized * offset;
            GameObject atom = Instantiate(particle, gameObject.transform.position + sortedVerts[i] + towardCenter, Quaternion.identity);
           
            atom.transform.parent = gameObject.transform;
            points.Add(atom);
        }   
    }


    /* todo
     * 
     * clean up outdated builder files before pushing to master
     * 
     * fill & lattice structure
     * 
     * 
     */


    #endregion
}
