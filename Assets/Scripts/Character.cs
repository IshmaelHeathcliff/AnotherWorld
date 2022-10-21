
using System;
using Sirenix.Utilities.Editor;
using UnityEngine;

public class Character: MonoBehaviour
{
    static Character _instance;

    public static Character Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            
            if (_instance == null)
            {
                _instance = FindObjectOfType<Character>();
            }
            
            return _instance;
        }
        private set => _instance = value;
    }

    public float speed = 5f;
    
    Rigidbody _rigidbody;
    BoxCollider _collider;

    Vector3 _targetPos;
    Vector3 _midpoint;
    bool _isMoving;
    bool _isMidpointArrived;

    void Awake()
    {
        if (Instance != this)
        {
            DestroyImmediate(this);
            return;
        }
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
        _rigidbody.position = new Vector3(0, 1.6f, 0);
    }

    public void MoveTo(Vector2 boardPos)
    {
        if (_isMoving) return;
        _targetPos = WorldBoard.BoardToWorldPosition(boardPos);
        _targetPos.y += 1.6f;
        _midpoint = (_rigidbody.position + _targetPos) / 2;
        _midpoint.y += 2f;
        _isMoving = true;
    }

    void Move()
    {
        if (!_isMoving) return;

        _rigidbody.useGravity = false;
        Vector3 currentPos = _rigidbody.position;
        if (Vector3.Distance(_rigidbody.position, _midpoint) < 0.1f)
        {
            _rigidbody.MovePosition(_midpoint);
            _isMidpointArrived = true;
        }

        if (!_isMidpointArrived)
        {
            Vector3 moveDirection = (_midpoint - currentPos).normalized;
            _rigidbody.MovePosition(currentPos + moveDirection * (Time.fixedDeltaTime * speed));
        }
        else if(Vector3.Distance(_rigidbody.position, _targetPos) > 0.1f)
        {
            Vector3 moveDirection = (_targetPos - currentPos).normalized;
            _rigidbody.MovePosition(currentPos + moveDirection * (Time.fixedDeltaTime * speed));
        }
        else
        {
            _rigidbody.MovePosition(_targetPos);
            _isMoving = false;
            _isMidpointArrived = false;
            _rigidbody.useGravity = true;
        }
    }

    void FixedUpdate()
    {
        Move();
    }
}