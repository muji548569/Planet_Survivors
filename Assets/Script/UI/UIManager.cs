using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private BasePanel[] panels;
    private Dictionary<E_PanelType, BasePanel> panelDict;
    public bool IsInitialized { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);   
    }

    public void Init()
    {
        if (IsInitialized) return;

        panelDict = new Dictionary<E_PanelType, BasePanel>();
        foreach (var panel in panels)
        {
            if (panel == null) continue;

            if (panelDict.ContainsKey(panel.PanelType))
            {
                Debug.LogError($"重複的 PanelType: {panel.PanelType}");
                continue;
            }

            panelDict.Add(panel.PanelType, panel);
            panel.HidePanel();
        }
        IsInitialized = true;
    }

    private void ShowPanel(E_PanelType panelType)
    {
        if(!panelDict.TryGetValue(panelType, out BasePanel panel))
        {
            Debug.LogError($"找不到Panel: {panelType}");
            return;
        }
        if (panel == null)
        {
            Debug.LogError($"Panel 已經是 null: {panelType}");
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
