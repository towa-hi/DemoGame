using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [Header("REQUIRED")]
    [SerializeField] Transform gunBarrel;
    [SerializeField] AudioSource audioSource;
    [SerializeField] LayerMask mask;
    [SerializeField] GameObject projectile;

    [Header("CONFIGURABLE PROPERTIES")] 
    [SerializeField] float projectileSpeed;

    [SerializeField] GameObject muzzleFlashSprite;
    Coroutine muzzleFlashRoutine;
    
    public void Shoot()
    {
        audioSource.Play();
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, mask))
        {
            targetPoint = hit.point;
        }
        else
        {
            float defaultDistance = 1000f;
            targetPoint = ray.origin + ray.direction * defaultDistance;
        }

        // Instantiate the projectile
        GameObject projectileInstance = Instantiate(projectile, gunBarrel.position, Quaternion.identity);
    
        // Calculate the direction from the gun barrel to the target point
        Vector3 targetDirection = targetPoint - gunBarrel.position;

        // Set the rotation of the projectile to face the target direction
        projectileInstance.transform.rotation = Quaternion.LookRotation(targetDirection);

        // Initialize the projectile's speed
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        projectileScript.Initialize(projectileSpeed);
        if (muzzleFlashRoutine != null)
        {
            StopCoroutine(muzzleFlashRoutine);
        }

        muzzleFlashRoutine = StartCoroutine(MuzzleFlashRoutine());
    }

    IEnumerator MuzzleFlashRoutine()
    {
        muzzleFlashSprite.SetActive(true);
        float duration = 0.1f;
        yield return new WaitForSeconds(duration);
        muzzleFlashSprite.SetActive(false);
    }
}
