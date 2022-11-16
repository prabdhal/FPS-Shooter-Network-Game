using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void Awake()
    {
        FirstObjectNotifier.onFirstObjectSpawned += FirstObjectNotifier_onFirstObjectSpawned;
    }

    private void FirstObjectNotifier_onFirstObjectSpawned(Transform obj)
    {
        CinemachineVirtualCamera vc = GetComponent<CinemachineVirtualCamera>();
        vc.Follow = obj;
        if (obj.tag.Equals("Player"))
        {
            obj.GetComponent<PlayerController>().vCam = vc;
        }
    }
}
