using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] Laser laser;
    [SerializeField] Transform gunBarrel;
    [SerializeField] AudioSource audioSource;

    public void Shoot()
    {
        audioSource.Play();
    }
}
