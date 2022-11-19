using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : NetworkBehaviour
{
    public SceneFader sceneFader;

    public Image crosshairImage;
    
    [Header("Texts")]
    public TextMeshProUGUI _playerTeamColor;
    public TextMeshProUGUI _playerHealthText;
    public TextMeshProUGUI _activeWeaponText;
    public TextMeshProUGUI _ammoText;
    public TextMeshProUGUI _feedbackText;

    [Header("Global Messaging Window")]    
    public GameObject _globalMessagingScrollView;
    public TextMeshProUGUI _killTextPrefab;


    private void Start()
    {
        _feedbackText.gameObject.SetActive(false);
    }

    private void Update()
    {
    }

    public void UpdatePlayerTeamColor(string teamColor)
    {
        _playerTeamColor.text = "Team: " + teamColor;
    }

    public void UpdateActiveWeapon(string weaponName)
    {
        _activeWeaponText.text = "Weapon: " + weaponName;
    }

    public void UpdateAmmo(int ammo)
    {
        _ammoText.text = "Ammo: " + ammo.ToString();
    }

    public void UpdateAmmo(string curMagAmmo, string curAmmo)
    {
        _ammoText.text = curMagAmmo + " / " + curAmmo;
    }

    public void UpdatePlayerHealth(float health)
    {
        _playerHealthText.text = "Player HP: " + health.ToString(); 
    }

    public void UpdateFeedbackText(string feedback)
    {
        _feedbackText.text = feedback;
        _feedbackText.gameObject.SetActive(true);
    }

    public void DisableFeedbackText()
    {
        _feedbackText.gameObject.SetActive(false);
        _feedbackText.text = "";
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
