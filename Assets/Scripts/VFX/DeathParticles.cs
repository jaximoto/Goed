using UnityEngine;
using System.Collections;
public class DeathParticles : MonoBehaviour
{
    SlimeController _sc;
    ParticleSystem _ps;
    bool die, stayDie;
    public float deathTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sc = gameObject.transform.parent.GetComponent<SlimeController>();
        _ps = gameObject.GetComponent<ParticleSystem>();    
    }

    // Update is called once per frame
    void Update()
    {
        die = _sc.deBone;
        if (die) Die(); 
        //if (stayDie) 
    }

    //-----------------------SLIME WALK PARTICLES--------------------

    void Die()
    {
        var emission = _ps.emission;
        emission.enabled = true;
        RaycastHit2D hit = Physics2D.Raycast(_sc.transform.position, Vector3.down, 1f, ~_sc.groundCheckIgnoreLayers.value);
        if (hit)
        {
            
            _ps.transform.position = hit.point;
            _ps.transform.up = Vector3.up;
        }
        StopDying();
    }
    IEnumerator StopDying()
    {
        yield return new WaitForSeconds(deathTime);
        stayDie = true;
    }
}
