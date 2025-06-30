using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.SceneManagement;

public class JewelController : MonoBehaviour
{
    public SlimeController _slimeController;
    Quaternion target;
    float smooth = 1f;
    bool slimeConnected, RDown;
    Rigidbody2D rb;
    Vector2 deathVel, deltaDir;
    float lastX, lastY;

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
            deathVel = _slimeController.GetComponent<Rigidbody2D>().linearVelocity;
            deathAngVel = _slimeController.GetComponent<Rigidbody2D>().angularVelocity;
            Debug.Log($"deathVel = {deathVel} deathAngVel = {deathVel.x} * {angMult} = {deathAngVel}, deltaDir.x = {deltaDir.x}");
            rb.AddForce(deathVel, ForceMode2D.Impulse);
            rb.AddTorque(deathAngVel, ForceMode2D.Impulse);
        }
        if (slimeConnected)
        {
            MoveToCore();
            RotateJewel();
        }
        RDown = Input.GetKeyDown("r");
        if (_slimeController.deBone && !_slimeController.levelEnding)
        {
            RestartLevel();
        }

        
    }
    
    //move Text Display over here -------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "pickup")
        {
            Reslime();
        }
    }
    void Reslime()
    {
        Debug.Log("Resliming");
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

    void RestartLevel()
    {
        Debug.Log($"press r? RDown = {RDown}");
        if (RDown) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

}
