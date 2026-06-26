using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : BasePanel
{
    [SerializeField] private Button btnRestart;
    [SerializeField] private Button btnQuit;
    [SerializeField] private Text textTime;
    
    void Start()
    {
        btnRestart.onClick.AddListener(() =>
        {
            GameFlowManager.Instance.StartGame();
            GamePauseManager.Instance.ResumeGame();
        });
        btnQuit.onClick.AddListener(GameFlowManager.Instance.QuitToMainScene);
    }

    private void OnEnable()
    {
        SetTime(GameSessionManager.Instance.ElapsedTime);
    }

    public void SetTime(float time)
    {
        textTime.text = FormatTime(time);
    }

    private string FormatTime(float time)
    {
        int minute = Mathf.FloorToInt(time / 60f);
        int second = Mathf.FloorToInt(time % 60f);
        return $"存活時間: {minute:00}:{second:00}";
    }
}
