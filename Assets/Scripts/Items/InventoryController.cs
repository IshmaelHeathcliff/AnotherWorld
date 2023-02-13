using System;
using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    public class InventoryController: MonoBehaviour
    {
        static InventoryController _instance;

        public static InventoryController Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
            
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InventoryController>();
                }
            
                return _instance;
            }
            private set => _instance = value;
        }
        
        public Item pickedUpItem;
        public InventoryGrid currentGrid;

        public void PickUpItem(Item item)
        {
            if (currentGrid == null) return;
            if (pickedUpItem != null) return;
            pickedUpItem = item;
            item.GetComponent<Image>().raycastTarget = false;
            Vector2Int gridPos = currentGrid.GetItemGridPosition(item);
            item.transform.SetAsLastSibling(); //显示在最上面
            currentGrid.RemoveItem(gridPos);
        }

        public void PutDownItem()
        {
            if (currentGrid == null) return;
            if (pickedUpItem == null) return;

            Vector2Int gridPos = currentGrid.GetGridPosition(pickedUpItem);
            if(currentGrid.AddItem(pickedUpItem, gridPos))
            {
                pickedUpItem.GetComponent<Image>().raycastTarget = true;
                pickedUpItem = null;
            }
        }

        void Awake()
        {
            if (Instance != this)
            {
                DestroyImmediate(this);
                return;
            }
        }

        void Update()
        {
            if (pickedUpItem != null)
            {
                pickedUpItem.transform.position = Input.mousePosition;
            }
        }
    }
}