using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Transform = UnityEngine.Transform;

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
                    _instance = FindFirstObjectByType<CharacterAttributesSystem>();
                }

                return _instance;
            }
            private set => _instance = value;
        }

        public enum CharacterAttributes
        {
            Strength,
            Dexterity,
            Intelligence,
            Sociability,
        }

        public enum CharacterNutrition
        {
            Protein,
            Sugar,
            Water,
            Vegetable,
        }

        [SerializeField] CharacterAttributesSO attributes;

        public GameObject mainAttributes;
        public GameObject nutritionAttributes;
        public GameObject nutritionAmount;

        List<Transform> _mainTransforms;
        List<Transform> _nutritionTransforms;

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
            _mainTransforms = mainAttributes.GetComponentsInChildren<Transform>().ToList();
            _nutritionTransforms = nutritionAttributes.GetComponentsInChildren<Transform>().ToList();

            UpdateMainAttributeUI();
            UpdateNutritionAttributeUI();
        }
        
        

        [ContextMenu("Update Attribute UI")]
        void UpdateAttributeUI()
        {
            _mainTransforms = mainAttributes.GetComponentsInChildren<Transform>().ToList();
            _nutritionTransforms = nutritionAttributes.GetComponentsInChildren<Transform>().ToList();
            UpdateMainAttributeUI();
            UpdateNutritionAttributeUI();
        }

        // Slider Name与CharacterAttributes强绑定
        void UpdateMainAttributeUI()
        {
            foreach (CharacterAttributes attributeType in Enum.GetValues(typeof(CharacterAttributes)))
            {
                foreach (Transform t in _mainTransforms.Where(t => t.name == attributeType.ToString()))
                {
                    var slider = t.GetComponent<Slider>();
                    var text = t.GetComponentInChildren<TextMeshProUGUI>();
                    var attribute = attributes.main[attributeType];
                    var maxAttribute = attributes.maxMain[attributeType];
                    slider.value = attribute;
                    slider.maxValue = maxAttribute;
                    text.text = $"{attribute:f0}/{maxAttribute:f0}";
                }
            }
        }

        void UpdateNutritionAttributeUI()
        {
            float sum = 0;
            float r = 75f;
            var image = nutritionAmount.GetComponent<Image>();
            var amountText = nutritionAmount.GetComponentInChildren<TextMeshProUGUI>();

            var allAmount = attributes.nutrition.Values.Sum();
            var maxNutrition = allAmount < attributes.maxNutrition? attributes.maxNutrition : allAmount;

            foreach ((CharacterNutrition nutritionType, var value) in attributes.nutrition)
            {
                foreach (Transform t in _nutritionTransforms.Where(t => t.name == nutritionType.ToString()))
                {
                    t.SetAsFirstSibling();
                    var slider = t.GetComponent<Slider>();
                    var text = t.GetComponentInChildren<TextMeshProUGUI>();
                    slider.maxValue = maxNutrition;
                    slider.value = value + sum;
                    sum = slider.value;

                    text.text = $"{value:f0}";
                    var theta = (1.5f - slider.value / slider.maxValue * 2f) * Mathf.PI + 0.25f;
                    text.transform.localPosition = r * new Vector3(Mathf.Cos(theta), Mathf.Sin(theta));
                }
            }

            if (allAmount < attributes.maxNutrition)
            {
                image.color = Color.green;
                amountText.text = "";
            }
            else
            {
                image.color = Color.red;
                amountText.text = $"{allAmount:f0}";
            }
        }

        public void UpdateMainAttribute(CharacterAttributes attributeType, float value, bool onMax = false)
        {
            var attribute = attributes.main[attributeType];
            var maxAttribute = attributes.maxMain[attributeType];

            if (onMax)
            {
                maxAttribute += value;
                if (maxAttribute < 0) maxAttribute = 0;
                else if (attribute > maxAttribute) attribute = maxAttribute;
            }
            else
            {
                attribute += value;
                if (attribute < 0) attribute = 0;
                else if (attribute > maxAttribute) attribute = maxAttribute;
            }
            
            attributes.maxMain[attributeType] = maxAttribute;
            attributes.main[attributeType] = attribute;
            UpdateMainAttributeUI();
        }

        public void UpdateNutritionAttribute(CharacterNutrition nutritionType, float value)
        {
            var nutrition = attributes.nutrition[nutritionType];
            nutrition += value;
            if (nutrition < 0) nutrition = 0;

            attributes.nutrition[nutritionType] = nutrition;
            UpdateNutritionAttributeUI();
        }
    }
}