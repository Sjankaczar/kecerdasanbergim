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

    public float VisionAngle => visionAngle;

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

    public bool IsInViewCone(
        Transform target,
        float range)
    {
        if (target == null)
            return false;

        Vector3 origin =
            transform.position +
            Vector3.up * eyeHeight;

        Vector3 targetPosition =
            target.position +
            Vector3.up * 0.8f;

        Vector3 direction =
            targetPosition - origin;

        float distance = direction.magnitude;

        if (distance > range)
            return false;

        float angle = Vector3.Angle(
            transform.forward,
            direction
        );

        if (angle > visionAngle * 0.5f)
            return false;

        bool blocked = Physics.Raycast(
            origin,
            direction.normalized,
            distance,
            obstacleMask
        );

        return !blocked;
    }

    private void Update()
    {
        CanSeePlayer = CheckPlayerVisibility();
    }

    private bool CheckPlayerVisibility()
    {
        return IsInViewCone(
            player,
            visionRange
        );
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
