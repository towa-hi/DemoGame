using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    public static PlayerInputActions input;
    [SerializeField] Vector2 movementInputDir;
    [SerializeField] Vector3 cameraMovementDir;
    [SerializeField] Vector3 targetVelocity;
    [SerializeField] Vector3 currentVelocity;
    [SerializeField] float defaultWalkingVelocity;
    [SerializeField] float brakingVelocity;
    [SerializeField] float maxJumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] bool isJumping = false;
    void Awake()
    {
        input = new PlayerInputActions();
        input.PlayerControls.Movement.performed += ctx =>
        {
            movementInputDir = ctx.ReadValue<Vector2>();
        };
        input.PlayerControls.Movement.canceled += ctx =>
        {
            movementInputDir = Vector2.zero;
        };
        input.PlayerControls.Jump.performed += ctx =>
        {
            Jump();
        };
    }

    void Start()
    {
        if (GameManager.instance.playerController)
        {
            Debug.Log("More than one PlayerController in scene");
        }
        else
        {
            GameManager.instance.playerController = this;
        }
    }
    
    void OnEnable()
    {
        input.PlayerControls.Enable();
    }

    void OnDisable()
    {
        input.PlayerControls.Disable();
    }

    void Update()
    {
        ApplyMovementInput();
        ApplyJump();
        characterController.Move(targetVelocity * Time.deltaTime);
    }

    void ApplyMovementInput()
    {
        float stopThreshold = 0.5f;
        Camera mainCamera = Camera.main;
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        Vector3 cameraForwardZProduct = movementInputDir.y * cameraForward;
        Vector3 cameraRightXProduct = movementInputDir.x * cameraRight;

        Vector3 cameraSpaceVector = cameraForwardZProduct + cameraRightXProduct;
        
        targetVelocity.x = Mathf.Abs(cameraSpaceVector.x) > 0 ? cameraSpaceVector.x * defaultWalkingVelocity : Mathf.Lerp(targetVelocity.x, 0, brakingVelocity * Time.deltaTime);
        targetVelocity.z = Mathf.Abs(cameraSpaceVector.y) > 0 ? cameraSpaceVector.y * defaultWalkingVelocity : Mathf.Lerp(targetVelocity.z, 0, brakingVelocity * Time.deltaTime);

        if (Mathf.Abs(targetVelocity.x) < stopThreshold) targetVelocity.x = 0;
        if (Mathf.Abs(targetVelocity.z) < stopThreshold) targetVelocity.z = 0;
    }

    
    void Jump()
    {
        if (characterController.isGrounded)
        {
            isJumping = true;
        }
    }

    float fallMultiplier = 2.5f;
    
    void ApplyJump()
    {
        // reset vertical velocity when landing
        if (characterController.isGrounded && targetVelocity.y < 0)
        {
            targetVelocity.y = 0f;
        }

        // Jump
        if (isJumping && characterController.isGrounded)
        {
            targetVelocity.y = Mathf.Sqrt(maxJumpHeight * -2 * gravityValue);
            isJumping = false;
        }

        // Apply gravity
        if (targetVelocity.y < 0)
        {
            targetVelocity.y += gravityValue * fallMultiplier * Time.deltaTime;
        }
        else
        {
            targetVelocity.y += gravityValue * Time.deltaTime;
        }
        
    }
}
