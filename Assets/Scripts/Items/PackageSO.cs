using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "newPackage", menuName = "ScriptObjects/Items/New Package")]
    public class PackageSO : SerializedScriptableObject
    {
        public Vector2Int size;
        public Dictionary<int, ItemSO> items;
    }
}
