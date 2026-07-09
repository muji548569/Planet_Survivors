using UnityEngine;
using UnityEngine.InputSystem;

public class GamePauseManager : MonoBehaviour
{
    public static GamePauseManager Instance { get; private set; }
    public bool IsPaused { get; private set; }
    private bool canPause;
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

    public void EnablePause()
    {
        canPause = true;
        inputAction.Game.Enable();
        inputAction.Game.Pause.performed += OnPause;
    }

    public void DisablePause()
    {
        canPause = false;

        if(IsPaused)
            ResumeGame();

        inputAction.Game.Pause.performed -= OnPause;
        inputAction.Game.Disable();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (!canPause) return;
        TogglePausePanel();
    }

    public void TogglePausePanel()
    {
        if (!canPause) return;
        if (IsPaused)
        {
            ResumeGame();
            UIManager.Instance.ClosePopup(E_PanelType.Pause);
        }
        else
        {
            PauseGame();
            UIManager.Instance.OpenPopup(E_PanelType.Pause);
        }
            
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
    }
}
