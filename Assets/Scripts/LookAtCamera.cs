using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] Camera _camera;


    private void Update()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
            return;
        }

        //transform.LookAt(_camera.transform);
    }
}
