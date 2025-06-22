using UnityEngine;
using UnityEngine.ProBuilder;
using System.Collections.Generic;
public class ProSlimeBuilder : MonoBehaviour
{
    public SpriteRenderer rend;
    public List<Vector2> vertices;

    public GameObject core;
    public GameObject particle;
    public List<GameObject> points;


    #region builder functions


    [ContextMenu("Get Mesh")]
    void GetMesh() 
    {   
        rend = GetComponent<SpriteRenderer>();
        
        Debug.Log(rend.sprite.vertices.Length);
        vertices = new List<Vector2>(rend.sprite.vertices);
    }

    [ContextMenu("Populate Particles")]
    void PopulateParticles()
    {   
        //SpawnCore at gameobject center
        GameObject coreAtom = Instantiate(core, gameObject.transform.position, Quaternion.identity);
        coreAtom.transform.parent = gameObject.transform;


        //lets generate an offset

        //spawn at each mesh vertex 
        for (int i = 0; i < vertices.Count; i++)
        {
            
            Vector3 vertice3 = new Vector3(vertices[i].x, vertices[i].y, gameObject.transform.position.z);

            float offset = particle.transform.localScale.x/2;

            Vector3 towardCenter = -vertice3.normalized * offset;

            Debug.Log($"Offset: desired = 0.125 || actual = {offset}");
            Debug.Log($"towardCenter: vertice3 = {vertice3} || towardCenter = {towardCenter}");
            Debug.Log($"");
            GameObject atom = Instantiate(particle, gameObject.transform.position + vertice3 + towardCenter, Quaternion.identity);
           
            atom.transform.parent = gameObject.transform;
            points.Add(atom);
        }   

         
    }

    [ContextMenu("CleanUp")]
    void CleanUp()
    {

    }

    #endregion
}
