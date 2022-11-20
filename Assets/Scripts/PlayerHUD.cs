using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public SceneFader sceneFader;

    public Image crosshairImage;

    [Header("Texts")]
    public TextMeshProUGUI _playerTeamColor;
    public TextMeshProUGUI _playerHealthText;
    public TextMeshProUGUI _activeWeaponText;
    public TextMeshProUGUI _ammoText;
    public TextMeshProUGUI _feedbackText;



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
}
