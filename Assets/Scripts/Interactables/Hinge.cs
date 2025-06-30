using UnityEngine;

public class Hinge : MonoBehaviour
{
    public GameObject lDoor, rDoor;
    public bool opening;
    public float openingSpeed;
    float lTarget, rTarget;

    private void Awake()
    {
        lTarget = lDoor.transform.position.x - 2;
        rTarget = rDoor.transform.position.x + 2;
    }

    private void Update()
    {
        if (opening) Open();

    }

    public void Open()
    {
        if(lDoor.transform.position.x > lTarget)
        {
            lDoor.transform.position += Vector3.left * openingSpeed * Time.deltaTime;
        }
        if(rDoor.transform.position.x < rTarget)
        {
            rDoor.transform.position += Vector3.right * openingSpeed * Time.deltaTime;
        }
    }
}
