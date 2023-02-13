using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Items
{
    [RequireComponent(typeof(Image))]
    public class Item : MonoBehaviour, IPointerClickHandler
    {
        public ItemSO itemSO;

        Image _image;
        RectTransform _rectTransform;

        void InitItem()
        {
            _image.sprite = itemSO.image;
        }
        
        void Awake()
        {
            _image = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();
        }

        void Start()
        {
            InitItem();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Debug.Log("item clicked");
            InventoryController.Instance.PickUpItem(this);
        }
    }
}