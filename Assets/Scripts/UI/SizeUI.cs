using UnityEngine;

public class SizeUI : MonoBehaviour
{
    public SlimeController _slimeController;
    public Transform circle;
    float proportion;
    Vector3 startSize;

    void Awake()
    {
        startSize = circle.localScale;   
    }

    void Update()
    {
        updateScale();
    }

    void updateScale()
    {
        if (proportion != _slimeController._proportion)
        {
            //Debug.Log($"proportion = {proportion}");
            proportion = _slimeController._proportion;
            circle.localScale = proportion * startSize;
        }
    }






}
