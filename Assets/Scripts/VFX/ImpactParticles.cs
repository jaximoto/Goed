using UnityEngine;

public class ImpactParticles : MonoBehaviour
{
    SlimeController _slimeController;
    Rigidbody2D rb;
    ParticleSystem _particleSystem;
    Vector2 _velocity;
    bool grounded;
    void Start()
    {
        _slimeController = gameObject.transform.parent.GetComponent<SlimeController>();
        rb = _slimeController.GetComponent<Rigidbody2D>();
        _particleSystem = gameObject.GetComponent<ParticleSystem>();
        grounded = _slimeController._grounded;

    }

    // Update is called once per frame
    void Update()
    {
        SpawnOnVel();

    }

    

    void SpawnOnVel()
    {
        var emission = _particleSystem.emission;
        float diff = rb.linearVelocity.magnitude - _velocity.magnitude;
        if (Mathf.Abs(diff) >= 5)
        {
            Debug.Log($"if and diff is {diff}");
            _particleSystem.Play();
            if(Mathf.Abs(diff) < 18)
                SoundManager.PlayRandomSoundPitch(SoundType.COLLIDE, .05f, true);
        }
        else
        {
            Debug.Log($"else and diff is {diff}");
            
        }
        _velocity = rb.linearVelocity;
    }
}
