using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                    _instance = FindFirstObjectByType<CellFactory>();
                }
            
                return _instance;
            }
            private set => _instance = value;
        }

        public bool IsAssetsLoaded { get; private set; }
        public string key = "cell";
        [SerializeField] Dictionary<CellType, List<GameObject>> _cellPrefs;
        Dictionary<CellType, List<Cell>> _cells;
        
        
        AsyncOperationHandle<IList<GameObject>> _handle;

        [ContextMenu("Load Assets")]
        void Load()
        {
            Release();
            
            foreach (CellType value in Enum.GetValues(typeof(CellType)))
            {
                _cellPrefs.Add(value, new List<GameObject>());
                _cells.Add(value, new List<Cell>());
            }
            StartCoroutine(LoadAssets());
        }
        
        [ContextMenu("Release Assets")]
        public void Release()
        {
            _cellPrefs = new Dictionary<CellType, List<GameObject>>();
            _cells = new Dictionary<CellType, List<Cell>>();

            if(_handle.IsValid())
                Addressables.Release(_handle);

            IsAssetsLoaded = false;
        }

        public IEnumerator LoadAssets()
        {
            _handle = Addressables.LoadAssetsAsync<GameObject>(key,
                ad =>
                {
                    var cell = ad.GetComponent<Cell>();
                    _cellPrefs[cell.celType].Add(ad);
                    _cells[cell.celType].Add(cell);
                });

            while(!_handle.IsDone)
                yield return _handle;

            IsAssetsLoaded = true;
            Addressables.Release(_handle);
        }

        public GameObject CreatCell(CellType cellType)
        {
            var cellPrefs = _cellPrefs[cellType];
            var cells = _cells[cellType];
            var weights = (from cell in cells select cell.weight).ToList();
            var rd = Random.Range(0, weights.Sum());
            for (var i = 0; i < weights.Count; i++)
            {
                if (rd < weights[i])
                    return cellPrefs[i];
                rd -= weights[i];
            }
            return cellPrefs.Last();

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