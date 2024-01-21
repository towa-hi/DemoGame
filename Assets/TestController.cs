using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class TestController : MonoBehaviour
{
    [Header("REQUIRED")]
    [SerializeField] Animator animator;
    [SerializeField] CharacterController characterController;
    [SerializeField] Transform cameraOrigin;
    [Header("CONFIGURABLE MOVEMENT PROPERTIES")]
    [SerializeField] float defaultWalkingVelocity;
    [SerializeField] float walkingAnimationBrakingFactor;
    [SerializeField] float maxWalkingSpeed;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float ainAnimationSpeed;
    [SerializeField] Vector3 gravityVector;
    [SerializeField] float maxJumpHeight;
    [Header("INPUT DATA")]
    [SerializeField] Vector2 movementInputDir;
    [SerializeField] Vector2 mouseInput;
    [Header("DEBUG DATA")]
    [SerializeField] Vector3 animationWalkingVelocity;
    [SerializeField] float cameraHorizontalRotation = 0f;
    [SerializeField] float cameraVerticalRotation = 0f;
    [SerializeField] Vector3 intendedWalkingVector;
    [SerializeField] Vector3 jumpingVector;
    
    [SerializeField] bool isAiming = false;
    [SerializeField] bool isJumping = false;
    PlayerInputActions input;
    [SerializeField] Vector3 finalMovementVector;
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
        input.PlayerControls.MouseAim.performed += ctx =>
        {
            mouseInput = ctx.ReadValue<Vector2>();
        };
        input.PlayerControls.MouseAim.canceled += ctx =>
        {
            mouseInput = Vector2.zero;
        };
        input.PlayerControls.Aim.performed += ctx =>
        {
            isAiming = true;
        };
        input.PlayerControls.Aim.canceled += ctx =>
        {
            isAiming = false;
        };
        input.PlayerControls.Jump.performed += ctx =>
        {
            Jump();
        };
    }

    void OnEnable()
    {
        input.PlayerControls.Enable();
    }
    
    void OnDisable()
    {
        input.PlayerControls.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Jump()
    {
        if (characterController.isGrounded)
        {
            isJumping = true;
        }
    }
    
    void Update()
    {
        UpdateMouseInput();
        UpdatePlayerRotationAndCamera();
        UpdateWalkingAnimation();
        UpdateAimingLayer();
        UpdateMovePlayer();
        UpdateJumping();
        finalMovementVector = intendedWalkingVector + jumpingVector;
        characterController.Move(finalMovementVector * Time.deltaTime);
    }

    void UpdateMouseInput()
    {
        // rotate x based on mouse pos
        Vector2 mouseDelta = mouseInput * mouseSensitivity * Time.deltaTime;
        cameraVerticalRotation += mouseDelta.x;
        // keep yRotation within reasonable bounds
        if (cameraVerticalRotation < -360f || cameraVerticalRotation > 360f)
        {
            cameraVerticalRotation %= 360f;
        }
        // clamp xRotation
        cameraHorizontalRotation -= mouseDelta.y;
        cameraHorizontalRotation = Mathf.Clamp(cameraHorizontalRotation, -89f, 70f);
    }

    void UpdatePlayerRotationAndCamera()
    {
        transform.localRotation = Quaternion.Euler(0f, cameraVerticalRotation, 0f);
        cameraOrigin.localRotation = Quaternion.Euler(cameraHorizontalRotation, 0f, 0f);
    }

    void UpdateWalkingAnimation()
    {
        
        // apply acceleration when there is input or deceleration if no input
        if (movementInputDir != Vector2.zero)
        {
            
            animationWalkingVelocity.x = Mathf.Lerp(animationWalkingVelocity.x, maxWalkingSpeed * movementInputDir.x, defaultWalkingVelocity * Time.deltaTime);
            animationWalkingVelocity.z = Mathf.Lerp(animationWalkingVelocity.z, maxWalkingSpeed * movementInputDir.y, defaultWalkingVelocity * Time.deltaTime);
        }
        else
        {
            animationWalkingVelocity.x = Mathf.Lerp(animationWalkingVelocity.x, 0, walkingAnimationBrakingFactor * Time.deltaTime);
            animationWalkingVelocity.z = Mathf.Lerp(animationWalkingVelocity.z, 0, walkingAnimationBrakingFactor * Time.deltaTime);
        }
        animator.SetFloat("Velocity X", animationWalkingVelocity.x);
        animator.SetFloat("Velocity Z", animationWalkingVelocity.z);
    }

    void UpdateMovePlayer()
    {
        // calculate the forward and right direction relative to the player
        Vector3 forward = new Vector3(transform.forward.x, 0f, transform.forward.z);
        Vector3 right = new Vector3(transform.right.x, 0f, transform.right.z);
        Vector3 relativeDir = (forward * movementInputDir.y + right * movementInputDir.x).normalized;
        // apply the movement to the character
        intendedWalkingVector = relativeDir * defaultWalkingVelocity;
    }

    void UpdateJumping()
    {
        // reset vertical velocity when landing
        if (characterController.isGrounded && jumpingVector.y < 0)
        {
            jumpingVector.y = 0f;
        }

        // Jump
        if (isJumping && characterController.isGrounded)
        {
            jumpingVector.y = Mathf.Sqrt(maxJumpHeight * -2 * -9.8f);
            isJumping = false;
        }

        // Apply gravity
        if (jumpingVector.y < 0)
        {
            float fallMultiplier = 2.5f;
            // why does this not work when I remove Time.deltaTime???
            jumpingVector.y += -9.8f * fallMultiplier * Time.deltaTime;
        }
        else
        {
            // why does this not work when I remove Time.deltaTime???
            jumpingVector.y += -9.8f  * Time.deltaTime;
        }
    }

    void UpdateAimingLayer()
    {
        float currentAimingLayerWeight = animator.GetLayerWeight(1);
        float targetWeight = isAiming ? 1.0f : 0.0f;
        
        float newWeight = Mathf.Lerp(currentAimingLayerWeight, targetWeight, ainAnimationSpeed * Time.deltaTime);
        animator.SetLayerWeight(1, newWeight);
    }
}
