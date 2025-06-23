using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;
using System.Collections.Generic;

public class SoftBodyController : MonoBehaviour
{
    #region consts
    private const float splineOffset = 0.1f;
    #endregion

    #region fields
    [SerializeField] public SpriteShapeController spriteShape;
    [SerializeField] public Transform[] points;
    
    [SerializeField] private List<SpringJoint2D> outerSprings = new List<SpringJoint2D>();
    [SerializeField] private List<DistanceJoint2D> outerRods = new List<DistanceJoint2D>();
    [SerializeField] private List<SpringJoint2D> innerSprings = new List<SpringJoint2D>();

    [Range(0, 3)]
    public float targetScale, scaleSpeed;

    #region 
    #endregion


    #endregion

    #region callbacks
    private void Awake()
    {
        MatchTargetScale();
        UpdateVertices();
        GetJoints();
    }
    private void Update()
    {
        UpdateVertices();
        UpdateSize();

    }
    #endregion

    #region privateMethods
    private void UpdateVertices() {
        for (int i = 0; i < points.Length; i++) {
            Vector2 vertex = points[i].localPosition;
            Vector2 towardsCenter = (Vector2.zero - vertex).normalized;

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
            Vector2 newLt = Vector2.zero - newRt;

            spriteShape.spline.SetLeftTangent(i, newLt);
            spriteShape.spline.SetRightTangent(i, newRt);

            
        }
    }

    //for use with growing and shrinking mechanic
    private void GetJoints() 
    {
        foreach (var point in points) 
        {
            foreach (SpringJoint2D spring in point.gameObject.GetComponents<SpringJoint2D>())
            {
                Debug.Log($"spring connected body = {spring.connectedBody} && gameobject = {gameObject}" );
                if (spring.connectedBody.gameObject  == gameObject)
                {
                    innerSprings.Add(spring);
                }
                else outerSprings.Add(spring);
            }
            outerRods.Add(point.gameObject.GetComponent<DistanceJoint2D>());
        }
    }

    private void MatchTargetScale()
    {
        targetScale = transform.localScale.x;
    }

    //this already has some dumbassery. Note, calling the vector conversion each time just to check 
    //this dumbassery is now outdated.
    //tfw we got new dumbassery
    private void UpdateSize() 
    {
        if (targetScale != transform.localScale.x) transform.localScale = new Vector3(targetScale, targetScale, targetScale);
    }
    //this is crazy. why no slow grow
    
    /*
    private void SlowGrow()
    {
        transform.localScale += new Vector3(transform.localScale.x + Time.deltaTime, transform.localScale.y + Time.deltaTime, transform.localScale.z + Time.deltaTime);
    }
    */
    /*
    private void GetInitSprings() 
    {
        innerDis = innerSprings[0].distance;
        outerDis = outerSprings[0].distance;
        frequency = outerSprings[0].frequency;
        damping = outerSprings[0].dampingRatio;
    }

    private void UpdateSprings() 
    {
        for(int i = 0; i < innerSprings.Count; i++)
        {
            innerSprings[i].frequency = frequency;
            innerSprings[i].dampingRatio = damping;
        }
        for(int i = 0;i < outerSprings.Count; i++)
        {
            outerSprings[i].frequency = frequency;
            outerSprings[i].dampingRatio = damping;
        }
    }
    */
    #endregion

}
