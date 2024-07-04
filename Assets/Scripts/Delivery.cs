using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delivery : MonoBehaviour
{    
    public AudioClip packageCollected;
    [SerializeField] Color32 hasPackageColor = new Color32(0, 0, 1, 1);
    [SerializeField] Color32 noPackageColor = new Color32(1, 1, 1, 1);
    [SerializeField] float  destroyTime = 0.5f;
    bool hasPackage;
    SpriteRenderer spriteRenderer;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void OnCollisionEnter2D(Collision2D other) 
    {       
        Debug.Log("Stop Hitting Sh*T!");        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Package" && !hasPackage)
        {
        Debug.Log("Package collected");
        spriteRenderer.color = hasPackageColor;
        hasPackage = true;
        GetComponent<AudioSource> ().PlayOneShot(packageCollected);
        Destroy(other.gameObject, destroyTime);
        }

        if (other.tag == "Human" && hasPackage)
        {
            Debug.Log("Package Delivered");
            spriteRenderer.color = noPackageColor;
            hasPackage = false;          
        }

        

    }


}
