using Sirenix.OdinInspector;
using UnityEngine;

namespace Character
{
    [CreateAssetMenu(fileName = "newAttributes", menuName = "Character/New Attributes")]
    public class CharacterAttributesSO : SerializedScriptableObject
    {
        public float maxStrength;
        public float maxDexterity;
        public float maxIntelligence;
        public float maxSociability;
        public float strength;
        public float dexterity;
        public float intelligence;
        public float sociability;
    }
}
