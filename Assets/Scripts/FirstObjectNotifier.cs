using FishNet.Object;
using System;
using UnityEngine;

public class FirstObjectNotifier : NetworkBehaviour
{
    public static event Action<Transform> onFirstObjectSpawned;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            NetworkObject nob = base.LocalConnection.FirstObject;
            if (nob == base.NetworkObject)
            {
                onFirstObjectSpawned?.Invoke(transform);
                Debug.Log("Invoked");
            }
        }
    }
}
