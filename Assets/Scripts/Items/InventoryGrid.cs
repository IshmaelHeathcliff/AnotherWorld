using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Items
{
    public class InventoryGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public int tileSize = 64;

        public PackageSO packageSO;
        public GameObject itemPrefab;

        Vector2Int _gridSize;
        RectTransform _rectTransform;

        public Vector2Int GetGridPosition(Item item)
        {
            var itemTransform = item.GetComponent<RectTransform>();

            Vector2 mousePosition = (Vector2)Input.mousePosition - 
                                    Vector2.Scale(item.itemSO.size * tileSize, new Vector2(1, -1)) / 2;
            Vector2 position = _rectTransform.position;
            
            var gridPosition =  new Vector2Int
            {
                x = Mathf.RoundToInt((mousePosition.x - position.x) / tileSize),
                y = Mathf.RoundToInt((position.y - mousePosition.y) / tileSize)
            };
            // Debug.Log(gridPosition);
            return gridPosition;
        }

        public  Vector2Int KeyToGridPosition(int key)
        {
            var gridPos = new Vector2Int(key % _gridSize.x, key / _gridSize.x);
            // Debug.Log(gridPos);
            return gridPos;
        }

        public int GridPositionToKey(Vector2Int gridPos)
        {
            var key = gridPos.x + gridPos.y * _gridSize.x;
            // Debug.Log(key);
            return key;
        }

        public Vector2Int GetItemGridPosition(Item item)
        {
            var itemTransform = item.GetComponent<RectTransform>();
            Vector2Int gridPos = Vector2Int.RoundToInt(
                Vector2.Scale(itemTransform.anchoredPosition / tileSize, new Vector2(1, -1))
                - (Vector2)item.itemSO.size / 2);
            // Debug.Log($"itemPos: {gridPos}");
            return gridPos;
        }

        void InitItem(Item item, int key)
        {
            var itemTransform = item.GetComponent<RectTransform>();

            Vector2Int gridPos = KeyToGridPosition(key);
            itemTransform.anchoredPosition = 
                Vector2.Scale(gridPos + (Vector2)item.itemSO.size / 2, new Vector2(1, -1)) * tileSize;
            itemTransform.sizeDelta = item.itemSO.size * tileSize;
        }
        
        void InitInventory()
        {
            _gridSize = packageSO.size;
            _rectTransform.sizeDelta = _gridSize * tileSize;

            foreach (var itemPair in packageSO.items)
            {
                GameObject itemGameObject = Instantiate(itemPrefab, transform);
                var item = itemGameObject.GetComponent<Item>();
                item.itemSO = itemPair.Value;
                item.name = item.itemSO.itemName;
                InitItem(item, itemPair.Key);
            }
        }

        bool CheckGridSpace(Item item, Vector2Int gridPos)
        {
            // 不能超出背包
            Vector2Int endPos = gridPos + item.itemSO.size - new Vector2Int(1, 1);
            
            // Debug.Log($"startPos: {gridPos}");
            // Debug.Log($"endPos: {endPos}");

            if (new Vector2Int[] {gridPos, endPos}.Any(
                    pos => pos.x < 0 || pos.x >= _gridSize.x || pos.y < 0 || pos.y >= _gridSize.y))
            {
                return false;
            }

            // 检查背包是够有空位
            var key = GridPositionToKey(gridPos);
            var keysToCheck = new List<int>();
            for (var i = 0; i < item.itemSO.size.x; i++)
            {
                for (var j = 0; j < item.itemSO.size.y; j++)
                {
                    var keyToCheck = key + i + j * _gridSize.x;
                    if (keyToCheck >= _gridSize.x * _gridSize.y) return false;
                    keysToCheck.Add(keyToCheck);
                }
            }

            return packageSO.items.All(itemSO => !keysToCheck.Contains(itemSO.Key));
        }

        public bool AddItem(Item item, Vector2Int gridPos)
        {
            if (!CheckGridSpace(item, gridPos)) return false;

            var key = GridPositionToKey(gridPos);
            InitItem(item, key);
            packageSO.items.Add(key, item.itemSO);
            return true;
        }

        public void RemoveItem(Vector2Int gridPos)
        {
            var key = GridPositionToKey(gridPos);
            packageSO.items.Remove(key);
        }

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        void Start()
        {
            InitInventory();
        }

        void Update()
        {
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            InventoryController.Instance.currentGrid = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InventoryController.Instance.currentGrid = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            InventoryController.Instance.PutDownItem();
        }
    }
}
