using UnityEngine;

public class OrbitBullet : MonoBehaviour
{
    private float damage;
    private Transform owner;
    public void Init(float damage, Transform owner)
    {
        this.damage = damage;
        this.owner = owner;
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
