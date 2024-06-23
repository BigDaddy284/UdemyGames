using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enimies : MonoBehaviour
{
    void OnParticleCollision(GameObject other) 
    {
        Debug.Log(other.gameObject.name);
        Destroy(gameObject);
    }
       
}
