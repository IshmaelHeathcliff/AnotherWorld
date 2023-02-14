using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Character
{
    [CreateAssetMenu(fileName = "newAttributes", menuName = "Character/New Attributes")]
    public class CharacterAttributesSO : SerializedScriptableObject
    {
        public Dictionary<string, float> main;
        public Dictionary<string, float> maxMain;
        
        public Dictionary<string, float> nutrition;
        public float maxNutrition;

    }
}
