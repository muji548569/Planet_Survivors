using UnityEngine;

[System.Serializable]
public class DropEntry
{
    public GameObject prefab;

    [Range(0f, 1f)] 
    public float dropChance = 1f;

    public int minAmount;
    public int maxAmount;
}
