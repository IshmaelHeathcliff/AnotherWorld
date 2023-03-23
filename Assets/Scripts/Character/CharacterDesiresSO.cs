using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Character
{
    [CreateAssetMenu(fileName = "newDesires", menuName = "ScriptObjects/Character/New Desires")]
    public class CharacterDesiresSO : SerializedScriptableObject
    {
        public readonly List<Desire> desires;
        public int maxCount = 10;

        public List<Desire> Add(Desire desire)
        {
            desires.Add(desire);
            while (desires.Count > maxCount)
            {
                desires.RemoveAt(0);
            }

            return desires;
        }
    }

    [Serializable]
    public class Desire
    {
        public string description;
        [Range(0, 100)] public float intensity;
        public Color color = new Color(0, 0, 0, 1);
        public int leftTime;
    }
}