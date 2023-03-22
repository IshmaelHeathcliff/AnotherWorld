using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float liftSpeed = 1000f;
    public Vector3 distanceToCharacter = new Vector3(0, 16, -8);
    public float maxViewY = 25f;
    public float minViewY = 5f;
    public bool updateWithCharacter;
    
    Vector3 _velocity = Vector3.zero;

    [ContextMenu("Move to Character")]
    void MoveToCharacter()
    {
        Vector3 characterPosition = Character.Character.Instance.transform.position;
        characterPosition.y = 0;
        Vector3 targetPosition = characterPosition + distanceToCharacter;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, 0.3f);
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
        switch (lift)
        {
            case < 0 when transform.position.y < maxViewY:
            case > 0 when transform.position.y > minViewY:
                Vector3 delta = new Vector3(0, -1, 0.5f) * (lift * liftSpeed * Time.deltaTime);
                if ((transform.position + delta).y > maxViewY || (transform.position + delta).y < minViewY)
                    break;
                transform.position += delta;
                distanceToCharacter += delta;
                break;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            updateWithCharacter = !updateWithCharacter;
        }

        if (updateWithCharacter)
        {
            MoveToCharacter();
        }
    }
}
