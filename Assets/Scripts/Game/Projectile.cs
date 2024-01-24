using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    float lifetime = 5f;
    [SerializeField] AudioSource hitAudio;
    [SerializeField] AudioClip hitClip;
    [SerializeField] GameObject holePrefab;
    bool hasLanded;
    GameObject hole;
    
    public void Initialize(float projectileSpeed)
    {

        speed = projectileSpeed;
    }

    void OnEnable()
    {
        Debug.Log("on enable");
        StartCoroutine(DeactivateAfterLifetime());
    }

    IEnumerator DeactivateAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(hole);
        Destroy(gameObject);
    }
    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        if (!hasLanded)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            
            // look ahead one frame to add bullet hole
            if (Physics.Raycast(ray, out hit, moveDistance))
            {
                // on hit
                hitAudio.PlayOneShot(hitClip);
                CreateBulletHole(hit);
                Debug.Log("hit");
                hasLanded = true;
                transform.position = hit.point;
                SendToPool();
            }
            else
            {
                transform.Translate(Vector3.forward * moveDistance);
            }
        }
    }

    void SendToPool()
    {
        //Debug.Log("sent to pool");
    }

    void CreateBulletHole(RaycastHit hit)
    {
        // Instantiate the hole prefab at the hit point
        hole = Instantiate(holePrefab, hit.point, Quaternion.identity);

        // Align the hole prefab's up vector with the hit normal
        hole.transform.up = hit.normal;

        // Slightly offset the decal to prevent z-fighting
        hole.transform.position += hole.transform.up * 0.01f;
    }
}
