using UnityEngine;
using UnityEngine.U2D;
using System.Collections.Generic;
using UnityEngine.UI;

public class ProSlimeBuilder : MonoBehaviour
{
    #region fields
    //for gathering and sorting verices
    public SpriteRenderer rend;
    public List<Vector2> vertices;
    public List<Vector3> sortedVerts;
    public SortedDictionary<float, Vector3> vertDict;

    //prefab junk
    private readonly string par = "particle", cor = "core", skin = "basic" ;

    private GameObject core;
    private GameObject particle;

    //child List
    public List<GameObject> points;
    #endregion

    #region designer methods
    [ContextMenu("Atomize")]
    void Atomize()
    {
        LoadPrefab();
        GetMesh();
        SortVertices();
        PopulateParticles();
    }
    #endregion


    #region builder methods
    void LoadPrefab()
    {
        particle = Resources.Load<GameObject>(par);
        core = Resources.Load<GameObject>(cor);
    }

    void GetMesh() 
    {   
        rend = GetComponent<SpriteRenderer>();
        vertices = new List<Vector2>(rend.sprite.vertices);
    }
    
    void SortVertices()
    {
        //Sorted dict lol leetcode be damned
        vertDict = new SortedDictionary<float, Vector3>();
        for (int i = 0; i < vertices.Count; i++)
        {
            //add parent z-pos to mesh vertex (Vector2)
            Vector3 vert3 = new Vector3(vertices[i].x, vertices[i].y, gameObject.transform.position.z);
            //signed angle from Vector3(1,0,0) and Vector3(vertex) (-180,180)
            float theta = Vector3.Angle(gameObject.transform.right, vert3) * Mathf.Sign(vertices[i].y);
            Debug.Log("gameobject.transform.position = ");
            //key is angle, vector3 value
            vertDict.Add(theta, vert3);
        }
        //copy sorted vertices to list. 
        sortedVerts = new List<Vector3>(vertDict.Values);
    }
    
    void PopulateParticles()
    {   
        //SpawnCore at gameobject center
        GameObject coreAtom = Instantiate(core, gameObject.transform.position, Quaternion.identity);
        coreAtom.transform.parent = gameObject.transform;
        points = new List<GameObject>();
        //spawn at each mesh vertex 
        for (int i = 0; i < sortedVerts.Count; i++)
        {
            //offset is radius of particle? i think roughly? maybe check this lol.
            float offset = particle.transform.localScale.x/2;
            //invert and normalize 
            Vector3 towardCenter = -sortedVerts[i].normalized * offset;
            GameObject atom = Instantiate(particle, gameObject.transform.position + sortedVerts[i] + towardCenter, Quaternion.identity);
           
            atom.transform.parent = gameObject.transform;
            points.Add(atom);
        }   
    }

    [ContextMenu("GiveBones")]
    void GiveBones()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].GetComponents<SpringJoint2D>()[0].connectedBody = gameObject.transform.GetChild(0).GetComponent<Rigidbody2D>();
            if (i == 0)
            {
                points[i].GetComponents<SpringJoint2D>()[1].connectedBody = points[points.Count - 1].GetComponent<Rigidbody2D>();
                points[i].GetComponents<SpringJoint2D>()[2].connectedBody = points[i + 1].GetComponent<Rigidbody2D>();
                points[i].GetComponent<HingeJoint2D>().connectedBody = points[i + 1].GetComponent<Rigidbody2D>();
            }
            else if (i == points.Count - 1) //Last Case
            {
                points[i].GetComponents<SpringJoint2D>()[1].connectedBody = points[i - 1].GetComponent<Rigidbody2D>();
                points[i].GetComponents<SpringJoint2D>()[2].connectedBody = points[0].GetComponent<Rigidbody2D>();
                points[i].GetComponent<HingeJoint2D>().connectedBody = points[0].GetComponent<Rigidbody2D>();
            }
            else
            {
                points[i].GetComponents<SpringJoint2D>()[1].connectedBody = points[i - 1].GetComponent<Rigidbody2D>();
                points[i].GetComponents<SpringJoint2D>()[2].connectedBody = points[i + 1].GetComponent<Rigidbody2D>();
                points[i].GetComponent<HingeJoint2D>().connectedBody = points[i + 1].GetComponent<Rigidbody2D>();
            }
        }
    }

    [ContextMenu("GiveSkin"), ExecuteInEditMode]
    void GiveSkin()
    {
        DestroyImmediate(GetComponent<SpriteRenderer>());
        gameObject.AddComponent<SpriteShapeRenderer>();
        gameObject.AddComponent<SpriteShapeController>();
        SpriteShapeController ssCont = gameObject.GetComponent<SpriteShapeController>();
        ssCont.spline.Clear();
        for (int i = 0; i < points.Count; i++) 
        {
            ssCont.spline.InsertPointAt(i, sortedVerts[i]);
            ssCont.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            ssCont.spline.SetHeight(i, 0);
        }
        ssCont.spriteShape = Resources.Load<SpriteShape>(skin);
    }

    #endregion
}
