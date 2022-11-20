using FishNet.Managing.Server;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class GlobalHUD : NetworkBehaviour
{
    [Header("Global Messaging Window")]
    public GameObject _globalMessagingScrollView;
    public TextMeshProUGUI _killTextPrefab;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {

        }
        else
        {
            GetComponent<GlobalHUD>().enabled = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateGlobalMessagingWindow(string player, string target)
    {
        GameObject go = Instantiate(_killTextPrefab.gameObject, _globalMessagingScrollView.transform);
        ServerManager.Spawn(go);
        UpdateMessage(go, player, target);

    }

    [ObserversRpc]
    public void UpdateMessage(GameObject go, string player, string target)
    {
        TextMeshProUGUI textPro = go.GetComponent<TextMeshProUGUI>();
        go.transform.SetParent(_globalMessagingScrollView.transform, false);
        textPro.text = player + " killed " + target;
    }
}
