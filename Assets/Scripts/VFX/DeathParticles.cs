using UnityEngine;
using System.Collections;
public class DeathParticles : MonoBehaviour
{
    SlimeController _sc;
    ParticleSystem _ps;
    bool die, stayDie;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _sc = GameObject.FindFirstObjectByType<SlimeController>();
        _ps = gameObject.GetComponent<ParticleSystem>();
        transform.localScale = _sc.transform.localScale;
        MoveToRay();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        die = _sc.deBone;
        if (die) Die();
        */
        //if (stayDie) 
        MoveToRay();
        if (!_ps.isPlaying) Destroy(gameObject);
    }

    void MoveToRay()
    {
        if (_sc.enabled)
        {
            RaycastHit2D hit = Physics2D.Raycast(_sc.transform.position, Vector3.down, 1f, ~_sc.groundCheckIgnoreLayers.value);
            if (hit)
            {
                _ps.transform.position = hit.point;
                _ps.transform.up = Vector3.up;
            }
        }
        
    }
    
}
