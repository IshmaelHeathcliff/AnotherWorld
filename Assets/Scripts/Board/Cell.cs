using UnityEngine;
using UnityEngine.EventSystems;

namespace Board
{
    public class Cell : MonoBehaviour
    {
        public Vector3Int boardPos;

        public bool canPass;

        public CellType celType;
        // public Color mapColor;

        public Events.CellEvents events;

        [Range(0, 100)]
        public float weight;

        float _highlight = 1.5f;

        bool _confirmed;

        Color _initialColor;

        MeshRenderer _renderer;

        public void Highlight()
        {
            _renderer.material.color *= _highlight;
        }
    
        public void Reset()
        {
            _renderer.material.color = _initialColor;
            _confirmed = false;
        }
    
        void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _initialColor = _renderer.material.color;
        }

        void OnMouseUp()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if(canPass && !Character.Character.Instance.isMoving)
            {
                switch (_confirmed)
                {
                    case false:
                    {
                        Character.Character.Instance.ResetPathColor();
                        if (WorldBoard.Instance.FindPath(boardPos))
                        {
                            Character.Character.Instance.HighlightPath();
                            _confirmed = true;
                        }
                        break;
                    }
                    case true:
                        Character.Character.Instance.StartMoving();
                        _confirmed = false;
                        break;
                }
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag($"Player"))
            {
                events.Execute();
            }
        }
    }
}
