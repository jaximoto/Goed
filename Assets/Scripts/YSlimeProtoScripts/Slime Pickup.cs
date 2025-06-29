using UnityEngine;

public class SlimePickup : MonoBehaviour
{
    public float slimeAmount, drainSpeed;
    public bool sliming;
    SlimeController _slimeController;

    void Awake()
    {
        _slimeController = GameObject.FindAnyObjectByType<SlimeController>(); 
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == 6) sliming = true; 
    }


    private void Update()
    {
        if (sliming) slimeAmount = _slimeController.AddSlime(slimeAmount, drainSpeed); 
    }

    void OnTriggerExit2D()
    {
        
        sliming = false;
    }

    
}
