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

public class PlayerController : MonoBehaviour
{
    [Header("REQUIRED")]
    [SerializeField] Animator animator;
    [SerializeField] CharacterController characterController;
    [SerializeField] Transform cameraOrigin;
    [SerializeField] Transform groundCheckOrigin;

    [Header("GUN PROPERTIES")] 
    [SerializeField] PlayerGun gun;
    [Header("CONFIGURABLE MOVEMENT PROPERTIES")]
    [SerializeField] float defaultWalkingVelocity;
    [SerializeField] float walkingAnimationBrakingFactor;
    [SerializeField] float maxWalkingSpeed;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float ainAnimationSpeed;
    [SerializeField] float gravity = -9.8f;
    [SerializeField] float maxJumpHeight;
    [SerializeField] float maxSlopeAngle = 45f;
    [Header("INPUT DATA")]
    [SerializeField] Vector2 movementInputDir;
    [SerializeField] Vector2 mouseInput;
    [Header("DEBUG DATA")]
    // state
    [SerializeField] bool isAiming;
    [SerializeField] bool isAimed;
    [SerializeField] bool isJumping;
    [SerializeField] bool isGrounded;
    [SerializeField] Vector3 jumpingVector;
    [SerializeField] Vector3 finalMovementVector;
    PlayerInputActions input;
    // camera stuff
    [SerializeField] float cameraHorizontalRotation = 0f;
    [SerializeField] float cameraVerticalRotation = 0f;
    // animation stuff
    [SerializeField] Vector3 animationWalkingVelocity;
    [SerializeField] Vector3 intendedWalkingVector;
    // ground detection properties
    [SerializeField] float groundCheckRadius;
    [SerializeField] float groundDistance;
    [SerializeField] LayerMask groundMask;
    
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
        input.PlayerControls.Shoot.performed += ctx =>
        {
            ShootGun();
        };
    }

    void OnEnable()
    {
        if (input != null)
        {
            input.PlayerControls.Enable();
        }
        
    }
    
    void OnDisable()
    {
        if (input != null)
        {
            input.PlayerControls.Disable();
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Jump()
    {
        if (isGrounded)
        {
            isJumping = true;
        }
    }

    void Update()
    { 
        SetIsGrounded(); // isGrounded set
        UpdateMouseInput(); // cameraHorizontalRotation and cameraVerticalRotation set
        UpdatePlayerRotationAndCamera(); // transform.localRotation and cameraOrigin.localRotation set
        UpdateWalkingAnimation(); // animationWalkingVelocity set
        UpdateAimingLayer(); // animator aiming layer weight, isAiming and isAimed set
        UpdateMovePlayer(); // intendedWalkingVector set (lateral movement)
        UpdateJumping(); // jumpingVector set (vertical movement)
        finalMovementVector = intendedWalkingVector + jumpingVector;
        characterController.Move(finalMovementVector * Time.deltaTime);
    }


     void SetIsGrounded()
    {
        bool groundDetected = Physics.SphereCast(groundCheckOrigin.position, groundCheckRadius, Vector3.down, out RaycastHit hitInfo, groundDistance, groundMask);
        float slopeAngle = Vector3.Angle(hitInfo.normal, Vector3.up);
        isGrounded = groundDetected && slopeAngle <= maxSlopeAngle;
    }

    void UpdateMouseInput()
    {
        // rotate whole object horizontally based on mouse pos
        Vector2 mouseDelta = mouseInput * (mouseSensitivity * Time.deltaTime);
        cameraHorizontalRotation += mouseDelta.x;
        // keep yRotation within reasonable bounds
        if (cameraHorizontalRotation is < -360f or > 360f)
        {
            cameraHorizontalRotation %= 360f;
        }
        // clamp xRotation
        cameraVerticalRotation -= mouseDelta.y;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -89f, 70f);
    }

    void UpdatePlayerRotationAndCamera()
    {
        transform.localRotation = Quaternion.Euler(0f, cameraHorizontalRotation, 0f);
        cameraOrigin.localRotation = Quaternion.Euler(cameraVerticalRotation, 0f, 0f);
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

    void UpdateAimingLayer()
    {
        float currentAimingLayerWeight = animator.GetLayerWeight(1);
        float targetWeight = isAiming ? 1.0f : 0.0f;
        float newWeight = Mathf.Lerp(currentAimingLayerWeight, targetWeight, ainAnimationSpeed * Time.deltaTime);
        animator.SetLayerWeight(1, newWeight);
        isAimed = newWeight >= 0.95f;
    }

    void UpdateMovePlayer()
    {
        // calculate the forward and right direction relative to the player and multiply by input
        Vector3 forward = new Vector3(transform.forward.x, 0f, transform.forward.z);
        Vector3 right = new Vector3(transform.right.x, 0f, transform.right.z);
        Vector3 relativeDir = (forward * movementInputDir.y + right * movementInputDir.x).normalized;
        intendedWalkingVector = relativeDir * defaultWalkingVelocity;
    }

    void UpdateJumping()
    {
        // reset vertical velocity when landing
        if (isGrounded && jumpingVector.y < 0)
        {
            jumpingVector.y = -9.8f;
        }

        // jump
        if (isGrounded && isJumping)
        {
            jumpingVector.y = Mathf.Sqrt(maxJumpHeight * -2 * gravity);
            isJumping = false;
        }
        
        if (!isGrounded)
        {
            float fallMultiplier = 2f;
            // apply a stronger gravity force when falling down
            jumpingVector.y += gravity * (jumpingVector.y < 0 ? fallMultiplier : 1) * Time.deltaTime;
        }
    }

    void ShootGun()
    {
        if (isAimed)
        {
            gun.Shoot();
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        // draw a sphere at the ground check position with the specified radius
        Gizmos.DrawWireSphere(groundCheckOrigin.position, groundCheckRadius);
        // draw a line to visualize ground distance
        Gizmos.DrawLine(groundCheckOrigin.position, groundCheckOrigin.position + Vector3.down * groundDistance);
    }
}
