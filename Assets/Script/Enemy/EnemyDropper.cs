using UnityEngine;

public class EnemyDropper : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    public void Drop()
    {
        if (data == null) return;
        
        foreach(DropEntry drop in data.drops)
        {
            if (drop == null) continue;
            if (Random.value > drop.dropChance) continue;

            int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);

            for (int i = 0; i < amount; i++)
            {
                Vector3 spawnPos = GetDropPosition();

                PoolManager.Instance.Get(drop.prefab, spawnPos, transform.rotation);
            }
        }
    }

    private Vector3 GetDropPosition()
    {
        Vector3 surfaceNormal = transform.up;

        Vector3 tangentA = Vector3.Cross(surfaceNormal,
            Mathf.Abs(Vector3.Dot(surfaceNormal, Vector3.up)) > 0.99f
            ? Vector3.right
            : Vector3.up).normalized;

        Vector3 tangentB = Vector3.Cross(surfaceNormal, tangentA).normalized;

        Vector2 randomOffset = Random.insideUnitCircle * data.dropSpreadRadius;

        return transform.position 
            + tangentA * randomOffset.x 
            + tangentB * randomOffset.y 
            + surfaceNormal * 0.1f;     // 讓掉落物稍微浮在表面上 避免卡進地面
    }
}
