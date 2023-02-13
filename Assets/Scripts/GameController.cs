using UnityEngine;

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
        // Pause();
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