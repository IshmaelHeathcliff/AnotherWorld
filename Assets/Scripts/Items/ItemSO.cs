using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    [CreateAssetMenu(fileName = "newItem", menuName = "ScriptObjects/Items/New Item")]
    public class ItemSO : SerializedScriptableObject
    {
        public string itemName;
        public Sprite image;
        public float price;
        public float weight;
        public Vector2Int size = new Vector2Int(1, 1);
    }
}
