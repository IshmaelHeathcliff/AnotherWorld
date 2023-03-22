using System.Collections.Generic;
using Board;
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

        public Vector2Int initialPosition = Vector2Int.zero;
        public float height = 0.24f;
        
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
            _collider = GetComponentInChildren<MeshCollider>();
        }

        void Start()
        {
            Path = new Queue<Cell>();
            _rigidbody.useGravity = false;
        }

        public void InitCharacter()
        {
            MoveToInitialPosition();
            _rigidbody.useGravity = true;
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
                cell.Reset();
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
                _targetCell.Reset();
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

        [ContextMenu("Move to initial position")]
        void MoveToInitialPosition()
        {
            transform.position = WorldBoard.BoardToWorldPosition(new Vector3Int(
                initialPosition.x, initialPosition.y, -initialPosition.x-initialPosition.y))
                                 + new Vector3(0, height, 0);
        }

        void FixedUpdate()
        {
            Move();
        }
    }
}