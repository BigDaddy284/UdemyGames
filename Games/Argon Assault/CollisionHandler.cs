using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    
    void OnCollisionEnter(Collision other) 
    {
        Debug.Log(this.name + "--Hit--" + other.gameObject.name);
    }        

    void OnTriggerEnter(Collider other) 
    {
        Debug.Log($"{this.name} **Trigger** {other.gameObject.name}");
    }
}
