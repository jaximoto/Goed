using UnityEngine;

public class Spikes : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            if(collision.gameObject.GetComponentInParent<SlimeController>())
            {
                collision.gameObject.GetComponentInParent<SlimeController>().SlimeDeath();
            }
        }
    }
}
