using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] Camera _camera;

    void Update()
    {
        transform.LookAt(_camera.transform);
    }
}
