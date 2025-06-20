using UnityEngine;

public class SlimeController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float rolltiplier;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float impulse = Input.GetAxisRaw("Horizontal") * Time.deltaTime;
        rb.AddTorque(impulse * -1 * rolltiplier, ForceMode2D.Impulse);
    }

}
