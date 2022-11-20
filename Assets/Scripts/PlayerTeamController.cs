using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerTeamController : NetworkBehaviour
{
    private PlayerHUD playerHUD;
    [SerializeField]
    private MeshRenderer meshRenderer;

    [SyncVar(OnChange = nameof(OnChangeColor))]
    public int currentTeam = (int)TeamColor.Neutral;

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
            playerHUD = GameObject.FindGameObjectWithTag("PlayerHUD").GetComponent<PlayerHUD>();
            playerHUD.UpdatePlayerTeamColor(currentTeamName.ToString());
            ApplyColorChange(GetColorFromTeamColor((TeamColor)currentTeam));
        }
        else
        {
            GetComponent<PlayerTeamController>().enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7))
            SelectTeam(teamRed);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            SelectTeam(teamBlue);
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            SelectTeam(teamYellow);
        else if (Input.GetKeyDown(KeyCode.Alpha0))
            SelectTeam(teamGreen);
    }

    [ServerRpc]
    private void SelectTeam(Color color)
    {
        currentTeam = (int)GetTeamColor(color);
    }

    private void OnChangeColor(int prev, int next, bool isServer)
    {
        Color color = GetColorFromTeamColor((TeamColor)next); 
        ApplyColorChange(color);
    }

    private void ApplyColorChange(Color color)
    {
        meshRenderer.material.color = color;
        currentTeamName = GetTeamColor(color).ToString();
        if (playerHUD != null)
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
