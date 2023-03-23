using UnityEngine;
using System.Collections;
using Board;
using TMPro;

public class GameController : MonoBehaviour
{
    static GameController _instance;

    public static GameController Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            if (_instance == null)
            {
                _instance = FindObjectOfType<GameController>();
            }

            if (_instance == null)
            {
                var obj = new GameObject("GameController");
                _instance = obj.AddComponent<GameController>();
            }

            return _instance;
        }
        private set => _instance = value;
    }

    public bool isPaused;
    [SerializeField] string loadingText = "Loading...";
    [SerializeField] string loadedText = "空格键开始";

    [SerializeField] GameObject loadTextUI;
    TextMeshProUGUI _loadText;

    void Awake()
    {
        if (Instance != this)
        {
            DestroyImmediate(this);
            return;
        }

        _loadText = loadTextUI.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        UIController.Instance.OpenUI("Load");
        _loadText.text = loadingText;
        Application.targetFrameRate = 100;
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        Pause();
        yield return StartCoroutine(WorldBoard.Instance.InitBoard());
        Character.Character.Instance.InitCharacter();
        _loadText.text = loadedText;
        Continue();
    }
    
    public void Pause()
    {
        Time.timeScale = 0;
        isPaused = true;
    }

    public void Continue()
    {
        Time.timeScale = 1;
        isPaused = false;
    }
        
}