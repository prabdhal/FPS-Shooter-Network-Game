using FishNet.Object;
using TMPro;
using UnityEngine;

public class ConsoleTextSpawn : NetworkBehaviour
{
    public Transform consoleLogUI;
    public GameObject consoleTextPf;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
        }
        else
        {
            GetComponent<ConsoleTextSpawn>().enabled = false;
        }
    }

    public void SpawnConsoleText(string logText)
    {
        Debug.Log("Instantiate console text: " + logText);
        TextMeshProUGUI log = Instantiate(consoleTextPf, consoleLogUI).GetComponent<TextMeshProUGUI>();
        log.text = logText;
    }
}
