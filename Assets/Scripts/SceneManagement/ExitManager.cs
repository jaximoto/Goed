using UnityEngine;
using UnityEngine.Android;

public class ExitManager : MonoBehaviour
{
    //Squash and Stretch 
    public float targetYScale, scaleSmooth;
    private float yScale;

    private bool exiting, shrinking;

    void Awake()
    {
        yScale = transform.localScale.y;    
    }

    void Update()
    {
        SquashAndStretch();
    }
    [ContextMenu("SquashAndStretch"), ExecuteInEditMode]
    void SquashAndStretch() 
    {
        if(transform.localScale.y > targetYScale) 
        {
            transform.localScale -= new Vector3(0f, scaleSmooth * Time.deltaTime, 0f);
            transform.position -= new Vector3(0f, scaleSmooth * Time.deltaTime, 0f);
        }
        else if (transform.localScale.y <= targetYScale)
        {
            targetYScale = yScale;
            transform.localScale += new Vector3(0f, scaleSmooth * Time.deltaTime, 0f);
            transform.position += new Vector3(0f, scaleSmooth * Time.deltaTime, 0f);
        }
    }

    void LoadNextScene()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!exiting && collision.gameObject.TryGetComponent<SlimeController>(out SlimeController _sc)) 
        {
            //_sc.  some exit behavior
            exiting = true;
        }
    }
}
