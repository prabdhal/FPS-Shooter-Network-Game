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
    public void UpdateGlobalMessagingWindow()
    {
        string recentLog = GlobalGameData.Instance.killLogs.Peek();
        GameObject go = Instantiate(_killTextPrefab.gameObject, _globalMessagingScrollView.transform);
        ServerManager.Spawn(go);
        UpdateMessage(go, recentLog);

    }

    [ObserversRpc]
    public void UpdateMessage(GameObject go, string log)
    {
        TextMeshProUGUI textPro = go.GetComponent<TextMeshProUGUI>();
        go.transform.SetParent(_globalMessagingScrollView.transform, false);
        textPro.text = log;
    }
}
