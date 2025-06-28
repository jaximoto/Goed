using UnityEngine;

public class SlimeParticleSysController : MonoBehaviour
{
    SlimeController _slimeController;
    ParticleSystem _particleSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _slimeController = gameObject.transform.parent.GetComponent<SlimeController>();
        _particleSystem = gameObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        SpawnerToGround();
    }

    //-----------------------SLIME WALK PARTICLES--------------------

    void SpawnerToGround()
    {
        if (_slimeController._grounded)
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _particleSystem.transform.position = _slimeController.transform.position + (Vector3.down * gameObject.transform.parent.localScale.x);
            _particleSystem.transform.up = Vector3.up; 
        }
    }






    //-----------------------SLIME IMPACT PARTICLES--------------------









}
