using Cinemachine;
using FishNet.Component.Transforming;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [Header("Move Value")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    [Header("Look Values")]
    private float rotationX = 0;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public float playerRot = 5.0f;

    [SerializeField] float cameraYOffset = 0.4f;

    public CinemachineVirtualCamera vCam;
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;

    [HideInInspector] public bool canMove = true;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
        }
        else
        {
            gameObject.GetComponent<PlayerController>().enabled = false;
        }
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    private void Update()
    {
        if (vCam == null) return;

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

        WeaponFire();
    }

    /// <summary>
    /// Rotate player to look direction
    /// </summary>
    private void RotationHandler()
    {
        var camForward = vCam.transform.forward;
        camForward.y = 0;
        Quaternion targetDir = Quaternion.LookRotation(camForward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetDir, playerRot);
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

    private void WeaponFire()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Debug.Log("Fire");
        }
    }
}
