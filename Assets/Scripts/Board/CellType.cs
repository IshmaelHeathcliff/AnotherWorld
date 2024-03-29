using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Board
{
    public enum CellType
    {
        Grass,
        Forest,
        Town,
        Water,
        Empty,
    }
    
    [CreateAssetMenu(fileName = "CellType", menuName = "ScriptObjects/Board/New CellType", order = 0)]
    public class CellTypeSO : SerializedScriptableObject
    {
        public Dictionary<Color, CellType> cellColor;
    }
}