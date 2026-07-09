using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private DropData data;
    [SerializeField] private LayerMask playerLayer;
    private Transform targetPlayer;
    private bool isAttracking;

    void Update()
    {
        FindPlayerInRange();
        if (isAttracking && targetPlayer != null)
        {
            MoveToPlayer();
        }
    }

    private void FindPlayerInRange()
    {
        if (isAttracking) return;

        // 確保角色數值初始化完成
        float pickupRange = 1f;
        if (PlayerDataManager.Instance != null && 
            PlayerDataManager.Instance.Data != null && 
            PlayerDataManager.Instance.Data.Stat != null)
        {
            pickupRange = PlayerDataManager.Instance.Data.Stat.pickupRange;
        }
        // 計算有效吸取範圍
        // 掉落物基礎吸取範圍 × 玩家拾取倍率
        float effectiveAttractRange = data.attractRange * Mathf.Max(1f, pickupRange);

        // 範圍檢測
        Collider[] hits = Physics.OverlapSphere(transform.position, effectiveAttractRange, playerLayer);
        if(hits.Length > 0 )
        {
            targetPlayer = hits[0].transform;
            isAttracking = true;
        }
    }

    private void MoveToPlayer()
    {
        Vector3 dir = (targetPlayer.position - transform.position).normalized;
        transform.position += dir * data.moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCollector player = other.GetComponent<PlayerCollector>();
        if (player == null) return;

        switch (data.dropType)
        {
            case E_DropType.Coin:
                player.AddCoin(data.amount);
                break;
            case E_DropType.Exp:
                player.AddExp(data.amount);
                break;
        }

        Destroy(gameObject);
    }
}
