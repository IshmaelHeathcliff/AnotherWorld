using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Character
{
    public class CharacterAttributes : MonoBehaviour
    {
        static CharacterAttributes _instance;

        public static CharacterAttributes Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                if (_instance == null)
                {
                    _instance = FindObjectOfType<CharacterAttributes>();
                }

                if (_instance == null)
                {
                    var obj = new GameObject("CharacterAttributes");
                    _instance = obj.AddComponent<CharacterAttributes>();
                }

                return _instance;
            }
            private set => _instance = value;
        }

        public CharacterAttributesSO attributes;

        public GameObject mainAttributes;
        public GameObject nutritionAttributes;

        readonly List<string> _mainAttributeNames = new()
        {
            "Strength",
            "Dexterity",
            "Intelligence",
            "Sociability"
        };

        readonly List<string> _nutritionAttributeNames = new()
        {
            "Water",
            "Sugar",
            "Protein",
            "Vegetable"
        };

        List<Slider> _sliders = new();

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
            var mainSliders = mainAttributes.GetComponentsInChildren<Slider>();
            var nutritionSliders = nutritionAttributes.GetComponentsInChildren<Slider>();
            // var attributeNames = _mainAttributeNames.Concat(_nutritionAttributeNames).ToList();

            _sliders = mainSliders.Concat(nutritionSliders).ToList();
            
            UpdateAttributeUI();
        }

        [ContextMenu("Update Attribute UI")]
        void UpdateAttributeUI()
        {
            UpdateMainAttributeUI();
            UpdateNutritionAttributeUI();
        }

        void UpdateMainAttributeUI()
        {
            foreach (var a in attributes.maxMain)
            {
                foreach (Slider slider in _sliders.Where(slider => slider.name == a.Key))
                {
                    slider.maxValue = a.Value;
                }
            }
            
            foreach (var a in attributes.main)
            {
                foreach (Slider slider in _sliders.Where(slider => slider.name == a.Key))
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
                foreach (Slider slider in _sliders.Where(slider => slider.name == a.Key))
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