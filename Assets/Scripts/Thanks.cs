using UnityEngine;

public class Thanks : MonoBehaviour
{
    GameObject thanks;
    private void OnTriggerEnter2D()
    {
        thanks.SetActive(true);
    }
}
