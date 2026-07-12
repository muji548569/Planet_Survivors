using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryPanel : BasePanel
{
    [SerializeField] private Text textScore;
    [SerializeField] private Button btnRestart;
    [SerializeField] private Button btnQuit;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Level Item")]
    [SerializeField] private UILevelItem levelItemPrefab;
    [SerializeField] private RectTransform content;

    private const int ItemsPerRow = 7;
    private const float StartX = 100f;
    private const float StartY = -50f;
    private const float SpacingX = 150f;
    private const float SpacingY = 150f;


    private void Start()
    {
        btnQuit.onClick.AddListener(() =>
        {
            GameFlowManager.Instance.QuitToMainScene();
        });
        btnRestart.onClick.AddListener(() =>
        {
            GameFlowManager.Instance.StartGame();
            GamePauseManager.Instance.ResumeGame();
        });
    }

    public override void ShowPanel()
    {
        base.ShowPanel();

        GetLevelItems();
        GetScore();

        // 每次打開時讓捲動位置回到頂端
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public override void HidePanel()
    {
        base.HidePanel();
        ClearLevelItems();
    }

    /// <summary>
    /// 得到所有獲得過的屬性跟武器
    /// </summary>
    private void GetLevelItems()
    {
        if (PlayerDataManager.Instance == null) 
        {
            Debug.LogError($"[WinPanel] 找不到角色資料管理器或角色資料");
            return;
        }
        if (WeaponController.Instance == null)
        {
            Debug.LogError($"[WinPanel] 找不到武器管理器或武器資料");
            return;
        }

        foreach (E_PlayerStat stat in System.Enum.GetValues(typeof(E_PlayerStat)))
        {
            int level = PlayerDataManager.Instance.GetStatLevel(stat);
            if (level <= 0) continue;

            Sprite icon = PlayerConfigDataManager.Instance.GetStatIcon(stat);

            CreateLevelItem(icon, level);
        }

        foreach (E_WeaponType weapon in System.Enum.GetValues(typeof(E_WeaponType)))
        {
            int level = WeaponController.Instance.GetWeaponLevel(weapon);
            if (level <= 0) continue;

            Sprite icon = WeaponDataManager.Instance.GetWeaponIcon(weapon);

            CreateLevelItem(icon, level);
        }
    }

    /// <summary>
    /// 創建屬性等級物件
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="level"></param>
    private void CreateLevelItem(Sprite icon, int level)
    {
        UILevelItem item = Instantiate(levelItemPrefab, content);
        item.UpdateInfo(icon, level);

        int index = content.childCount - 1;
        int column = index % ItemsPerRow;
        int row = index / ItemsPerRow;

        RectTransform itmeRect = item.GetComponent<RectTransform>();
        itmeRect.anchoredPosition = new Vector2(StartX + column * SpacingX, StartY - row * SpacingY);

        UpdateContentHeight(row + 1);
    }

    private void UpdateContentHeight(int rowCount)
    {
        float requiredHeight = Mathf.Abs(StartY) + rowCount * SpacingY;
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, requiredHeight);
    }

    public void ClearLevelItems()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    public void GetScore()
    {
        int score = PlayerDataManager.Instance.Data.level * 100 +
                    PlayerDataManager.Instance.Data.currentCoin * 10;

        textScore.text = score.ToString();
    }
}
