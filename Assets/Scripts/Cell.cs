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

    MeshRenderer _renderer;

    public void Highlight()
    {
        _renderer.material.color *= _highlight;
    }
    
    public void ColorReset()
    {
        _renderer.material.color /= _highlight;
    }
    
    void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    void OnMouseUp()
    {
        if (canPass)
        {
            WorldBoard.Instance.FindPath(boardPos);
        }
        else
        {
            Debug.Log("can't pass");
        }
    }

    void OnMouseOver()
    {
        
    }
}
