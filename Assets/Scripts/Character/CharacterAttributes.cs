using UnityEngine;

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
        
        void Awake()
        {
            if (Instance != this)
            {
                DestroyImmediate(this);
                return;
            }
        }

        
    }
}