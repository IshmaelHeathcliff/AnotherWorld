using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomStep = 2f;
    public float zoomSpeed = 5f;
    public Vector3 distanceToCharacter = new Vector3(0, 16, -8);
    public float maxViewY = 25f;
    public float minViewY = 5f;
    public bool updateWithCharacter;
    
    Vector3 _velocity = Vector3.zero;
    Vector3 _direction;
    Vector3 _targetPosition;

    [ContextMenu("Move to Character")]
    void MoveToCharacter()
    {
        Vector3 characterPosition = Character.Character.Instance.transform.position;
        characterPosition.y = 0;
        _targetPosition = characterPosition + distanceToCharacter;
        transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, 0.3f);
    }

    void Start()
    {
        MoveToCharacter();
        PlayerInput inputAction = InputController.Instance.PlayerInput;
        inputAction.Camera.Move.performed += MoveCameraOnPerformed;
        inputAction.Camera.Zoom.performed += ZoomOnPerformed;
    }

    void ZoomOnPerformed(InputAction.CallbackContext obj)
    {
        float zoom = obj.ReadValue<float>() switch
        {
            > 0 => 1,
            < 0 => -1,
            _ => 0
        };
        var y = _targetPosition.y - zoom * zoomStep;
        Vector3 delta = Vector3.zero;
        if (y < maxViewY && y > minViewY)
            delta = new Vector3(0, -1, 0.5f) * zoom * zoomStep;
        _targetPosition += delta;
        distanceToCharacter += delta;
    }

    void MoveCameraOnPerformed(InputAction.CallbackContext obj)
    {
        var value = obj.ReadValue<Vector2>();
        _direction = new Vector3(value.x, 0, value.y);
    }

    void MoveCamera()
    {
        transform.position += _direction * moveSpeed * _targetPosition.y * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        MoveToCharacter();
    }
    
}
