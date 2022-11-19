using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;

public class PlayerTeamController : NetworkBehaviour
{
    private PlayerHUD playerHUD;
    [SerializeField]
    private MeshRenderer playerMaterial;

    public TeamColor currentTeam = TeamColor.Neutral;
    public string currentTeamName = "Neutral";

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

    private void Start()
    {
        playerHUD = GameObject.FindGameObjectWithTag("PlayerHUD").GetComponent<PlayerHUD>();
        playerHUD.UpdatePlayerTeamColor(currentTeam.ToString());
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
        currentTeam = GetTeamColor(color);
    }

    [ObserversRpc]
    private void ChangeColor(Color color)
    {
        playerMaterial.material.color = color;
        currentTeamName = GetTeamColor(color).ToString();
        playerHUD.UpdatePlayerTeamColor(currentTeamName.ToString());
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

    private Color GetColorFromTeamColor(TeamColor team)
    {
        if (team.Equals(TeamColor.Red))
            return Color.red;
        else if (team.Equals(TeamColor.Blue))
            return Color.blue;
        else if (team.Equals(TeamColor.Yellow))
            return Color.yellow;
        else if (team.Equals(TeamColor.Green))
            return Color.green;
        else
            return Color.white;
    }
}
