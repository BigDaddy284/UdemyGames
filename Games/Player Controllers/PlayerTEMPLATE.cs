using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] float forwardSpeed = 20f;  // Adjust speed as necessary
    [SerializeField] float rotationSpeed = 160f; // Degrees per second

    public AudioClip playerMove;  //Add Audio file in inspector, would be added to Update
    public AudioClip playerDeath; // See example Void OnCollisionEnter below
    public AudioClip playerOther; // continuie to add for access to add audio via the instpector and call below as things happen to the player

    public AudioClip turnSkid;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Accelerate();
        Steering();

    }

    private void Accelerate()
    {
        float accelerationSpeed = Input.GetAxis("Vertical") * forwardSpeed * Time.deltaTime;
        transform.Translate(0, accelerationSpeed, 0);        
    }

    void Steering()
    {
        float steerAmount = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, 0, -steerAmount);        
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "null") //will be called only when colliding with an object with the 'Tag' shown here as null
        {
            Destroy(collision.gameObject); //This will destrory the object you collided with

            GetComponent<AudioSource> ().PlayOneShot(playerDeath);
        }
    }
}
