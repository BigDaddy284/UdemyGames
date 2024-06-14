using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float mainThrust = 5f;
    [SerializeField] float rotationThrust = 50f;
    [SerializeField] AudioClip mainEngine;

    [SerializeField] ParticleSystem boosterLF;
    [SerializeField] ParticleSystem boosterLR;
    [SerializeField] ParticleSystem boosterRF;
    [SerializeField] ParticleSystem boosterRR;
    Rigidbody rb;
    AudioSource audioSource;
    ParticleSystem ps;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();  
        ps = GetComponent<ParticleSystem>();      
    }

    // Update is called once per frame
    void Update()
    {
        ProcessThrust();
        ProcessRotation();
    }

    void ProcessThrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddRelativeForce(Vector3.up * mainThrust);
            if(!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(mainEngine);
                boosterLF.Play();
                boosterLR.Play();
                boosterRF.Play();
                boosterRR.Play();
            }
            
        }
            else
            {
                audioSource.Stop();               
            }
            
    }

    void ProcessRotation()
    {        
        if (Input.GetKey(KeyCode.A))
        {
            ApplyRotation(rotationThrust);  
            boosterLF.Stop();
            boosterLR.Stop();          
        }
        else if (Input.GetKey(KeyCode.D))
        {
            ApplyRotation(-rotationThrust);
            boosterRF.Stop();
            boosterRR.Stop();           
        }
    }

    void ApplyRotation(float rotationThisFrame)
    {
        rb.freezeRotation = true; // freezing rotation so we can manually rotate
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.deltaTime);
        rb.freezeRotation = false; // unfreezing rotation so the physics system can take over
    }
}