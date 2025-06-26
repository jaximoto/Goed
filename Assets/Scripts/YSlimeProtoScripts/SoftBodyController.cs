using UnityEngine;
using UnityEngine.U2D;
using System.Collections.Generic;

public class SoftBodyController : MonoBehaviour
{
    #region consts
    private const float splineOffset = 0.1f;
    #endregion

    #region fields
    public SpriteShapeController spriteShape;
    public List<Transform> points = new List<Transform>();
    

    #region 
    #endregion


    #endregion

    #region callbacks
    private void Awake()
    {        
        UpdateVertices();        
    }
    private void Update()
    {
        UpdateVertices();        
    }
    #endregion

    #region privateMethods
    private void UpdateVertices() {
        for (int i = 0; i < points.Count; i++) {
            Vector2 vertex = points[i].localPosition * gameObject.transform.localScale.x;
            Vector2 towardsCenter = -vertex.normalized;

            Debug.Log("localPositions ");
            //when softbody controller is setup pull radius from there, this is stupid
            float colliderRadius = gameObject.transform.localScale.x * points[i].transform.localScale.x * points[i].gameObject.GetComponent<CircleCollider2D>().radius;
            //Debug.Log($"points[i].gameObject.transform.localScale.x is {points[i].gameObject.transform.localScale.x} and radius is{points[i].gameObject.GetComponent<CircleCollider2D>().radius}");
            try
            {
                spriteShape.spline.SetPosition(i, vertex - (towardsCenter * colliderRadius));
            }
            catch 
            {
                Debug.Log("Catch");
                spriteShape.spline.SetPosition(i, vertex - (towardsCenter * (colliderRadius + splineOffset)));
            }

            //Debug.Log($"Vertex is {vertex}");
            Vector2 lt = spriteShape.spline.GetLeftTangent(i);

            Vector2 newRt = Vector2.Perpendicular(towardsCenter) * lt.magnitude;
            Vector2 newLt = -newRt;

            spriteShape.spline.SetLeftTangent(i, newLt);
            spriteShape.spline.SetRightTangent(i, newRt);

        }
    }


    #endregion

}
