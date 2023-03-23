using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : SerializedMonoBehaviour
{
    static UIController _instance;

    public static UIController Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            if (_instance == null)
            {
                _instance = FindObjectOfType<UIController>();
            }

            return _instance;
        }
        private set => _instance = value;
    }
    
    [SerializeField] Dictionary<string, GameObject> _uis;

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
        InitUI();
        InputController.Instance.PlayerInput.UI.Inventory.performed += SwitchInventoryUI;
        InputController.Instance.PlayerInput.UI.Character.performed += SwitchCharacterUI;
        InputController.Instance.PlayerInput.UI.Load.performed += CloseLoad;
        InputController.Instance.PlayerInput.UI.Load.Enable();
    }

    void CloseLoad(InputAction.CallbackContext obj)
    {
        MainUI();
        InputController.Instance.PlayerInput.Enable();
        InputController.Instance.PlayerInput.UI.Load.Disable();
        
    }

    void SwitchCharacterUI(InputAction.CallbackContext obj)
    {
        SwitchUI("Character");
    }

    void SwitchInventoryUI(InputAction.CallbackContext obj)
    {
        SwitchUI("Inventory");
    }

    [ContextMenu("Init UI")]
    void InitUI()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            Transform ui = transform.GetChild(i);
            _uis.Add(ui.name, ui.gameObject);
        }
    }
    
    void CloseAllUI()
    {
        foreach ((var n, GameObject ui) in _uis)
        {
            ui.SetActive(false);
        }
    }

    public void MainUI()
    {
        CloseAllUI();
        OpenUI("Main");
    }
    
    public void OpenUI(string uiName)
    {
        CloseAllUI();
        _uis[uiName].SetActive(true);
    }

    public void SwitchUI(string uiName)
    {
        GameObject ui = _uis[uiName];
        if (ui.activeSelf)
        {
            MainUI();
        }
        else
        {
            CloseAllUI();
            OpenUI(uiName);
        }
    }
}