using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public bool IsInitialized { get; private set; }

    [SerializeField] private BasePanel[] panels;
    private Dictionary<E_PanelType, BasePanel> panelDict;
    // 利用Stack先進後出的特性 讓popup類型的panel也通過先進後出的方式來管理
    private Stack<E_PanelType> popupStack = new();

    // 是否存在激活的 popup panel
    public bool HasPopup => popupStack.Count > 0;
    // 目前在stack頂部的 popup panel
    // 在型別後面加 ? 可以讓該值可為空
    public E_PanelType? TopPopup
    {
        get
        {
            if (popupStack.Count <= 0)
                return null;

            return popupStack.Peek();
        }
    }

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

    public bool TryGetPanel(E_PanelType panelType, out BasePanel panel)
    {
        panel = null;
        if(panelDict == null)
        {
            Debug.LogError("[UIManager] UIManager 尚未初始化。");
            return false;
        }
        if(!panelDict.TryGetValue(panelType, out panel))
        {
            Debug.LogError($"[UIManager] 找不到 Panel: {panelType}");
            return false;
        }
        if(panel == null)
        {
            Debug.LogError($"[UIManager] Panel 已經是 null: {panelType}");
            return false;
        }

        return true;
    }

    private void ShowPanel(E_PanelType panelType)
    {
        if(!TryGetPanel(panelType, out BasePanel panel))
            return;

        panel.ShowPanel();
    }

    private void HidePanel(E_PanelType panelType)
    {
        if (!TryGetPanel(panelType, out BasePanel panel))
            return;

        panel.HidePanel();
    }

    public void SwitchScreen(E_PanelType panelType)
    {
        HideAllPanels();
        popupStack.Clear();

        ShowPanel(panelType);
    }

    public void OpenPopup(E_PanelType panelType)
    {
        if (popupStack.Contains(panelType))
        {
            Debug.LogWarning($"Popup 已經開啟: {panelType}");
            return;
        }

        ShowPanel(panelType);
        popupStack.Push(panelType);
    }

    public void ClosePopup(E_PanelType panelType)
    {
        if (popupStack.Count == 0)
        {
            HidePanel(panelType);
            return;
        }

        if(popupStack.Peek() != panelType)
        {
            Debug.LogError(
                $"無法關閉非最上層 Popup: {panelType}，" +
                $"目前最上層是: {popupStack.Peek()}"
                );

            return;
        }

        popupStack.Pop();
        HidePanel(panelType);
    }

    public bool CloseTopPopup()
    {
        if(popupStack.Count == 0)
            return false;

        E_PanelType panelType = popupStack.Pop();
        HidePanel(panelType);
        return true;
    }

    public void HideAllPanels()
    {
        foreach (BasePanel panel in panels)
        {
            if(panel == null) continue;
            panel.HidePanel();
        }

        popupStack.Clear();
    }
}
