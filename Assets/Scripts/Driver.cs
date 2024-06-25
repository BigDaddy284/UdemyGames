using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    [SerializeField] float forwardSpeed = 20f;  // Adjust speed as necessary
    [SerializeField] float rotationSpeed = 160f; // Degrees per second

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float steerAmount = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        float accelerationSpeed = Input.GetAxis("Vertical") * forwardSpeed * Time.deltaTime;
        transform.Translate(0, accelerationSpeed, 0);
        transform.Rotate(0, 0, -steerAmount);
    }
}
