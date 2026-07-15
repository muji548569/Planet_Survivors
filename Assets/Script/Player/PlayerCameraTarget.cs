using UnityEngine;

public class PlayerCameraTarget : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform planet;
    private Vector3 currentForward;

    private void Awake()
    {
        if(player == null || planet == null)
        {
            Debug.LogError("[PlayerCameraTarget] Player 或 Planet 尚未指定。");
            enabled = false;
            return;
        }

        Vector3 planetUp = GetPlanetUp();
        currentForward = Vector3.ProjectOnPlane(player.forward, planetUp).normalized;
    }

    private void LateUpdate()
    {
        Vector3 planetUp = GetPlanetUp();
        // 將上一幀的前方投影到新的球面切線上
        Vector3 projectedForward = Vector3.ProjectOnPlane(currentForward, planetUp);
        if (projectedForward.sqrMagnitude < 0.01f)
        {
            projectedForward = Vector3.ProjectOnPlane(player.forward, planetUp).normalized;
        }
        if (projectedForward.sqrMagnitude < 0.001f)
        {
            return;
        }
        projectedForward.Normalize();

        currentForward = projectedForward.normalized;

        transform.position = player.position;
        transform.rotation = Quaternion.LookRotation(currentForward, planetUp);
    }


    private Vector3 GetPlanetUp()
    {
        return (player.position - planet.position).normalized;
    }
}
