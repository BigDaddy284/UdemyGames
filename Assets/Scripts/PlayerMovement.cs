using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Header("General Setup Settings")]
    [Header("Modifies Player Ship Attributes")]
    [Tooltip("How fast ship moves up and down")]
    [SerializeField] float controlSpeed = 20f;

    [Tooltip("Movement Range left to right")]
    [SerializeField] float xRange = 5.5f;
    
    [Tooltip("Movement Range up and down")]
    [SerializeField] float yRange = 4f;

    [Header("Modifies Player Ship X Y Z")]

    [Tooltip("Modifies Player Phip position factor")]
    [SerializeField] float positionPitchFactor = -2f;

    [Tooltip("Modifies player ship pitch factor")]
    [SerializeField] float controlPitchFactor = -20f;

    [Tooltip("Modifies player ship yaw factor")]
    [SerializeField] float positionYawFactor = 2;

    [Tooltip("Used for grouping ogjects in an arry (Lasers)")]
    [SerializeField] GameObject[] lasers;

    float xMove, yMove;
    
    // Update is called once per frame
    void Update()
    {
        ProcessMovement();
        ProcessRotation();
        ProcessFiring();

    }

    void ProcessRotation()
    {
        float pitchDueToPosition = transform.localPosition.y * positionPitchFactor;
        float pitchDueTControlMove = yMove * controlPitchFactor;
            float pitch = pitchDueToPosition + pitchDueTControlMove;

        float yawDueToPosition = transform.localPosition.x * positionYawFactor;        
            float yaw = yawDueToPosition;

        float rollDueTControlMove = xMove * controlPitchFactor;
            float roll = rollDueTControlMove;

        transform.localRotation = Quaternion.Euler(pitch, yaw, roll);
    }

    private void ProcessMovement()
    {
        xMove = Input.GetAxis("Horizontal");
        yMove = Input.GetAxis("Vertical");

        float xOffset = xMove * Time.deltaTime * controlSpeed;
        float newXPos = transform.localPosition.x + xOffset;
        float xPosLimit = Mathf.Clamp(newXPos, -xRange, xRange);

        float yOffset = yMove * Time.deltaTime * controlSpeed;
        float newYPos = transform.localPosition.y + yOffset;
        float yPosLimit = Mathf.Clamp(newYPos, -yRange, yRange);

        transform.localPosition = new Vector3(xPosLimit, yPosLimit, transform.localPosition.z);
    }

    void ProcessFiring()
    {
         if (Input.GetButton("Fire1"))
        {
            SetLasersActive(true);
        }
        else
        {
            SetLasersActive(false);
        }
    }

    private void SetLasersActive(bool isActive) 
    {
        foreach (GameObject laser in lasers)
        {
            var emissionModule = laser.GetComponent<ParticleSystem>().emission;

            emissionModule.enabled = (isActive);
        }
    }

}
