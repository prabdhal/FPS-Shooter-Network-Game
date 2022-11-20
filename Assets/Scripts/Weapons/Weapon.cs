using Cinemachine;
using FishNet.Object;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    // weapon values
    [Header("Weapon Values")]
    [SerializeField]
    protected string _name = "Default Weapon";
    public string Name { get { return _name; } }
    [SerializeField]
    protected float _damage = 5f;
    public float Damage { get { return _damage; } }
    [SerializeField]
    protected float _range = 15f;
    public float Range { get { return _range; } }
    [SerializeField]
    protected float _fireRate = 1f;
    public float FireRate { get { return _fireRate; } }
    [SerializeField]
    protected float _fireRateTimer = 0f;
    public float FireRateTimer { get { return _fireRateTimer; } }
    [SerializeField]
    protected float _reloadSpeed = 1f;
    public float ReloadSpeed { get { return _reloadSpeed; } }
    public float _reloadSpeedTimer = 0;


    // ammo values
    [Header("Ammo Values")]
    [SerializeField]
    protected int _maxAmmoCapacity = 30;
    public int MaxAmmoCapacity { get { return _maxAmmoCapacity; } }
    [SerializeField]
    protected int _maxMagAmmoCapacity = 5;
    public int MaxMagAmmoCapacity { get { return _maxMagAmmoCapacity; } }
    [SerializeField]
    protected int _startingAmmo = 15;
    public int StartingAmmo { get { return _startingAmmo; } }
    protected int _currentAmmo = 30;
    public int CurrentAmmo { get { return _currentAmmo; } }
    protected int _currentMagAmmo;
    public int CurrentMagAmmo { get { return _currentMagAmmo; } }

    // useful props
    public bool IsEmptyClip { get { return _currentMagAmmo <= 0 && !FireType.Equals(WeaponFireType.Melee); } }    
    public bool IsEmptyWeapon { get { return _currentAmmo <= 0 && _currentMagAmmo <= 0 && !FireType.Equals(WeaponFireType.Melee); } }
    protected bool _isReloading = false;
    public bool IsReloading { get { return _isReloading; } }
    [SerializeField]
    protected WeaponFireType _fireType;
    public WeaponFireType FireType { get { return _fireType; } }



    private void Start()
    {
        // ensures starting ammo and max mag ammo capacity are lower than max values
        if (_startingAmmo > _maxAmmoCapacity)
            _startingAmmo = _maxAmmoCapacity;
        if (_maxMagAmmoCapacity > _maxAmmoCapacity)
            _maxMagAmmoCapacity = _maxAmmoCapacity;


        _currentAmmo = _startingAmmo;
        _currentMagAmmo = _maxMagAmmoCapacity;
    }

    public void WeaponUpdate(PlayerController player)
    {
        FireWeapon(player);
        ReloadHander(player);
    }


    #region Weapon Firing Logic
    /// <summary>
    /// Returns the appropriate firing input depending on WeaponFireType.
    /// </summary>
    /// <param name="player"></param>
    protected virtual void FireWeapon(PlayerController player)
    {
        if (_fireType.Equals(WeaponFireType.Automatic))
        {
            if (Input.GetKey(KeyCode.F))
            {
                FireRateHandler(player);
            }
            else
                _fireRateTimer = 0f;
        }
        else if (_fireType.Equals(WeaponFireType.SemiAutomatic))
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                FireRateHandler(player);
            }
            else
                _fireRateTimer = 0f;
        }
        else if (_fireType.Equals(WeaponFireType.Burst))
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                FireRateHandler(player);
            }
            else
                _fireRateTimer = 0f;
        }
        else if (_fireType.Equals(WeaponFireType.Melee))
        {
            if (Input.GetKey(KeyCode.F))
            {
                FireRateHandler(player);
            }
            else
                _fireRateTimer = 0f;
        }
    }

    /// <summary>
    /// Handles the fire rate cooldown of the weapon
    /// </summary>
    /// <param name="cam"></param>
    private void FireRateHandler(PlayerController player)
    {
        // Checks if player has ammo
        if (IsEmptyClip || IsEmptyWeapon)
        {
            Debug.Log("Player needs to reload!");
            return;
        }

        if (_isReloading)
            CancelReload(player);

        if (_fireRateTimer <= 0)
        {
            Debug.Log("Raycast/Bullet fired!");
            ActivateRaycast(player);
            _fireRateTimer = _fireRate;
            
            // Update ammo ONLY if WeaponFireType is NOT Melee   
            if (!FireType.Equals(WeaponFireType.Melee))
            {
                _currentMagAmmo -= 1;
                player.playerHUD.UpdateAmmo(_currentMagAmmo.ToString(), _currentAmmo.ToString());
            }
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
    private void ActivateRaycast(PlayerController player)
    {
        Vector3 origin = player.vCam.transform.position;
        Vector3 direction = player.vCam.transform.forward;
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
                Debug.Log("target: " + target.name);
                ApplyDamage(player, target);
            }
        }
    }

    /// <summary>
    /// Applies weapon damage to targets
    /// </summary>
    /// <param name="target"></param>
    [ServerRpc]
    private void ApplyDamage(PlayerController player, PlayerController target)
    {
        Debug.Log("player team: " + player.playerTeam.currentTeam.Equals(target.playerTeam.currentTeam));
        if (!target.playerTeam.currentTeam.Equals(player.playerTeam.currentTeam))
        {
            target.currentHealth -= _damage;
            if (target.currentHealth <= 0)
            {
                Debug.Log("target: " + target);
                Debug.Log("Killed by: " + player);
                target.killedByPlayer = player;
                //target.playerHUD.UpdateGlobalMessagingWindow(player.name.ToString(), target.name.ToString());
            }
            Debug.Log(-_damage + " points of damage applied to " + target.name);
        }
        else
            Debug.Log("Cannot friendly fire!");
    }

    #endregion

    #region Reloading Logic
    /// <summary>
    /// Initials weapon reloading
    /// </summary>
    private void InitiateReload(PlayerController player)
    {
        if (Input.GetKeyDown(KeyCode.R) && !_isReloading)
        {
            // returns if no ammo
            if (IsEmptyWeapon)
            {
                Debug.Log("You cannot reload since you have no ammo!");
                return;
            }
            _reloadSpeedTimer = 0;
            _isReloading = true;
            player.playerHUD.UpdateFeedbackText("Reloading...");
        }
    }

    /// <summary>
    /// Handles reload cooldown
    /// </summary>
    protected virtual void ReloadHander(PlayerController player)
    {
        if (_currentMagAmmo < _maxMagAmmoCapacity)
            InitiateReload(player);

        // reload after reload time
        if (_reloadSpeedTimer >= _reloadSpeed)
        {
            Reload(player);
            _reloadSpeedTimer = 0;
            _isReloading = false;
            player.playerHUD.DisableFeedbackText();
        }
        else if (_isReloading)
        {
            Debug.Log("Reloading");
            _reloadSpeedTimer += Time.deltaTime;
        }
    }

    public virtual void CancelReload(PlayerController player)
    {
        _reloadSpeedTimer = 0;
        _isReloading = false;
        player.playerHUD.DisableFeedbackText();
    }

    /// <summary>
    /// Updates the ammo amount in the magizine and total ammo
    /// </summary>
    private void Reload(PlayerController player)
    {
        int ammoNeeded = _maxMagAmmoCapacity - _currentMagAmmo;
        if (ammoNeeded > _currentAmmo)
            ammoNeeded = _currentAmmo;
        
        _currentMagAmmo += ammoNeeded;
        
        _currentAmmo -= ammoNeeded;
        if (_currentAmmo <= 0) 
            _currentAmmo = 0;

        player.playerHUD.UpdateAmmo(_currentMagAmmo.ToString(), _currentAmmo.ToString());
    }
    #endregion 
}
