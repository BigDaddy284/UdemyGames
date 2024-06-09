using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropper3Secs : MonoBehaviour
{
    MeshRenderer renderer;
    Rigidbody rigidBody;
    [SerializeField] float timeToWait3 = 3f;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        rigidBody = GetComponent<Rigidbody> ();

        renderer.enabled = false;
        rigidBody.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
    { 
        if(Time.time > timeToWait3)
        {
            renderer.enabled = true;
            rigidBody.useGravity = true;
        }
    }
    }
}
