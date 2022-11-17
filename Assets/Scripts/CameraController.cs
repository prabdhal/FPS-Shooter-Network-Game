using Cinemachine;
using FishNet.Object;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void Awake()
    {
        FirstObjectNotifier.onFirstObjectSpawned += FirstObjectNotifier_onFirstObjectSpawned;
    }

    private void OnDestroy()
    {
        FirstObjectNotifier.onFirstObjectSpawned -= FirstObjectNotifier_onFirstObjectSpawned; 
    }

    private void FirstObjectNotifier_onFirstObjectSpawned(Transform obj)
    {
        CinemachineVirtualCamera vc = GetComponent<CinemachineVirtualCamera>();
        vc.Follow = obj;
        //obj.GetComponent<PlayerController>().vCam = vc;
    }

}
