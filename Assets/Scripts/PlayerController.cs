using Cinemachine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;


public class PlayerController : NetworkBehaviour
{
    public ConsoleTextSpawn consoleTextSpawn;
    [Header("Camera References")]
    public Camera cam;
    public CinemachineVirtualCamera vCam;
    public CinemachineBrain brainCam;
    public AudioListener audioListener;

    [Header("Player Component References")]
    private CharacterController characterController;
    private Animator anim;
    public Transform playerModel;

    [Header("Other References")]
    public PlayerHUD playerHUD;
    public GlobalHUD globalHUD;
    public PlayerTeamController playerTeam;

    [SyncVar(OnChange = nameof(OnChangeName))]
    public string playerName;

    [Header("Move Value")]
    [SerializeField]
    private float walkingSpeed = 7.5f;
    [SerializeField]
    private float runningSpeed = 11.5f;
    [SerializeField]
    private float jumpSpeed = 8.0f;
    [SerializeField]
    private float gravity = 20.0f;
    [SerializeField]
    private float xInput;
    [SerializeField]
    private float yInput;
    private bool isRunning = false;
    public bool IsRunning { get { return isRunning; } }

    [Header("Look Values")]
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public float playerRot = 5.0f;

    [Header("Stats")]
    public float maxHealth = 100f;
    [SyncVar(Channel = Channel.Reliable, OnChange = nameof(UpdateHealthHUD))]
    public float currentHealth = 100f;

    [Header("Weapon")]  // weapon scripts
    //private GameObject projectilePrefab;
    public Weapon activeWeapon;
    public Weapon defaultWeapon;
    public Weapon pistol;
    public Weapon submachine;
    public bool isFiring = false;

    [Header("Weapon GO")]   // weapon gameobjects on the player
    public GameObject pistolGO;
    public GameObject submachineGO;

    private Vector3 moveDirection = Vector3.zero;

    [HideInInspector] public bool canMove = true;

    [Header("Death")]
    [SerializeField]
    private Transform[] spawnPoints = new Transform[3];
    [SerializeField]
    private float spawnTimer = 5f;
    private float curSpawnTimer = 0;
    public bool IsDead { get { return isDead; } }
    private bool isDead = false;

    // Events
    public delegate void OnWeaponSwap(Weapon weapon);
    public event OnWeaponSwap OnWeaponSwapEvent;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            vCam.enabled = true;
            brainCam.enabled = true;
            cam.enabled = true;
            audioListener.enabled = true;
            GameObject[] spawns = GameObject.FindGameObjectsWithTag("SpawnPoint");
            spawnPoints = new Transform[spawns.Length];
            for (int i = 0; i < spawns.Length; i++)
            {
                spawnPoints[i] = spawns[i].transform;
            }
        }
        else
        {
            gameObject.GetComponent<PlayerController>().enabled = false;
        }
    }

    private void Start()
    {
        characterController = GetComponentInChildren<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        playerTeam = GetComponent<PlayerTeamController>();
        playerHUD = GameObject.FindGameObjectWithTag(StringData.PlayerHUDTag).GetComponent<PlayerHUD>();
        globalHUD = GameObject.FindGameObjectWithTag(StringData.GlobalHUDTag).GetComponent<GlobalHUD>();
        currentHealth = maxHealth;
        playerHUD.UpdatePlayerHealth(currentHealth);
        ApplyNameChange();

        consoleTextSpawn = GameObject.FindGameObjectWithTag(StringData.PlayerHUDTag).GetComponent<ConsoleTextSpawn>();
         
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
            curSpawnTimer = 0f;
            isDead = true;
            DeathServer();
        }
        DeathHandler();

        isRunning = false;

        // We are grounded, so recalculate move direction based on axis
        Vector3 forward = vCam.transform.TransformDirection(Vector3.forward);
        Vector3 right = vCam.transform.TransformDirection(Vector3.right);

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * xInput : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * yInput : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedY) + (right * curSpeedX);

        // Press Left Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift) && moveDirection.normalized.magnitude > 0f;

        RotationHandler();
        JumpHandler(movementDirectionY);
        AnimatorHandler();

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        FireWeaponHandler();
        WeaponSwapHandler();
    }


    private void OnChangeName(string prev, string next, bool isServer)
    {
        gameObject.name = playerName;
        playerTeam.UpdateName(playerName);
    }

    private void ApplyNameChange()
    {
        playerName = Random.Range(000, 999).ToString();
    }

    #region Player Animation Handler
    private void AnimatorHandler()
    {
        float moveDir = new Vector3(moveDirection.x, 0f, moveDirection.z).magnitude;
        anim.SetFloat(StringData.Speed, moveDir);
        anim.SetFloat(StringData.SpeedX, xInput);
        anim.SetFloat(StringData.SpeedY, yInput);
        anim.SetBool(StringData.IsRunning, IsRunning);
    }

    public void SetAnimBool(string value, bool fire)
    {
        anim.SetBool(value, fire);
    }

    public void PlayAnim(string animName, WeaponType type)
    {
        anim.Play(animName, (int)type);
    }

    public void SetWeaponAnimSpeed(float speed)
    {
        anim.speed = speed;
    }

    #endregion

    #region Player Movement Handlers

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
        if (Input.GetButton(StringData.Jump) && canMove && characterController.isGrounded)
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

    #endregion

    #region Player Weapon Handlers

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
            ServerSetWeaponGO(WeaponType.Melee);

            anim.SetLayerWeight((int)WeaponType.Pistol, 0);
            anim.SetLayerWeight((int)WeaponType.Submachine, 0);

            playerHUD.UpdateActiveWeapon(defaultWeapon.Name);
            playerHUD.UpdateAmmo("N", "A");

            OnWeaponSwapEvent?.Invoke(activeWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (activeWeapon != null)
                activeWeapon.CancelReload(this);

            activeWeapon = pistol.GetComponent<Weapon>();
            ServerSetWeaponGO(WeaponType.Pistol);

            anim.SetLayerWeight((int)WeaponType.Pistol, 1);
            anim.SetLayerWeight((int)WeaponType.Submachine, 0);

            playerHUD.UpdateActiveWeapon(activeWeapon.Name);
            playerHUD.UpdateAmmo(activeWeapon.CurrentMagAmmo.ToString(), activeWeapon.CurrentAmmo.ToString());

            OnWeaponSwapEvent?.Invoke(activeWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (activeWeapon != null)
                activeWeapon.CancelReload(this);

            activeWeapon = submachine.GetComponent<Weapon>();
            ServerSetWeaponGO(WeaponType.Submachine);

            anim.SetLayerWeight((int)WeaponType.Pistol, 0);
            anim.SetLayerWeight((int)WeaponType.Submachine, 1);

            playerHUD.UpdateActiveWeapon(activeWeapon.Name);
            playerHUD.UpdateAmmo(activeWeapon.CurrentMagAmmo.ToString(), activeWeapon.CurrentAmmo.ToString());

            OnWeaponSwapEvent?.Invoke(activeWeapon);
        }
    }

    [ServerRpc]
    private void ServerSetWeaponGO(WeaponType weaponType)
    {
        SetWeaponGO(weaponType);
    }

    [ObserversRpc]
    private void SetWeaponGO(WeaponType weaponType)
    {
        pistolGO.SetActive(false);
        submachineGO.SetActive(false);

        switch (weaponType)
        {
            case WeaponType.Pistol:
                pistolGO.SetActive(true);
                break;
            case WeaponType.Submachine:
                submachineGO.SetActive(true);
                break;
            default:
                break;
        }
    }

    #endregion

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

    #region Death Handler

    [ServerRpc]
    private void DeathServer()
    {
        //Debug.Log("Player " + gameObject.name + " is dead!");
        //Debug.Log("Killed By: " + killedByPlayer.name + "!");
        //playerHUD.UpdateGlobalMessagingWindow(gameObject.name.ToString(), killedByPlayer.name.ToString());
        Death();
        //playerHUD.sceneFader.gameObject.SetActive(true);
    }

    [ObserversRpc]
    private void Death()
    {
        playerModel.gameObject.SetActive(false);
        playerModel.tag = StringData.UntaggedTag;
    }

    public void DeathHandler()
    {
        if (isDead == false) return;

        Debug.Log("Death Handler");
        if (curSpawnTimer >= spawnTimer)
        {
            curSpawnTimer = 0f;
            ResetP();
        }
        else
            curSpawnTimer += Time.deltaTime;
    }

    private void ResetP()
    {
        playerHUD.UpdatePlayerHealth(currentHealth);
        playerHUD.UpdateActiveWeapon(defaultWeapon.Name);
        playerHUD.UpdateAmmo("N", "A");
        pistol.ResetWeapon();
        submachine.ResetWeapon();
        isDead = false;
        ResetPlayerServer();
        //// spawn player at a random spawn point
        //int spawnIdx = Random.Range(0, spawnPoints.Length - 1);
        //transform.position = spawnPoints[spawnIdx].position;
    }

    [ServerRpc]
    private void ResetPlayerServer()
    {
        currentHealth = maxHealth;
        ResetPlayer();
    }

    [ObserversRpc]
    private void ResetPlayer()
    {
        playerModel.gameObject.SetActive(true);
        playerModel.tag = StringData.PlayerTag;
    }
    #endregion
}
