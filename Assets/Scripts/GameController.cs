using UnityEngine;
using System.Collections;
using Board;

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

    [SerializeField] GameObject loadingUI;

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
        loadingUI.SetActive(true);
        Application.targetFrameRate = 100;
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        Pause();
        yield return StartCoroutine(WorldBoard.Instance.InitBoard());
        Character.Character.Instance.InitCharacter();
        loadingUI.SetActive(false);
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