using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        {
            
        }  
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        if ("Circle" == this.gameObject.name)
        {
            Destroy(this.gameObject);
        }
        else
            Debug.Log("Hit" + other.gameObject.name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Activated");  
    }
}
