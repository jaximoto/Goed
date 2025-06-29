using UnityEngine;

public class EndLevelDetector : MonoBehaviour
{
    ExitManager em;

    private void Awake()
    {
        em = transform.parent.parent.GetComponent<ExitManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collision");
        if(collision.gameObject.layer == 6)
        {
            Debug.Log("slimeHit");
            em.EndLevel();
        }
    }
}
