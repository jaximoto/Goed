using UnityEngine;

public class JointChecker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Update()
    {
        Debug.Log($"joint force is {gameObject.GetComponents<SpringJoint2D>()[0].reactionForce}");
    }
    private void OnJointBreak2D(Joint2D joint)
    {
        Debug.Log($"joint broke with force {joint.reactionForce}");
        
    }
}
