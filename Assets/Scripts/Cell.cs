using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Cell : MonoBehaviour
{
    public Vector3 boardPos;

    public bool canPass;
    public abstract bool MapColor(Color color);

    const float _highlight = 1.5f;

    Color _initialColor;

    MeshRenderer _renderer;

    public void Highlight()
    {
        _renderer.material.color *= _highlight;
    }
    
    public void ColorReset()
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
        if (canPass && !Character.Instance.isMoving)
        {
            Character.Instance.StartMoving();
        }

    }

    void OnMouseEnter()
    {
        if (canPass && !Character.Instance.isMoving)
        {
            if(WorldBoard.Instance.FindPath(boardPos))
                Character.Instance.HighlightPath();
        }
    }

    void OnMouseExit()
    {
        if (!Character.Instance.isMoving)
        {
            Character.Instance.ResetPathColor();
        }
    }

    void OnMouseOver()
    {
        
    }
}
