using UnityEngine;

public class JewelController : MonoBehaviour
{
    public SlimeController _slimeController;
    Quaternion target;
    float smooth = 1f;
    bool slimeConnected;
    Rigidbody2D rb;
    Vector2 deathVel;
    float deathAngVel;
    public float angMult, linMult;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }
    void Update()
    {
        slimeConnected = !_slimeController.deBone;
        if (!slimeConnected && !rb.simulated)
        {
            rb.simulated = true;
            deathVel = _slimeController.GetComponent<Rigidbody2D>().linearVelocity * linMult;
            deathAngVel = _slimeController.GetComponent<Rigidbody2D>().angularVelocity * angMult;
            rb.AddForce(deathVel, ForceMode2D.Impulse);
            rb.AddTorque(deathAngVel, ForceMode2D.Impulse);
        }
        if (slimeConnected)
        {
            MoveToCore();
            RotateJewel();
        }
    }

    void LateUpdate()
    {
        slimeConnected = !_slimeController.deBone;
        if (slimeConnected) MoveToCore();
    
    }


    void MoveToCore()
    {
        transform.position = _slimeController.transform.position;
    }

    void RotateJewel()
    {
        float xVel = _slimeController.GetComponent<Rigidbody2D>().linearVelocity.x;
        if (Mathf.Abs(xVel) > 0.5f)
        {
            if(xVel > 0)
            {
                //rotate to the left
                target = Quaternion.Euler(0,0,90);
            }
            else
            {
                target = Quaternion.Euler(0, 0, -90);
                //rotate to the right
            }
        }
        else
        {
            target = Quaternion.Euler(0, 0, 0);
            //rotate to middle
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
    }


}
