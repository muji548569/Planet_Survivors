using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    [SerializeField] private Transform planet;
    private GameObject target;
    private void Start()
    {
        target = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        Vector3 targetDir = target.transform.position - transform.position;
        Vector3 normal = (transform.position - planet.position).normalized;
        Vector3 moveDir = Vector3.ProjectOnPlane(targetDir, normal).normalized;
        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 25);
        transform.position += moveDir * data.moveSpeed * Time.deltaTime; 
    }
}
