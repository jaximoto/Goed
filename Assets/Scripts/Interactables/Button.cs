using UnityEngine;

public class Button : MonoBehaviour
{
    bool pushin;
    float prop;
    float requiredWeight;
    public float springBackSpeed;
    float yStart;
    float targetY;
    private void Awake()
    {
        yStart = transform.position.y;
        targetY = yStart - 0.15f;
    }

    private void Update()
    {
        if(!pushin)
        {
            SpringBack();

        }
    }


    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.layer == 6)
        {
            if (col.gameObject.GetComponentInParent<SlimeController>())
            {
                prop = col.gameObject.GetComponentInParent<SlimeController>()._proportion;
                PushButton(prop);
            }
        }
        
    }



    void PushButton(float prop)
    {
        Debug.Log("Pushin");
        if (prop >= requiredWeight) 
        {
            pushin = true;
            if (transform.position.y > targetY)
            {
                transform.position -= new Vector3(0, prop * Time.deltaTime, 0);
            }
        }
 
        
    }

    void SpringBack()
    {
        if (transform.position.y <= yStart)
        {
            transform.position += new Vector3(0, springBackSpeed * Time.deltaTime, 0);
        }
    }        
}
