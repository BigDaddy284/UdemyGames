using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlimpMovement : MonoBehaviour
{
    [SerializeField] float m_Speedx = 1f; // Speed of the movement
    [SerializeField] float m_Rangex = 12f; // Range of the movement
    [SerializeField] float m_Speedy = 0.2f; // Speed of the movement
    [SerializeField] float m_Rangey = 5f; // Range of the movement
    private float startX; // Initial x position
    private float startY; // Initial y position

    // Start is called before the first frame update
    void Start()
    {
        startX = transform.position.x; // Store the initial x position
        startY = transform.position.y; // Store the initial x position
    }

    // Update is called once per frame
    void Update()
    {
        float newX = startX + Mathf.Sin(Time.time * m_Speedx) * m_Rangex; // Calculate new x position
        transform.position = new Vector3(newX, transform.position.y, transform.position.z); // Update position
        float newY = startY + Mathf.Sin(Time.time * m_Speedy) * m_Rangey; // Calculate new y position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
