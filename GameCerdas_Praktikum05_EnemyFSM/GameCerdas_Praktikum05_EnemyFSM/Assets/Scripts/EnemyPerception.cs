using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Vision")]
    [SerializeField] private float visionRange = 10f;
    [SerializeField, Range(0f, 360f)]
    private float visionAngle = 90f;

    [SerializeField] private float eyeHeight = 1f;

    [Header("Layer")]
    [SerializeField] private LayerMask obstacleMask;

    public bool CanSeePlayer { get; private set; }

    public float DistanceToPlayer
    {
        get
        {
            if (player == null)
                return Mathf.Infinity;

            return Vector3.Distance(
                transform.position,
                player.position
            );
        }
    }

    private void Update()
    {
        CanSeePlayer = CheckPlayerVisibility();
    }

    private bool CheckPlayerVisibility()
    {
        if (player == null)
            return false;

        Vector3 origin =
            transform.position +
            Vector3.up * eyeHeight;

        Vector3 target =
            player.position +
            Vector3.up * 0.8f;

        Vector3 direction =
            target - origin;

        float distance = direction.magnitude;

        // 1. Periksa jarak
        if (distance > visionRange)
            return false;

        // 2. Periksa sudut pandang
        float angle = Vector3.Angle(
            transform.forward,
            direction
        );

        if (angle > visionAngle * 0.5f)
            return false;

        // 3. Periksa obstacle
        bool blocked = Physics.Raycast(
            origin,
            direction.normalized,
            distance,
            obstacleMask
        );

        if (blocked)
            return false;

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            visionRange
        );

        Vector3 leftDirection =
            Quaternion.Euler(
                0f,
                -visionAngle * 0.5f,
                0f
            ) * transform.forward;

        Vector3 rightDirection =
            Quaternion.Euler(
                0f,
                visionAngle * 0.5f,
                0f
            ) * transform.forward;

        Gizmos.DrawRay(
            transform.position,
            leftDirection * visionRange
        );

        Gizmos.DrawRay(
            transform.position,
            rightDirection * visionRange
        );
    }
}

