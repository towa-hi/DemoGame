using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    float lifetime = 5f;
    [SerializeField] AudioSource hitAudio;
    [SerializeField] AudioClip hitClip;
    [SerializeField] AudioClip targetHitClip;
    [SerializeField] GameObject holePrefab;
    bool hasLanded;
    GameObject hole;
    
    public void Initialize(float projectileSpeed)
    {

        speed = projectileSpeed;
    }

    void OnEnable()
    {
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
            int layerMask = 1 << LayerMask.NameToLayer("Projectile");
            layerMask = ~layerMask;
            // look ahead one frame to add bullet hole
            if (Physics.Raycast(ray, out hit, moveDistance, layerMask))
            {
                // on hit
                if (hit.collider.gameObject.GetComponent<TargetHitDetector>())
                {
                    hitAudio.PlayOneShot(targetHitClip, 2);
                }
                else
                {
                    hitAudio.PlayOneShot(hitClip, 0.5f);
                }
                
                CreateBulletHole(hit);
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
        hole = Instantiate(holePrefab, hit.point, Quaternion.identity);
        hole.transform.up = hit.normal;
        hole.transform.position += hole.transform.up * 0.01f;
    }
}
