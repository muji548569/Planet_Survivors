using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    private Transform planet;
    private Transform target;

    void Update()
    {
        Vector3 targetDir = target.position - transform.position;
        Vector3 normal = (transform.position - planet.position).normalized;
        Vector3 moveDir = Vector3.ProjectOnPlane(targetDir, normal).normalized;
        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 25);
        transform.position += moveDir * data.moveSpeed * Time.deltaTime; 
    }

    public void Init(Transform planet, Transform target)
    {
        this.planet = planet;
        this.target = target;
    }
}
