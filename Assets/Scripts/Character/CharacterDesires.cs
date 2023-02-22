using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Mathf;

namespace Character
{
    public class CharacterDesires : MonoBehaviour
    {
        static CharacterDesires _instance;
        public static CharacterDesires Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                if (_instance == null)
                {
                    _instance = FindObjectOfType<CharacterDesires>();
                }

                return _instance;
            }
            private set => _instance = value;
        }
        
        public CharacterDesiresSO desiresSO;
        
        public GameObject desiresUI;
        public GameObject desirePrefab;

        // test
        // public GameObject desireTestA;
        // public GameObject desireTestB;
        // public GameObject desireTestC;
        
        void Awake()
        {
            if (Instance != this)
            {
                DestroyImmediate(this);
            }
        }

        void Start()
        {
            UpdateDesireUI();
        }

        void UpdateUI()
        {
        }
        
        [ContextMenu("Update Desire UI")]
        void UpdateDesireUI()
        {
            ClearDesireUI();

            var count = 0;
            foreach (Desire desire in desiresSO.desires)
            {
                var obj = Instantiate(desirePrefab, desiresUI.transform);
                
                obj.name = $"Desire ({count})";
                count += 1;
                
                var tf = obj.GetComponent<RectTransform>();
                var img = obj.GetComponent<Image>();
                
                tf.sizeDelta *= desire.intensity / 100 + 0.5f;
                img.color = desire.color;
            }
        }

        // [ContextMenu("Clear Desire UI")]
        void ClearDesireUI()
        {
            while(desiresUI.transform.childCount > 0)
            {
                Transform child = desiresUI.transform.GetChild(0);
                DestroyImmediate(child.gameObject);
            }
        }
        
        [ContextMenu("Sort Desire UI")]
        void SortDesireUI()
        {
            var desires = new List<RectTransform>();
            Transform uiTransform = desiresUI.transform;
            
            for (var i = 0; i < uiTransform.childCount; i++)
            {
                desires.Add(uiTransform.GetChild(i).GetComponent<RectTransform>());
            }
            
            var desirePositions = new Queue<int[]>();
            
            desires[0].localPosition = new Vector3(0, 0);
            var r = (desires[0].sizeDelta.x + desires[1].sizeDelta.x) / 2;
            var t = Random.Range(0, 2*PI);
            desires[1].localPosition = desires[0].localPosition + new Vector3(r * Cos(t), r * Sin(t));
            
            // 0为右，1为左
            desirePositions.Enqueue(new []{0, 1, 0});
            desirePositions.Enqueue(new []{0, 1, 1});

            for (var i = 2; i < desires.Count; i++)
            {
                var confirmed = false;

                var posPair = new int[] {};
                var localPos = new Vector3();

                while (!confirmed)
                {
                    posPair = desirePositions.Dequeue();
                    localPos = CalcDesirePos(desires[posPair[0]], desires[posPair[1]], desires[i].sizeDelta.x/2)[posPair[2]];
                    for (var j = 0; j < i; j++)
                    {
                        RectTransform rt = desires[j];
                        if (Vector3.Distance(rt.localPosition, localPos) 
                            < (rt.sizeDelta.x + desires[i].sizeDelta.x) / 2 - 0.5f)
                        {
                            desirePositions.Enqueue(posPair);
                            confirmed = false;
                            break;
                        }

                        confirmed = true;
                    }
                }

                desires[i].localPosition = localPos;
                RectTransform rt1 = desires[posPair[0]];
                RectTransform rt2 = desires[posPair[1]];

                desirePositions.Enqueue(IsRightOfLine(rt1.localPosition, localPos, rt2.localPosition)
                    ? new[] {i, posPair[1], 1}
                    : new[] {i, posPair[1], 0});

                desirePositions.Enqueue(IsRightOfLine(rt2.localPosition, localPos, rt1.localPosition)
                    ? new[] {i, posPair[0], 1}
                    : new[] {i, posPair[0], 0});
            }
        }

        Vector3[] CalcDesirePos(RectTransform desireA, RectTransform desireB, float r3)
        {
            var a = desireA.localPosition.x;
            var b = desireA.localPosition.y;
            var c = desireB.localPosition.x;
            var d = desireB.localPosition.y;
            var r1 = desireA.sizeDelta.x / 2;
            var r2 = desireB.sizeDelta.x / 2;
            
            var l = r1 + r3;
            var m = r2 + r3;
            
            var pos1 = new Vector3();
            var pos2 = new Vector3();

            if (Abs(b - d) + Abs(a - c) < Epsilon) return null;
            
            if(Abs(b - d) > Epsilon)
            {
                var T = (l * l - m * m + c * c + d * d - a * a - b * b) / 2 / (d - b);
                var Y = (a - c) / (d - b);
                var A = 1 + Y * Y;
                var B = 2 * (T - b) * Y - 2 * a;
                var C = a * a + Pow(T - b, 2) - l * l;
                
                var D = B * B - 4 * A * C;

                if (D < 0) return null;
                
                pos1.x = (-B + Sqrt(D)) / 2 / A;
                pos2.x = (-B - Sqrt(D)) / 2 / A;
                pos1.y = T + Y * pos1.x;
                pos2.y = T + Y * pos2.x;
                return new[] {pos1, pos2};

            }
            else 
            {
                var x = (l * l - m * m) / 2 / (c - a) + (a + c) / 2;
                var D = l * l - Pow(x - a, 2);
                
                if (D < 0) return null;
                
                pos1.x = x;
                pos2.x = x;
                pos1.y = b + Sqrt(D);
                pos2.y = b - Sqrt(D);
                return new[] {pos1, pos2};
            }
        }

        
        // True在右，False在左
        bool IsRightOfLine(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            var a = point.x;
            var b = point.y;
            var c = linePoint1.x;
            var d = linePoint1.y;
            var e = linePoint2.x;
            var f = linePoint2.y;

            var x = (b - d) * (e - c) / (f - d) + c;
            return x <= a;
        }
    }
}