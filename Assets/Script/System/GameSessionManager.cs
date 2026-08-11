using System;
using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }
    public float ElapsedTime { get; private set; }
    public bool IsPlaying { get; private set; }

    public event Action<float> OnTimeChanged;
    public event Action OnGameWin;
    public event Action OnGameLose;

    [SerializeField] private float WinGameTime = 300f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPlaying) return;

        ElapsedTime += Time.deltaTime;
        OnTimeChanged?.Invoke(ElapsedTime);

        if(ElapsedTime >= WinGameTime)
        {
            WinGame();
        }
    }

    public void StartSession()
    {
        ElapsedTime = 0;
        IsPlaying = true;
    }

    public void EndSession()
    {
        IsPlaying = false;
    }

    public string GetFormattedTime()
    {
        int minute = Mathf.FloorToInt(ElapsedTime / 60f);
        int second = Mathf.FloorToInt(ElapsedTime % 60f);
        return $"{minute:00}:{second:00}";
    }

    private void WinGame()
    {
        if (!IsPlaying) return;

        IsPlaying = false;

        OnGameWin?.Invoke();
    }

    public void LoseGame()
    {
        if(!IsPlaying) return;

        IsPlaying = false;

        OnGameLose?.Invoke();
    }
}
