using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        // Point the object at the world origin (0,0,0)
        transform.LookAt(Vector3.zero);
    }
}
