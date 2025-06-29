using UnityEngine;
using UnityEngine.Android;

public class ExitManager : MonoBehaviour
{

    Animator anim;
    
    void Awake()
    {
          anim = GetComponent<Animator>();
          
    }
 

    public void EndLevel()
    {
        anim.Play("ExitAnim");
    }

}
