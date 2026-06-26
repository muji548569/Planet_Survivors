using UnityEngine;

public class EnemyContact : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    private float timer;

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (timer > 0) return;

        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(data.contactDamage);
            print("造成傷害");
        }

        timer = data.attackCooldown;
    }
}
