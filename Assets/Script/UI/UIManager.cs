using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private BasePanel[] panels;
    private Dictionary<E_PanelType, BasePanel> panelDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        panelDict = new Dictionary<E_PanelType, BasePanel>();
        foreach (var panel in panels)
        {
            panelDict.Add(panel.PanelType, panel);
            panel.HidePanel();
        }
    }

    private void ShowPanel(E_PanelType panelType)
    {
        if(!panelDict.TryGetValue(panelType, out BasePanel panel))
        {
            Debug.LogError($"找不到Panel: {panelType}");
            return;
        }

        panel.ShowPanel();
    }

    private void HidePanel(E_PanelType panelType)
    {
        if (!panelDict.TryGetValue(panelType, out BasePanel panel))
        {
            Debug.LogError($"找不到Panel: {panelType}");
            return;
        }

        panel.HidePanel();
    }

    public void SwitchScreen(E_PanelType panelType)
    {
        HideAllPanels();
        ShowPanel(panelType);
    }

    public void OpenPopup(E_PanelType panelType)
    {
        ShowPanel(panelType);
    }

    public void ClosePopup(E_PanelType panelType)
    {
        HidePanel(panelType);
    }

    public void HideAllPanels()
    {
        foreach (BasePanel panel in panels)
        {
            panel.HidePanel();
        }
    }
}
