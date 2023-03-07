using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Board
{
    public class CellFactory : SerializedMonoBehaviour
    {

        static CellFactory _instance;
        public static CellFactory Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
            
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CellFactory>();
                }
            
                return _instance;
            }
            private set => _instance = value;
        }

        public bool IsAssetsLoaded { get; private set; }
        public string key = "cell";
        public Dictionary<CellType, List<GameObject>> cells;
        
        AsyncOperationHandle<IList<GameObject>> _handle;

        [ContextMenu("Load Assets")]
        void Load()
        {
            Release();
            
            foreach (CellType value in Enum.GetValues(typeof(CellType)))
            {
                cells.Add(value, new List<GameObject>());
            }
            StartCoroutine(LoadAssets());
        }
        
        [ContextMenu("Release Assets")]
        public void Release()
        {
            cells = new Dictionary<CellType, List<GameObject>>();

            if(_handle.IsValid())
                Addressables.Release(_handle);

            IsAssetsLoaded = false;
        }

        public IEnumerator LoadAssets()
        {
            _handle = Addressables.LoadAssetsAsync<GameObject>(key,
                ad =>
                {
                    cells[ad.GetComponent<Cell>().celType].Add(ad);
                });

            while(!_handle.IsDone)
                yield return _handle;

            IsAssetsLoaded = true;
            Addressables.Release(_handle);
        }

        public GameObject CreatCell(CellType cellType)
        {
            var cells = this.cells[cellType];
            return cells[Random.Range(0, cells.Count)];
        }
        
        
        void Start()
        {
            Load();
        }

        void OnDestroy()
        {
            Release();
        }
    }
}