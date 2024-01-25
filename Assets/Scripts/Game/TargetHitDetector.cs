using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHitDetector : MonoBehaviour
{
    [SerializeField] Color targetColor;
    [SerializeField] Color hitColor;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hitClip;
    Renderer rend;
    Coroutine colorChangeRoutine;
    
    void Awake()
    {
        rend = GetComponent<Renderer>();
        Material newMaterial = new Material(rend.material);
        rend.material = newMaterial;
        newMaterial.color = targetColor;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            if (colorChangeRoutine != null)
            {
                StopCoroutine(colorChangeRoutine);
            }
            colorChangeRoutine = StartCoroutine(ChangeToHitColor());
        }
    }

    IEnumerator ChangeToHitColor()
    {
        rend.material.color = hitColor;
        float duration = 1f;
        float time = 0;
        while (time < duration)
        {
            rend.material.color = Color.Lerp(hitColor, targetColor, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        rend.material.color = targetColor;
    }
}
