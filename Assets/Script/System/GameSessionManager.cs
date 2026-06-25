using System;
using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }
    public float ElapsedTime { get; private set; }
    public bool IsPlaying { get; private set; }

    public event Action<float> OnTimeChanged;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPlaying) return;
        ElapsedTime += Time.deltaTime;
        OnTimeChanged?.Invoke(ElapsedTime);
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
}
