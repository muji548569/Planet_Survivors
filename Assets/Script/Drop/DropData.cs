using UnityEngine;

[CreateAssetMenu(fileName = "DropData", menuName = "Drop/DropData")]
public class DropData : ScriptableObject
{
    public E_DropType dropType;
    public int amount;

    public float attractRange;
    public float moveSpeed;
}

public enum E_DropType
{
    Coin,
    Exp,
    Heart,
}
