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

        Collider[] hits = Physics.OverlapSphere(transform.position, data.attractRange, playerLayer);
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
