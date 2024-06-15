using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitApplication : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Quit();
    }

    void Quit()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();            
        }
    }
}
