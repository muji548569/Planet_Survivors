using UnityEngine;

public class DropItem : MonoBehaviour, IPoolable
{
    [SerializeField] private DropData data;
    [SerializeField] private LayerMask playerLayer;

    private readonly Collider[] playerHits = new Collider[1];

    private Transform targetPlayer;
    private bool isAttracking;
    private bool isCollected;
    private PoolableObject poolable;

    

    private void Awake()
    {
        poolable = GetComponent<PoolableObject>();
        if (poolable == null)
        {
            Debug.LogError($"[DropItem] {name} 缺少 PoolableObject 元件。", this);
        }
    }

    void Update()
    {
        if (isCollected) return;

        FindPlayerInRange();

        if (isAttracking && targetPlayer != null)
        {
            MoveToPlayer();
        }
    }

    private void FindPlayerInRange()
    {
        if (isAttracking || isCollected) return;

        // 確保角色數值初始化完成
        float pickupRange = 1f;

        if (PlayerDataManager.Instance?.Data?.Stat != null)
        {
            pickupRange = PlayerDataManager.Instance.Data.Stat.pickupRange;
        }

        // 計算有效吸取範圍
        // 掉落物基礎吸取範圍 × 玩家拾取倍率
        float effectiveAttractRange = data.attractRange * Mathf.Max(1f, pickupRange);

        // 範圍檢測
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, effectiveAttractRange, playerHits ,playerLayer);
        
        if (hitCount <= 0) return;

        PlayerController player = playerHits[0].GetComponentInParent<PlayerController>();
        if(player == null) return;
        
        targetPlayer = player.transform;
        isAttracking = true;
    }

    private void MoveToPlayer()
    {
        Vector3 dir = (targetPlayer.position - transform.position);
        if (dir.sqrMagnitude <= 0.0001f) return;

        transform.position += dir.normalized * data.moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        PlayerCollector player = other.GetComponentInParent<PlayerCollector>();
        if (player == null) return;

        isCollected = true;

        switch (data.dropType)
        {
            case E_DropType.Coin:
                player.AddCoin(data.amount);
                break;
            case E_DropType.Exp:
                player.AddExp(data.amount);
                break;
        }

        poolable.Release();
    }

    public void OnSpawnFromPool()
    {
        targetPlayer = null;
        isAttracking = false;
        isCollected = false;
    }

    public void OnReturnToPool()
    {
        targetPlayer = null;
        isAttracking = false;
        isCollected = true;
    }
}
