using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Image crosshair;
    public TextMeshProUGUI _playerHealth;
    public TextMeshProUGUI _activeWeapon;
    public TextMeshProUGUI _ammo;

    public void UpdateActiveWeapon(string weaponName)
    {
        _activeWeapon.text = "Active Weapon: " + weaponName;
    }

    public void UpdateAmmo(int ammo)
    {
        _ammo.text = "Ammo: " + ammo.ToString();
    }

    public void UpdateAmmo(string ammo)
    {
        _ammo.text = "Ammo: " + ammo;
    }

    public void UpdatePlayerHealth(float health)
    {
        _playerHealth.text = "Player HP: " + health.ToString(); 
    }
}
