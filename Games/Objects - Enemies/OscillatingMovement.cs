using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillatingMovement : MonoBehaviour
{
    [SerializeField] float m_Speedx = 0f; // Speed of the movement along x-axis
    [SerializeField] float m_Rangex = 0f; // Range of the movement along x-axis
    [SerializeField] float m_Speedy = 0f; // Speed of the movement along y-axis
    [SerializeField] float m_Rangey = 0f; // Range of the movement along y-axis
    [SerializeField] float m_Speedz = 0f; // Speed of the movement along z-axis
    [SerializeField] float m_Rangez = 0f; // Range of the movement along z-axis

    [SerializeField] float m_RotationSpeedX = 0f; // Speed of rotation around x-axis
    [SerializeField] float m_RotationSpeedY = 0f; // Speed of rotation around y-axis
    [SerializeField] float m_RotationSpeedZ = 0f; // Speed of rotation around z-axis

    private float startX; // Initial x position
    private float startY; // Initial y position
    private float startZ; // Initial z position

    // Start is called before the first frame update
    void Start()
    {
        startX = transform.position.x; // Store the initial x position
        startY = transform.position.y; // Store the initial y position
        startZ = transform.position.z; // Store the initial z position
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate new positions
        float newX = startX + Mathf.Sin(Time.time * m_Speedx) * m_Rangex;
        float newY = startY + Mathf.Sin(Time.time * m_Speedy) * m_Rangey;
        float newZ = startZ + Mathf.Sin(Time.time * m_Speedz) * m_Rangez;
        
        // Update position
        transform.position = new Vector3(newX, newY, newZ);
        
        // Calculate new rotation
        float rotationX = m_RotationSpeedX * Time.deltaTime;
        float rotationY = m_RotationSpeedY * Time.deltaTime;
        float rotationZ = m_RotationSpeedZ * Time.deltaTime;
        
        // Update rotation
        transform.Rotate(rotationX, rotationY, rotationZ);
    }
}