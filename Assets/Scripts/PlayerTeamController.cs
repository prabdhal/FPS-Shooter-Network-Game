using FishNet.Object;
using UnityEngine;

public class PlayerTeamController : NetworkBehaviour
{
    [SerializeField]
    private MeshRenderer playerMaterial;

    public TeamColor currentTeam;

    [SerializeField]
    private Color teamRed = Color.red;
    [SerializeField]
    private Color teamBlue = Color.blue;
    [SerializeField]
    private Color teamYellow = Color.yellow;
    [SerializeField]
    private Color teamGreen = Color.green;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {

        }
        else
        {
            gameObject.GetComponent<PlayerTeamController>().enabled = false;
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SelectTeam(teamRed);
            Debug.Log(gameObject.name + " joined Team Red");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SelectTeam(teamBlue);
            Debug.Log(gameObject.name + " joined Team Blue");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SelectTeam(teamYellow);
            Debug.Log(gameObject.name + " joined Team Yellow");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SelectTeam(teamGreen);
            Debug.Log(gameObject.name + " joined Team Green");
        }
    }

    [ServerRpc]
    private void SelectTeam(Color color)
    {
        ChangeColor(color);
    }

    [ObserversRpc]
    private void ChangeColor(Color color)
    {
        playerMaterial.material.color = color;
        currentTeam = GetTeamColor(color);
    }

    private TeamColor GetTeamColor(Color color)
    {
        if (color.Equals(teamRed))
            return TeamColor.Red;
        else if (color.Equals(teamBlue))
            return TeamColor.Blue;
        else if (color.Equals(teamYellow))
            return TeamColor.Yellow;
        else if (color.Equals(teamGreen))
            return TeamColor.Green;
        else
            return TeamColor.Neutral;
    }
}
