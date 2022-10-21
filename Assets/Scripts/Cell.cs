using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Cell : MonoBehaviour
{
    public Vector2 boardPos;
    public abstract bool MapColor(Color color);

    void OnMouseDown()
    {
        Character.Instance.MoveTo(boardPos);
    }
}
