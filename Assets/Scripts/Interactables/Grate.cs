using UnityEngine;

public class Grate : MonoBehaviour
{
    public float grateLoss;
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            if (collision.gameObject.GetComponentInParent<SlimeController>())
            {
                collision.gameObject.GetComponentInParent<SlimeController>().GrateSlimeLoss(grateLoss);
                
            }
        }
    }
}
