using Cinemachine;
using FishNet.Object;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    public string Name { get; }
    public string _name = "Default Weapon";
    public float Damage { get; set; }
    public float _damage = 5f;
    public float Range { get; }
    public float _range = 15f;
    public int MaxAmmoCapacity { get; }
    public int maxAmmoCapacity = 30;
    public int MaxMagAmmoCapacity { get; }
    public int maxMagAmmoCapacity = 5;
    public int CurrentAmmo { get; }
    public int _currentAmmo = 30;
    public int CurrentMagAmmo { get; }
    public int _currentMagAmmo;
    public float FireRate { get; }
    public float _fireRate = 1f;
    public float FireRateTimer { get; }
    public float _fireRateTimer = 0f;
    public bool HasAmmo { get; }
    private bool _hasAmmo = true;
    public bool IsEmptyClip { get; }
    private bool _isEmptyClip = false;
    public bool IsEmptyWeapon { get; }
    private bool _isEmptyWeapon = false;
    public WeaponFireType FireType { get; }
    public WeaponFireType _fireType;


    /// <summary>
    /// Returns the appropriate firing input depending on WeaponFireType.
    /// </summary>
    /// <param name="player"></param>
    public virtual void FireWeapon(PlayerController player)
    {
        if (_fireType.Equals(WeaponFireType.Automatic))
        {
            if (Input.GetKey(KeyCode.F))
            {
                Debug.Log("Firing automatic weapon");
                FireRateHandler(player.vCam);
            }
            else
            {
                Debug.Log("Stopped firing!");
                _fireRateTimer = 0f;
            }
        }
        else if (_fireType.Equals(WeaponFireType.SemiAutomatic))
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                FireRateHandler(player.vCam);
            }
            else
                _fireRateTimer = 0f;
        }
        else if (_fireType.Equals(WeaponFireType.Burst))
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                FireRateHandler(player.vCam);
            }
            else
                _fireRateTimer = 0f;
        }
    }

    /// <summary>
    /// Handles the fire rate cooldown of the weapon
    /// </summary>
    /// <param name="cam"></param>
    private void FireRateHandler(CinemachineVirtualCamera cam)
    {
        if (_fireRateTimer <= 0)
        {
            Debug.Log("Raycast/Bullet fired!");
            ActivateRaycast(cam);
            _fireRateTimer = _fireRate;
            _currentAmmo -= 1;
        }
        else
        {
            Debug.Log("Raycast on Cooldown");
            _fireRateTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Fires weapon raycast to detect targets
    /// </summary>
    /// <param name="cam"></param>
    private void ActivateRaycast(CinemachineVirtualCamera cam)
    {
        Vector3 origin = cam.transform.position;
        Vector3 direction = cam.transform.forward;
        RaycastHit hit;
        Debug.Log("Firing raycast!");
        if (Physics.Raycast(origin, direction, out hit, _range))
        {
            Debug.Log("Hit Player");
            Debug.DrawRay(origin, direction * _range, Color.red);
            if (hit.collider.tag.Equals("Player"))
            {
                Debug.Log("Hit: " + hit.collider.name);
                PlayerController target = hit.collider.gameObject.GetComponentInParent<PlayerController>();
                ApplyDamage(target);
            }
        }
    }

    /// <summary>
    /// Applies weapon damage to targets
    /// </summary>
    /// <param name="target"></param>
    [ServerRpc]
    private void ApplyDamage(PlayerController target)
    {
        target.currentHealth -= _damage;
        Debug.Log(-_damage + " points of damage applied to " + target.name);
    }

}
