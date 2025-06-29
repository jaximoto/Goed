using UnityEngine;

public class JewelController : MonoBehaviour
{
    public SlimeController _slimeController;
    Quaternion target;
    float smooth = 1f;
    // Update is called once per frame
    void Update()
    {
        MoveToCore();
        RotateJewel();
    }

    void LateUpdate()
    {
        MoveToCore();
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
