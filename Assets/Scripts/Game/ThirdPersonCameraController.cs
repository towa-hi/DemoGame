using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] Vector3 defaultOffset;

    [SerializeField] Vector3 aimingOffset;

    [SerializeField] Animator animator;

    [SerializeField] Transform cameraOrigin;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    
    [SerializeField] float defaultFOV;
    [SerializeField] float aimFOV;

    Vector2 defaultCrosshairSize = new Vector2(50, 50);
    Vector2 focusedCrosshairSize = new Vector2(25, 25);
    [SerializeField] RectTransform crosshairPanel;
    
    void Update()
    {
        float aimingWeight = animator.GetLayerWeight(1);

        // lerp offset
        Vector3 currentOffset = Vector3.Lerp(defaultOffset, aimingOffset, aimingWeight);
        cameraOrigin.localPosition = currentOffset;

        // lerp crosshair
        Vector2 currentCrosshairSize = Vector2.Lerp(defaultCrosshairSize, focusedCrosshairSize, aimingWeight);
        crosshairPanel.sizeDelta = currentCrosshairSize;
        
        // lerp FOV
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(defaultFOV, aimFOV, aimingWeight);

    }
}
