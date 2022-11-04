﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float liftSpeed = 1000f;
    
    // Update is called once per frame
    void Update()
    {
        var horizon = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        transform.position += new Vector3(horizon, 0, vertical) * (moveSpeed * Time.deltaTime);

        var lift = Input.GetAxis("Mouse ScrollWheel");
        transform.position += new Vector3(0, lift, 0) * (liftSpeed * Time.deltaTime);
    }
}