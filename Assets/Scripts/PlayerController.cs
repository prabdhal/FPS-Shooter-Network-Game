using Cinemachine;
using FishNet.Component.Transforming;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Threading;
using UnityEngine;


public class PlayerController : NetworkBehaviour
{
    public Camera cam;
    public CinemachineVirtualCamera vCam;
    public CinemachineBrain brainCam;
    public AudioListener audioListener;
    private CharacterController characterController;
    [SerializeField]
    private Transform playerModel;


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
    [SyncVar] 
    public float currentHealth = 100f;

    [Header("Weapon")]
    //private GameObject projectilePrefab;
    public Weapon activeWeapon;
    public Weapon defaultWeapon;
    public Weapon equippedWeapon;
    public bool isFiring = false;
    private float fireTimer = 0;


    private Vector3 moveDirection = Vector3.zero;

    [HideInInspector] public bool canMove = true;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            gameObject.name = this.IsHost ? "Server" : "Client";
            vCam.enabled = true;
            brainCam.enabled = true;
            cam.enabled = true;
            audioListener.enabled = true;
        }
        else
        {
            gameObject.GetComponent<PlayerController>().enabled = false;
        }
    }

    private void Start()
    {
        characterController = GetComponentInChildren<CharacterController>();
        currentHealth = maxHealth;

        // Lock cursor
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    private void Update()
    {
        if (vCam == null)   return;


        if (currentHealth <= 0)
        {
            Death();
            return;
        }

        //UpdateValue();

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

        if (activeWeapon != null)
        {
            Debug.Log("Active weapon is active");
            SFireWeapon(activeWeapon, this);
        }
        else
        {
            Debug.Log("Default weapon is active");
            SFireWeapon(defaultWeapon, this);
        }
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

    private void SFireWeapon(Weapon weapon, PlayerController player)
    {
        Debug.Log("Player Controller FireWeapon");
        weapon.FireWeapon(this);
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
        SetSpawnObject(go);
    }

    [ServerRpc]
    public void UpdateHealth(float amount)
    {
        currentHealth += amount;
        Debug.Log(gameObject.name + " taking " + amount + " points of damage");
    }

    private void Death()
    {
        Debug.Log("Player " + " " + " is dead!");
    }
}
