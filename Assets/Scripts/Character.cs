
using System;
using System.Collections.Generic;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEngine.Serialization;

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

    public float maxDistance = 8f;

    public Cell CurrentCell { get; set; }
    

    public List<Cell> path;

    Rigidbody _rigidbody;
    BoxCollider _collider;

    Vector3 _targetPos;
    Cell _targetCell;
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
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
    }

    void Start()
    {

        _rigidbody.position = new Vector3(0, 1.6f, 0);
    }

    public void MoveTo(Cell targetCell)
    {
        if (_isMoving) return;
        _targetCell = targetCell;
        _targetPos = WorldBoard.BoardToWorldPosition(targetCell.boardPos);
        _targetPos.y += 1.5f;
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
            _targetCell.ColorReset();
            _isMoving = false;
            _isMidpointArrived = false;
            _rigidbody.useGravity = true;
        }
    }

    void FixedUpdate()
    {
        if (!_isMoving && path.Count > 0)
        {
            MoveTo(path[0]);
            CurrentCell = path[0];
            path.RemoveAt(0);
        }
        
        Move();
    }
}