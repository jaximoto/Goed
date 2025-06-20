using UnityEngine;
using UnityEngine.U2D;

public class SoftBodyController : MonoBehaviour
{

    #region fields
    [SerializeField]
    public SpriteShapeController spriteShape;
    [SerializeField]
    public Transform[] points;

    [SerializeField]
    private SpringJoint2D[] outerSprings;
    [SerializeField]
    private DistanceJoint2D outerRods;
    [SerializeField]
    private SpringJoint2D[] innerSprings;
    [SerializeField]
    private DistanceJoint2D[] innerRods;
    #endregion

    #region callbacks
    #endregion

    #region privateMethods
    private void UpdateVertices() {
        for (int i = 0; i < points.Length; i++) {
            Vector2 vertex = points[i].localPosition;
            Vector2 towardsCenter = (Vector2.zero - vertex).normalized;

            //when softbody controller is setup pull radius from there, this is stupid
            float colliderRadius = points[i].gameObject.GetComponent<CircleCollider2D>().radius;
            spriteShape.spline.SetPosition(i, (vertex - towardsCenter * colliderRadius));
        }
    }

    //for use with growing and shrinking mechanic
    private void GetJoints() 
    {
        foreach (var point in points) 
        {
            foreach (SpringJoint2D spring in point.gameObject.GetComponents<SpringJoint2D>())
            {
                

            }
        }
    }
    private void UpdateDistance() { } 
    #endregion

}
