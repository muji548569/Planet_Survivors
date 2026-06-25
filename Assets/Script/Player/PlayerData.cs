using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level = 1;
    public int currentExp;
    public int currentCoin;

    public float currentHp;
    public float maxHp = 1;

    public int baseExpToNextLevel = 10;
    public float expGrowthRate = 1.25f;
}
