using UnityEngine;

public abstract class WeaponBase
{
    public WeaponData weaponData;
    public int level = 1;
    public Transform owner;
    public float timer;

    public WeaponBase(WeaponData weaponData, Transform owner)
    {
        this.weaponData = weaponData;
        this.owner = owner;
    }

    public void Tick(float deltaTime)
    {
        timer += deltaTime;

        // 確保角色數值初始化完成
        float attackSpeed = 1f;
        if (PlayerDataManager.Instance != null && 
            PlayerDataManager.Instance.Data != null && 
            PlayerDataManager.Instance.Data.Stat != null)
        {
            attackSpeed = PlayerDataManager.Instance.Data.Stat.attackSpeed;
        }
        attackSpeed = Mathf.Max(0.01f, attackSpeed);

        // 計算實際武器觸發間隔
        float effectiveCooldown = GetCooldown() / attackSpeed;

        if (timer > effectiveCooldown)
        {
            Attack();
            timer = 0;
        }
    }

    public void LevelUp()
    {
        level++;
        OnLevelUp();
    }

    public abstract void Attack();
    public abstract float GetCooldown();

    protected virtual void OnLevelUp() { }
}
