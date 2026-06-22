using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    public void AddExp(int amount)
    {
        PlayerDataManager.Instance.AddExp(amount);
    }

    public void AddCoin(int amount)
    {
        PlayerDataManager.Instance.AddCoin(amount);
    }
}
