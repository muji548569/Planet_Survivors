using UnityEngine;

public class SwordSlash : MonoBehaviour
{
    private float damage;
    private Transform owner;
    private float lifetime;

    /// <summary>
    /// 提供給外部初始化數值
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="owner"></param>
    /// <param name="lifetime"></param>
    public void Init(float damage, Transform owner, float lifetime)
    {
        this.damage = damage;
        this.owner = owner;
        this.lifetime = lifetime;

        Destroy(this.gameObject, this.lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 敵人受傷邏輯
            print($"{owner.name}對{other.name}造成{damage}點傷害");
        }
    }
}
