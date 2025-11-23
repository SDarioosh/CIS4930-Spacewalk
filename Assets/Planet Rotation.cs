using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 50, 0); // Degrees per second

    void Update()
    {
        // Rotate around the specified axes
        transform.Rotate(rotationSpeed * Time.deltaTime); 
    }
}