using UnityEngine;
using UnityEngine.InputSystem;

public class GamePauseManager : MonoBehaviour
{
    public static GamePauseManager Instance { get; private set; }
    public bool IsPaused { get; private set; }
    private PlayerInputActions inputAction;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        inputAction = new PlayerInputActions();
    }

    public void OnEnable()
    {
        inputAction.Game.Enable();
        inputAction.Game.Pause.performed += OnPause;
    }

    public void OnDisable()
    {
        inputAction.Game.Pause.performed -= OnPause;
        inputAction.Game.Disable();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if(IsPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        UIManager.Instance.OpenPopup(E_PanelType.Pause);
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        UIManager.Instance.ClosePopup(E_PanelType.Pause);
    }
}
