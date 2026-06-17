using UnityEngine;

public abstract class BasePanel : MonoBehaviour
{
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
