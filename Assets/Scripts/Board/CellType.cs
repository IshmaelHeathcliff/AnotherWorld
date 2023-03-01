using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Board
{
    [CreateAssetMenu(fileName = "CellType", menuName = "Board/new CellType", order = 0)]
    public class CellType : SerializedScriptableObject
    {
        public Dictionary<string, Color> cellColor;
    }
}