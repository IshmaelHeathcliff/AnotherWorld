using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Character
{
    public class CharacterAttributesSystem : MonoBehaviour
    {
        static CharacterAttributesSystem _instance;

        public static CharacterAttributesSystem Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                if (_instance == null)
                {
                    _instance = FindObjectOfType<CharacterAttributesSystem>();
                }

                return _instance;
            }
            private set => _instance = value;
        }

        public CharacterAttributesSO attributes;

        public GameObject mainAttributes;
        public GameObject nutritionAttributes;

        List<Slider> _mainSliders;
        List<Slider> _nutritionSliders;

        void Awake()
        {
            if (Instance != this)
            {
                DestroyImmediate(this);
                return;
            }
        }

        void Start()
        {
            _mainSliders = mainAttributes.GetComponentsInChildren<Slider>().ToList();
            _nutritionSliders = nutritionAttributes.GetComponentsInChildren<Slider>().ToList();
            
            UpdateMainAttributeUI();
            UpdateNutritionAttributeUI();
        }

        [ContextMenu("Update Attribute UI")]
        void UpdateAttributeUI()
        {
            _mainSliders = mainAttributes.GetComponentsInChildren<Slider>().ToList();
            _nutritionSliders = nutritionAttributes.GetComponentsInChildren<Slider>().ToList();
            UpdateMainAttributeUI();
            UpdateNutritionAttributeUI();
        }

        void UpdateMainAttributeUI()
        {
            foreach (var a in attributes.maxMain)
            {
                foreach (Slider slider in _mainSliders.Where(slider => slider.name == a.Key))
                {
                    slider.maxValue = a.Value;
                }
            }
            
            foreach (var a in attributes.main)
            {
                foreach (Slider slider in _mainSliders.Where(slider => slider.name == a.Key))
                {
                    slider.value = a.Value;
                }
            }
        }

        void UpdateNutritionAttributeUI()
        {
            float sum = 0;

            foreach (var a in attributes.nutrition)
            {
                foreach (Slider slider in _nutritionSliders.Where(slider => slider.name == a.Key))
                {
                    slider.transform.SetAsFirstSibling();
                    slider.maxValue = attributes.maxNutrition;
                    slider.value = a.Value + sum;
                    sum = slider.value;
                }
            }
        }
    }
}