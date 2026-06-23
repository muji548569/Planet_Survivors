using UnityEngine;

public abstract class BasePanel : MonoBehaviour
{
    [SerializeField] private E_PanelType panelType;
    public E_PanelType PanelType => panelType;
    public virtual void ShowPanel() 
    {
        gameObject.SetActive(true);
    }

    public virtual void HidePanel() 
    {
        gameObject.SetActive(false);
    }

    public virtual void InitPanel() { }
}

public enum E_PanelType
{
    Start,
    Game,
    GameOver,
    LevelUp,
    Setting,
    Pause,
}
