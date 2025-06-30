using UnityEngine;

public class Spikes : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 6)
        {
            if(col.gameObject.GetComponentInParent<SlimeController>())
            {
                col.gameObject.GetComponentInParent<SlimeController>().SlimeDeath();
                col.gameObject.GetComponentInParent<SlimeController>().poked = true;
            }
        }
    }
}
