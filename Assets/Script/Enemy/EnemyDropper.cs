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
                Vector3 offset = Random.insideUnitSphere * data.dropSpreadRadius;
                Vector3 spawnPos = transform.position + offset;

                Instantiate(drop.prefab, spawnPos, Quaternion.identity);
            }
        }
    }
}
