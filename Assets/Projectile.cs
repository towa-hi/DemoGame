using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    float lifetime = 5f;

    [SerializeField] GameObject holePrefab;
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
        SendToPool();
    }
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        float moveDistance = speed * Time.deltaTime;
        // look ahead one frame to add bullet hole
        if (Physics.Raycast(ray, out hit, moveDistance))
        {
            CreateBulletHole(hit);
            SendToPool();
            return;
        }
        else
        {
            transform.Translate(Vector3.forward * moveDistance);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("HIT TARGET");
        //
        // SendToPool();
    }

    void SendToPool()
    {
        Debug.Log("sent to pool");
        Destroy(gameObject);
    }

    void CreateBulletHole(RaycastHit hit)
    {
        // Instantiate the hole prefab at the hit point
        GameObject hole = Instantiate(holePrefab, hit.point, Quaternion.identity);

        // Align the hole prefab's up vector with the hit normal
        hole.transform.up = hit.normal;

        // Slightly offset the decal to prevent z-fighting
        hole.transform.position += hole.transform.up * 0.01f;
    }
}
