using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform target;
    bool laserEnabled = false;
    
    public void EnableLaser(bool isEnabled)
    {
        if (laserEnabled != isEnabled)
        {
            laserEnabled = isEnabled;
            lineRenderer.enabled = isEnabled;
        }
    }
    
    void Update()
    {
        // point laser at target
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, target.position);
    }
}
