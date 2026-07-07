using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance { get; private set; }
    [SerializeField] private WeaponData[] startWeapons;
    [SerializeField] private Transform planet;

    // List用來遍歷會比較快 需要所有全部同時執行時會有優勢
    private List<WeaponBase> weapons = new();
    // Dictionary在查找上有優勢 
    private Dictionary<E_WeaponType, WeaponBase> weaponDic = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (startWeapons == null) return;

        foreach (WeaponData data in startWeapons)
        {
            if (data == null) continue; 
            AddWeapon(WeaponFactory.Create(data, transform, planet));
        }
    }

    void Update()
    {
        foreach (WeaponBase weapon in weapons)
        {
            weapon.Tick(Time.deltaTime);
        }
    }

    public void AddWeapon(WeaponBase weapon)
    {
        if (weapon == null) return;
        if (weapon.weaponData == null)
        {
            Debug.LogError("新增武器失敗：weaponData 是 null");
            return;
        }

        E_WeaponType weaponType = weapon.weaponData.weaponType;

        if (weaponDic.ContainsKey(weaponType))
        {
            Debug.LogWarning($"已持有武器，不能重複新增: {weaponType}");
            return;
        }
        weapons.Add(weapon);
        weaponDic.Add(weaponType, weapon);
    }

    public void UpgradeWeapon(E_WeaponType weaponType)
    {
        // 確定是否有該武器
        if(weaponDic.TryGetValue(weaponType, out WeaponBase weapon))
        {
            weapon.LevelUp();
            return;
        }

        // 若未持有 則新增武器
        WeaponData data = WeaponDataManager.Instance.GetWeaponData(weaponType);
        if (data == null) return;

        AddWeapon(WeaponFactory.Create(data, transform, planet));
    }

    public int GetNextWeaponLevel(E_WeaponType weaponType)
    {
        if (weaponDic.TryGetValue(weaponType, out WeaponBase weapon))
        {
            return weapon.level + 1;
        }

        return 1;
    }
}
