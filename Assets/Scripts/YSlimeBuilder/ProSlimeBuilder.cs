using UnityEngine;
using UnityEngine.U2D;
using System.Collections.Generic;
using UnityEngine.UI;

public class ProSlimeBuilder : MonoBehaviour
{
    #region ingredients
    //for gathering and sorting verices
    private SpriteRenderer rend;
    public List<Vector2> vertices;
    public List<Vector3> sortedVerts;
    public SortedDictionary<float, Vector3> vertDict;

    //prefab junk
    private readonly string par = "particle", cor = "core", skin = "basic" ;

    private GameObject core;
    private GameObject particle;
    private GameObject coreAtom;
    //child List
    public List<GameObject> points;

    public List<float> angles;
    #endregion

    #region spells
    [ContextMenu("Polymorph Slime")]
    void PolymorphSlime()
    {
        Sporify();
        GiveForm();
        SwapSkin();
    }
    

    void Sporify()
    {
        LoadPrefab();
        GetMesh();
        SortVertices();
        PopulateParticles();
    }

    void GiveForm()
    {
        GiveBones();
        GiveSkin();  
    }

    void SwapSkin()
    {
        gameObject.transform.DetachChildren();
        gameObject.transform.SetParent(coreAtom.transform);
    }


    #endregion


    #region spell components
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
            float theta = 180 + Vector3.Angle(gameObject.transform.right, vert3) * Mathf.Sign(vertices[i].y);
            //key is angle, vector3 value
            vertDict.Add(theta, vert3);
        }
         angles = new List<float>(vertDict.Keys);
        //Refine Vertice list before passing it on
        CheckAngles();
        
        //copy sorted vertices to list. terrible to do this twice i think 
        sortedVerts = new List<Vector3>(vertDict.Values);
        CheckDistance();
        sortedVerts = new List<Vector3>(vertDict.Values);
    }


    
    //Various helpers for SortVertice(). Goal is to refine sorted Vector3 List before passing to particlize functions. Ideally clean up extra data storage when done
    #region sorting particle helpers
    void CheckAngles()
    {
        for (int i = 0; i < angles.Count; i++)
        {
            float angleDiff;
            
            if (i == angles.Count - 1)
            {   
                angleDiff = Mathf.Abs(angles[i] - (angles[0] + 360));
                if (angleDiff <= 8) MergeAtomsFromAngles(angles[i], angles[0]); 
            }
            else 
            { 
                angleDiff = Mathf.Abs(angles[i] - angles[i + 1]);
                if (angleDiff <= 8) MergeAtomsFromAngles(angles[i], angles[i + 1]); 
            }
            Debug.Log($"iteration {i} has angleDiff {angleDiff}");
        }
    }

    void MergeAtomsFromAngles(float a1, float a2) //i should call these aa and ab lol
    {
        
        Vector3 p1 = vertDict[a1]; Vector3 p2 = vertDict[a2]; 
        Vector3 newP = (p1 + p2).normalized * p1.magnitude;
        float newA = 180 + Vector3.Angle(gameObject.transform.right, newP) * Mathf.Sign(newP.y);
        Debug.Log($"merging atoms a1 {a1}, a2 {a2} || new angle = {newA}, new point = {newP}");
        vertDict.Remove(a1); vertDict.Remove(a2);
        vertDict.Add(newA, newP);
    }

    void CheckDistance()
    {
        for (int i = 0; i < sortedVerts.Count; i++) 
        {
            float distance;
            if (i == sortedVerts.Count - 1)
            {
                distance = (sortedVerts[i] - sortedVerts[0]).magnitude;
                if (distance >= 0.5) FillDistance(distance, sortedVerts[i], sortedVerts[0]);
            }
            else 
            {
                distance = (sortedVerts[i] - sortedVerts[i + 1]).magnitude;
                if (distance >= 1) FillDistance(distance, sortedVerts[i], sortedVerts[i + 1]);
            }
        }
    }


    void FillDistance(float dis, Vector3 v1, Vector3 v2)
    {
        
        int newPoints = Mathf.RoundToInt(dis / 0.5f) - 1;
        //get direction
        Vector3 dir = (v2 - v1).normalized;
        float disAdd = dis / (newPoints + 1);
        Debug.Log($"v1 = {v1}, v2 = {v2}, dir = {dir}, disAdd = {disAdd}");
        for (int i = 0;i < newPoints;i++) 
        { 
            Vector3 newP = v1 + (dir * disAdd * (i+1));
            //add Manual offset

            Vector3 offset = newP.normalized * (particle.transform.localScale.x * particle.GetComponent<CircleCollider2D>().radius)/2;
            Vector3 offP = newP + offset;
            float newA = 180 + Vector3.Angle(gameObject.transform.right, newP) * Mathf.Sign(newP.y);
            Debug.Log($"iteration {i} || newP is {newP} and newA is {newA}");
            vertDict.Add(newA, offP);
        }
    }

    #endregion 
    void PopulateParticles()
    {
        //SpawnCore at gameobject center
        coreAtom = Instantiate(core, gameObject.transform.position, Quaternion.identity);
        coreAtom.transform.parent = gameObject.transform;
        points = new List<GameObject>();
        //spawn at each mesh vertex 
        for (int i = 0; i < sortedVerts.Count; i++)
        {
            //offset is radius of particle? i think roughly? maybe check this lol.
            float offset = particle.transform.localScale.x * particle.GetComponent<CircleCollider2D>().radius;
            //invert and normalize 
            Vector3 towardCenter = -sortedVerts[i].normalized * offset;
            GameObject atom = Instantiate(particle, gameObject.transform.position + sortedVerts[i] + towardCenter, Quaternion.identity);
           
            atom.transform.parent = coreAtom.transform;
            coreAtom.GetComponent<SoftBodyController>().points.Add(atom.transform);
            points.Add(atom);
        }   

    }

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

    [ExecuteInEditMode]
    void GiveSkin()
    {
        DestroyImmediate(gameObject.GetComponent<SpriteRenderer>());
        gameObject.AddComponent<SpriteShapeRenderer>();
        gameObject.AddComponent<SpriteShapeController>();
        SpriteShapeController ssCont = gameObject.GetComponent<SpriteShapeController>();
        coreAtom.GetComponent<SoftBodyController>().spriteShape = ssCont;
        ssCont.spline.Clear();
        for (int i = 0; i < points.Count; i++) 
        {
            
            ssCont.spline.InsertPointAt(i, sortedVerts[i]);
            ssCont.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            ssCont.spline.SetHeight(i, 0);
        }
        ssCont.spriteShape = Resources.Load<SpriteShape>(skin);
        
    }


    void SetTangents()
    {

    
    }
    void UpdateTangents()
    {
       
    }

    #endregion
}
