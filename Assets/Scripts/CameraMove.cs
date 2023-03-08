using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float liftSpeed = 1000f;
    public Vector3 distanceToCharacter = new Vector3(0, 12, -8);

    [ContextMenu("Move to Character")]
    void MoveToCharacter()
    {
        var characterPosition = Character.Character.Instance.transform.position;
        transform.position = characterPosition + distanceToCharacter;
    }
    
    void Start()
    {
        MoveToCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.isPaused) return;
        
        var horizon = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        transform.position += new Vector3(horizon, 0, vertical) * (moveSpeed * Time.deltaTime);

        var lift = Input.GetAxis("Mouse ScrollWheel");
        transform.position += new Vector3(0, -lift, 0) * (liftSpeed * Time.deltaTime);
    }
}
