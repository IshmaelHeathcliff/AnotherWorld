using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Character
{
    [CreateAssetMenu(fileName = "newAttributes", menuName = "Character/New Attributes")]
    public class CharacterAttributesSO : SerializedScriptableObject
    {
        public Dictionary<CharacterAttributesSystem.CharacterAttributes, float> main;
        public Dictionary<CharacterAttributesSystem.CharacterAttributes, float> maxMain;
        
        public Dictionary<CharacterAttributesSystem.CharacterNutrition, float> nutrition;
        public float maxNutrition;
    }
}
