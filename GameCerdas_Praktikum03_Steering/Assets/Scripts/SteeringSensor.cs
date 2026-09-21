using UnityEngine;

public class SteeringSensor : MonoBehaviour
{
    [Header("Obstacle Sensor")]

    [SerializeField]
    private float sensorDistance = 3f;

    [SerializeField]
    private float sensorRadius = 0.5f;

    [SerializeField]
    private float sensorHeight = 0.5f;

    [SerializeField]
    private LayerMask obstacleMask;

    [Header("Avoidance")]

    [SerializeField]
    private float forwardBias = 0.5f;

    [SerializeField, Min(0.5f)]
    private float slideHysteresis = 0.6f;

    [Header("Debug")]
    [SerializeField]
    private bool logSensor;

    [SerializeField, Min(0.1f)]
    private float logInterval = 0.5f;

    private float nextLogTime;
    private bool obstacleDetected;
    private RaycastHit lastHit;
    private int slideSide;
    private float slideHysteresisTimer;

    public bool ObstacleDetected => obstacleDetected;

    public RaycastHit LastHit => lastHit;

    public float SensorDistance { get => sensorDistance; set => sensorDistance = Mathf.Max(0f, value); }
    public float SensorRadius { get => sensorRadius; set => sensorRadius = Mathf.Max(0f, value); }
    public float ForwardBias { get => forwardBias; set => forwardBias = Mathf.Max(0f, value); }

    public Vector3 GetAvoidanceDirection(
        Vector3 movementDirection)
    {
        obstacleDetected = false;

        if (movementDirection.sqrMagnitude < 0.001f)
        {
            return Vector3.zero;
        }

        movementDirection.Normalize();

        Vector3 origin =
            transform.position +
            Vector3.up * sensorHeight;

        if (Physics.SphereCast(
            origin,
            sensorRadius,
            movementDirection,
            out lastHit,
            sensorDistance,
            obstacleMask,
            QueryTriggerInteraction.Ignore))
        {
            obstacleDetected = true;
            slideHysteresisTimer = slideHysteresis;
            LogSensor(movementDirection);

            if (slideSide == 0)
            {
                slideSide = ChooseSlideSide(movementDirection);
            }

            Vector3 surfaceNormal =
                Vector3.ProjectOnPlane(
                    lastHit.normal,
                    Vector3.up
                );

            surfaceNormal.y = 0f;

            if (surfaceNormal.sqrMagnitude > 0.001f)
            {
                surfaceNormal.Normalize();
            }

            Vector3 slideDirection =
                Vector3.Cross(Vector3.up, surfaceNormal);

            if (slideSide < 0)
            {
                slideDirection = -slideDirection;
            }

            Vector3 avoidDirection =
                surfaceNormal * Mathf.Max(0.25f, forwardBias) +
                slideDirection;

            return avoidDirection.normalized;
        }

        UpdateSlideHysteresis();
        LogSensor(movementDirection);
        return Vector3.zero;
    }

    private int ChooseSlideSide(Vector3 movementDirection)
    {
        Vector3 toTarget =
            movementDirection;

        Vector3 surfaceNormal =
            Vector3.ProjectOnPlane(
                lastHit.normal,
                Vector3.up
            );

        surfaceNormal.y = 0f;

        if (surfaceNormal.sqrMagnitude < 0.001f)
        {
            return 1;
        }

        surfaceNormal.Normalize();

        float rightDot =
            Vector3.Dot(
                Vector3.Cross(Vector3.up, surfaceNormal),
                toTarget
            );

        if (Mathf.Abs(rightDot) < 0.05f)
        {
            return 1;
        }

        return rightDot > 0f ? 1 : -1;
    }

    private void UpdateSlideHysteresis()
    {
        if (slideHysteresisTimer > 0f)
        {
            slideHysteresisTimer -= Time.deltaTime;

            if (slideHysteresisTimer <= 0f)
            {
                slideSide = 0;
            }
        }
    }

    private void LogSensor(Vector3 direction)
    {
        if (!logSensor || Time.unscaledTime < nextLogTime)
        {
            return;
        }

        nextLogTime = Time.unscaledTime + Mathf.Max(0.1f, logInterval);
        Collider hitCollider = obstacleDetected ? lastHit.collider : null;
        string hitInfo = hitCollider != null
            ? $"{hitCollider.name}#{hitCollider.GetEntityId()} layer={LayerMask.LayerToName(hitCollider.gameObject.layer)}({hitCollider.gameObject.layer}) distance={lastHit.distance:F3} normal={lastHit.normal.ToString("F3")}"
            : "none";

        Debug.Log(
            $"[SteeringSensor] {name}#{GetEntityId()} frame={Time.frameCount} mask={obstacleMask.value} direction={direction.ToString("F3")} hit={hitInfo}",
            this
        );
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin =
            transform.position +
            Vector3.up * sensorHeight;

        Vector3 direction =
            transform.forward;

        Gizmos.DrawWireSphere(
            origin,
            sensorRadius
        );

        Gizmos.DrawLine(
            origin,
            origin + direction * sensorDistance
        );

        Gizmos.DrawWireSphere(
            origin + direction * sensorDistance,
            sensorRadius
        );
    }
}