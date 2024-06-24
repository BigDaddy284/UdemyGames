//simple move for 3D flat platform movement

using System;
using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Math.EC.Rfc7748;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float xValue = (Input.GetAxis("Horizontal") * Time.deltaTime) * moveSpeed;
        float zValue = (Input.GetAxis("Vertical") * Time.deltaTime) * moveSpeed;
        
        transform.Translate(xValue,0,zValue);
    }
}
