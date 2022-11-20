using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameData : NetworkBehaviour
{
    #region Singleton
    public static GlobalGameData Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    #endregion

    public Stack<string> killLogs = new Stack<string>();


    public void AddKillLog(string log)
    {
        killLogs.Push(log);
        Debug.Log("Adding to kill logs: " + log);
    }

    public void ClearLog()
    {
        killLogs.Clear();
        Debug.Log("Cleared Kill Logs");
    }

    public void PrintKillLogs()
    {
        if (killLogs.Count <= 0)
            Debug.Log("No Kill Logs!");

        foreach (string log in killLogs)
        {
            Debug.Log(log + "\n");
        }
    }

}
