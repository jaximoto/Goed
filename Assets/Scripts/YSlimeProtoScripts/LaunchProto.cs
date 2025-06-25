using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaunchProto : MonoBehaviour
{
    public bool launchHeld = false;
    public bool releasingLaunch = false;
    public float x, y;
    public bool deBone = false;
    public Vector2 launchVector;
    public float chargeDelta;
    public float chargeMax;
    public float currCharge = 0;


    public List<GameObject> points;
    void Awake()
    {
        points = new List<GameObject>();
        // exclude last child which doesnt have rigidbody
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            if (transform.GetChild(i).TryGetComponent<Rigidbody2D>(out _))
                points.Add(transform.GetChild(i).gameObject);
        }
    }

    void Update()
    {
        if (deBone)
        {
            DeBone();
            deBone = false;

        }
        GatherInput();
        ChargeLaunch();

    }

    void GatherInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");

        
        if (x != 0 || y != 0)
        {
            launchHeld = true;
        }
        else if (launchHeld && x == 0 && y == 0)
        {
            launchHeld = false;
            releasingLaunch = true;
        }
    }

    void ChargeLaunch()
    {
        if (launchHeld && !releasingLaunch)
        {
            Vector2 moveDir = new Vector2(x, y).normalized;
            currCharge += chargeDelta * Time.deltaTime;
            Debug.Log($"adding force = {moveDir * currCharge}");
            gameObject.GetComponent<Rigidbody2D>().AddForce(moveDir * currCharge);
            
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].GetComponents<SpringJoint2D>()[0].frequency != 0.5f)
                {
                    points[i].GetComponents<SpringJoint2D>()[0].frequency = 0.5f;
                }
            }
        }
        else if (releasingLaunch)
        {
            currCharge = 0;
            for (int i = 0; i < points.Count; i++)
            {
                points[i].GetComponents<SpringJoint2D>()[0].frequency = 100;
                deBone = true;
                releasingLaunch = false;
            }
        }

    }


    void OnDrawGizmos()
    {
        //Gather Component Vectors
        if(launchHeld)
        {
            //Debug.Log($"");
            Gizmos.color = Color.green;
            Gizmos.DrawLine(gameObject.transform.position, 
                new Vector3(gameObject.transform.position.x - x, gameObject.transform.position.y - y, gameObject.transform.position.z));
        }
    }

    void DeBone()
    {
        for(int i = 0; i < points.Count; i++) 
        {
            points[i].GetComponents<SpringJoint2D>()[0].enabled = false;
        }
        gameObject.transform.DetachChildren();
    }
    
}
