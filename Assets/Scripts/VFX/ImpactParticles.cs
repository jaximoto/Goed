using UnityEngine;

public class ImpactParticles : MonoBehaviour
{
    SlimeController _slimeController;
    Rigidbody2D rb;
    ParticleSystem _particleSystem;
    Vector2 _velocity;
    public float emitMin, emitMax;
    void Start()
    {
        _slimeController = gameObject.transform.parent.GetComponent<SlimeController>();
        rb = _slimeController.GetComponent<Rigidbody2D>();
        _particleSystem = gameObject.GetComponent<ParticleSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        SpawnOnVel();

    }

    //-----------------------SLIME WALK PARTICLES--------------------

    void SpawnOnVel()
    {
        var emission = _particleSystem.emission;
        float diff = rb.linearVelocity.magnitude - _velocity.magnitude;
        if (Mathf.Abs(diff) >= 5)
        {
            Debug.Log($"if and diff is {diff}");
            _particleSystem.Play();
        }
        else
        {
            Debug.Log($"else and diff is {diff}");
            
        }
        _velocity = rb.linearVelocity;
    }
}
