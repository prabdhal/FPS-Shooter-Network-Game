using Cinemachine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using TMPro;
using UnityEngine;


public class PlayerController : NetworkBehaviour
{
    [Header("Camera References")]
    public Camera cam;
    public CinemachineVirtualCamera vCam;
    public CinemachineBrain brainCam;
    public AudioListener audioListener;

    [Header("Player Component References")]
    private CharacterController characterController;
    public Transform playerModel;

    [Header("Other References")]
    public PlayerHUD playerHUD;
    public GlobalHUD globalHUD;
    public PlayerTeamController playerTeam;

    [Header("Move Value")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    [Header("Look Values")]
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public float playerRot = 5.0f;

    [Header("Stats")]
    public float maxHealth = 100f;
    [SyncVar(Channel = Channel.Reliable, OnChange = nameof(UpdateHealthHUD))]
    public float currentHealth = 100f;

    [Header("Weapon")]
    //private GameObject projectilePrefab;
    public Weapon activeWeapon;
    public Weapon defaultWeapon;
    public Weapon pistol;
    public Weapon submachine;
    public bool isFiring = false;

    private Vector3 moveDirection = Vector3.zero;

    [HideInInspector] public bool canMove = true;
    public bool IsDead { get { return isDead; } }
    private bool isDead = false;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            vCam.enabled = true;
            brainCam.enabled = true;
            cam.enabled = true;
            audioListener.enabled = true;
            gameObject.name = Random.Range(000, 999).ToString();
        }
        else
        {
            gameObject.GetComponent<PlayerController>().enabled = false;
        }
    }

    private void Start()
    {
        characterController = GetComponentInChildren<CharacterController>();
        playerTeam = GetComponent<PlayerTeamController>();
        currentHealth = maxHealth;
        playerHUD = GameObject.FindGameObjectWithTag("PlayerHUD").GetComponent<PlayerHUD>();
        globalHUD = GameObject.FindGameObjectWithTag("GlobalHUD").GetComponent<GlobalHUD>();
        playerHUD.UpdatePlayerHealth(currentHealth);
        if (activeWeapon != null)
        {
            playerHUD.UpdateActiveWeapon(activeWeapon.Name);
            playerHUD.UpdateAmmo(activeWeapon.CurrentAmmo);
        }
        else
        {
            playerHUD.UpdateActiveWeapon(defaultWeapon.Name);
            playerHUD.UpdateAmmo("N", "A");
        }

        // Lock cursor
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    private void Update()
    {
        GlobalGameData.Instance.PrintKillLogs();

        if (vCam == null) return;

        if (currentHealth <= 0 && isDead == false)
        {
            Death();
        }
        DeathHandler();

        bool isRunning = false;

        // Press Left Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // We are grounded, so recalculate move direction based on axis
        Vector3 forward = vCam.transform.TransformDirection(Vector3.forward);
        Vector3 right = vCam.transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        RotationHandler();
        JumpHandler(movementDirectionY);

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        FireWeaponHandler();
        WeaponSwapHandler();
    }

    /// <summary>
    /// Rotate player to look direction
    /// </summary>
    private void RotationHandler()
    {
        var camForward = vCam.transform.forward;
        camForward.y = 0;
        Quaternion targetDir = Quaternion.LookRotation(camForward);
        playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetDir, playerRot);
    }

    /// <summary>
    /// Controls player jump 
    /// </summary>
    private void JumpHandler(float moveDirY)
    {
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = moveDirY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// Checks for active weapon to fire, otherwise player will use melee attack only
    /// </summary>
    private void FireWeaponHandler()
    {
        if (activeWeapon != null)
            FireWeapon(activeWeapon, this);
        else
            FireWeapon(defaultWeapon, this);
    }

    /// <summary>
    /// Runs the FireWeapon method on the weapon base class
    /// </summary>
    private void FireWeapon(Weapon weapon, PlayerController player)
    {
        weapon.WeaponUpdate(this);
    }

    /// <summary>
    /// Handler player weapon equipping
    /// </summary>
    private void WeaponSwapHandler()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (activeWeapon != null)
                activeWeapon.CancelReload(this);
            activeWeapon = null;
            playerHUD.UpdateActiveWeapon(defaultWeapon.Name);
            playerHUD.UpdateAmmo("N", "A");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (activeWeapon != null)
                activeWeapon.CancelReload(this);
            activeWeapon = pistol.GetComponent<Weapon>();
            playerHUD.UpdateActiveWeapon(activeWeapon.Name);
            playerHUD.UpdateAmmo(activeWeapon.CurrentMagAmmo.ToString(), activeWeapon.CurrentAmmo.ToString());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (activeWeapon != null)
                activeWeapon.CancelReload(this);
            activeWeapon = submachine.GetComponent<Weapon>();
            playerHUD.UpdateActiveWeapon(activeWeapon.Name);
            playerHUD.UpdateAmmo(activeWeapon.CurrentMagAmmo.ToString(), activeWeapon.CurrentAmmo.ToString());
        }
    }

    //[ServerRpc]
    //private void InstantiateProjectile()
    //{
    //    GameObject go = Instantiate(projectilePrefab, vCam.transform.position, vCam.transform.rotation);
    //    ServerManager.Spawn(go);
    //    SetSpawnObject(go);
    //}

    [ObserversRpc]
    private void SetSpawnObject(GameObject go)
    {
        Projectile proj = go.GetComponent<Projectile>();
        proj.Init(this, 25f, 2f);
    }
    
    private void UpdateHealthHUD(float prev, float next, bool asServer)
    {
        if (playerHUD != null)
            playerHUD.UpdatePlayerHealth(next);
    }

    private void Death()
    {
        //Debug.Log("Player " + gameObject.name + " is dead!");
        //Debug.Log("Killed By: " + killedByPlayer.name + "!");
        //playerHUD.UpdateGlobalMessagingWindow(gameObject.name.ToString(), killedByPlayer.name.ToString());
        isDead = true;
        playerHUD.sceneFader.gameObject.SetActive(true);
    }

    public void DeathHandler()
    {
        Debug.Log("Death Handler");
    }
}
