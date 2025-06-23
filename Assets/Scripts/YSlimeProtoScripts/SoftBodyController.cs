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
    public Transform[] points;
    

    #region 
    #endregion


    #endregion

    #region callbacks
    private void Awake()
    {        
        UpdateVertices();        
    }
    private void LateUpdate()
    {
        UpdateVertices();        
    }
    #endregion

    #region privateMethods
    private void UpdateVertices() {
        for (int i = 0; i < points.Length; i++) {
            Vector2 vertex = points[i].localPosition;
            Vector2 towardsCenter = -vertex.normalized;

            //when softbody controller is setup pull radius from there, this is stupid
            float colliderRadius = points[i].gameObject.GetComponent<CircleCollider2D>().radius;
            try
            {
                spriteShape.spline.SetPosition(i, (vertex - towardsCenter * colliderRadius));
            }
            catch 
            {
                Debug.Log("Spline points are too close.. recalculate");
                spriteShape.spline.SetPosition(i, (vertex - towardsCenter * (colliderRadius + splineOffset)));
            }

            Vector2 lt = spriteShape.spline.GetLeftTangent(i);

            Vector2 newRt = Vector2.Perpendicular(towardsCenter) * lt.magnitude;
            Debug.Log($"lt magnitude is {lt.magnitude}");
            Vector2 newLt = -newRt;

            spriteShape.spline.SetLeftTangent(i, newLt);
            spriteShape.spline.SetRightTangent(i, newRt);

        }
    }


    #endregion

}
