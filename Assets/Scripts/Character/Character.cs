using System.Collections.Generic;
using UnityEngine;

namespace Character
{
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

        public int maxDistance = 8;

        public Cell CurrentCell { get; set; }
    

        public Queue<Cell> Path;

        [HideInInspector] public bool isMoving;

        Rigidbody _rigidbody;
        MeshCollider _collider;

        Vector3 _targetPos;
        Cell _targetCell;
        Vector3 _midpoint;
        bool _isMidpointArrived;

        void Awake()
        {
            if (Instance != this)
            {
                DestroyImmediate(this);
                return;
            }
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<MeshCollider>();
        }

        void Start()
        {
            Path = new Queue<Cell>();
            _rigidbody.position = new Vector3(0, 1.6f, 0);
        }

        public void HighlightPath()
        {
            foreach (Cell cell in Path)
            {
                cell.Highlight();
            }
        }

        public void ResetPathColor()
        {
            foreach (Cell cell in Path)
            {
                cell.ResetColor();
            }
        }

        public void StartMoving()
        {
            if (isMoving || Path.Count <= 0) return;
        
            isMoving = true;
            JumpTo(Path.Peek());
            CurrentCell = Path.Dequeue();
        }
    
        void JumpTo(Cell targetCell)
        {
            _targetCell = targetCell;
            _targetPos = WorldBoard.BoardToWorldPosition(targetCell.boardPos);
            // _targetPos.y += 0.1f;
            _midpoint = (_rigidbody.position + _targetPos) / 2;
            _midpoint.y += 2f;
        }

        void Move()
        {
            if (!isMoving) return;

            _rigidbody.useGravity = false;
            _collider.isTrigger = true;
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
                _targetCell.ResetColor();
                if (Path.Count > 0)
                {
                    JumpTo(Path.Peek());
                    CurrentCell = Path.Dequeue();
                    _isMidpointArrived = false;
                }
                else
                {
                    _isMidpointArrived = false;
                    _rigidbody.useGravity = true;
                    _collider.isTrigger = false;
                    isMoving = false;
                }

            }
        }

        void FixedUpdate()
        {
            Move();
        }
    }
}