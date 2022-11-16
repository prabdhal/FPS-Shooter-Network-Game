using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera _camera;

    void Update()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
            return;
        }

        transform.LookAt(_camera.transform);
    }
}
