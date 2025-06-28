using UnityEngine;
using UnityEngine.Android;

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
        var emission = _particleSystem.emission;
        if (_slimeController._grounded)
        {
            RaycastHit2D hit = Physics2D.Raycast(_slimeController.transform.position, Vector3.down, 1f, ~_slimeController.groundCheckIgnoreLayers.value);
            if (hit)
            {
                emission.enabled = true;
                _particleSystem.transform.position = hit.point;
                _particleSystem.transform.up = Vector3.up;
            }   
        }
        else 
        {
            emission.enabled = false;
        }
    }






    //-----------------------SLIME IMPACT PARTICLES--------------------









}
