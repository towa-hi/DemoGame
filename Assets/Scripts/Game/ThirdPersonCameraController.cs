using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField] Vector3 defaultOffset;

    [SerializeField] Vector3 aimingOffset;

    [SerializeField] Animator animator;

    [SerializeField] Transform cameraOrigin;

    Vector2 defaultCrosshairSize = new Vector2(50, 50);
    Vector2 focusedCrosshairSize = new Vector2(25, 25);
    [SerializeField] RectTransform crosshairPanel;
    
    void Update()
    {
        // Get the weight of the aiming layer
        float aimingWeight = animator.GetLayerWeight(1);

        // Interpolate between defaultOffset and aimingOffset based on aimingWeight
        Vector3 currentOffset = Vector3.Lerp(defaultOffset, aimingOffset, aimingWeight);

        // Set the local position of the cameraOrigin
        cameraOrigin.localPosition = currentOffset;

        // Interpolate the size of the crosshair
        Vector2 currentCrosshairSize = Vector2.Lerp(defaultCrosshairSize, focusedCrosshairSize, aimingWeight);

        // Apply the interpolated size to the crosshairPanel
        crosshairPanel.sizeDelta = currentCrosshairSize;
    }
}
