using UnityEngine;
using UnityEngine.EventSystems;

namespace Board
{
    public class Cell : MonoBehaviour
    {
        public Vector3 boardPos;

        public bool canPass;
        // public Color mapColor;

        public Events.CellEvents events;

        float _highlight = 1.5f;

        Color _initialColor;

        MeshRenderer _renderer;

        public void Highlight()
        {
            _renderer.material.color *= _highlight;
        }
    
        public void ResetColor()
        {
            _renderer.material.color = _initialColor;
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
        
            if (canPass && !Character.Character.Instance.isMoving)
            {
                Character.Character.Instance.StartMoving();
            }

        }

        void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
        
            if (canPass && !Character.Character.Instance.isMoving)
            {
                if(WorldBoard.Instance.FindPath(boardPos))
                    Character.Character.Instance.HighlightPath();
            }
        }

        void OnMouseExit()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
        
            if (!Character.Character.Instance.isMoving)
            {
                Character.Character.Instance.ResetPathColor();
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
