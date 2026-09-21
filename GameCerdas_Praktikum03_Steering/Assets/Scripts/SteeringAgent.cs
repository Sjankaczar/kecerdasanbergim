using UnityEngine;

public class SteeringAgent : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private bool useTarget = true;

    [Header("Separation")]
    [SerializeField] private float separationRadius = 1.5f;
    [SerializeField] private float separationWeight = 1.5f;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float maxAcceleration = 8f;
    [SerializeField] private float turnSpeed = 8f;

    [Header("Arrive")]
    [SerializeField] private float slowRadius = 4f;
    [SerializeField] private float stopRadius = 1.5f;

    [Header("Wander")]
    [SerializeField] private float wanderSpeed = 2.5f;
    [SerializeField] private float wanderChangeInterval = 1.5f;
    [SerializeField] private float wanderAngleChange = 45f;

    [Header("Obstacle Avoidance")]
    [SerializeField] private SteeringSensor sensor;
    [SerializeField] private float avoidanceWeight = 2.5f;

    [Header("Flee (Level 2)")]
    [SerializeField] private float panicRadius = 5f;
    [SerializeField] private float fleeSpeed = 6f;

    [Header("Pursue (Level 4)")]
    [SerializeField] private float predictionTime = 0.5f;

    [Header("Color (Level 1)")]
    [SerializeField] private Color arriveColor = Color.red;
    [SerializeField] private Color wanderColor = Color.blue;
    [SerializeField] private Color avoidingColor = Color.yellow;

    private Vector3 velocity;
    private Vector3 wanderDirection;
    private float wanderTimer;
    private Renderer cachedRenderer;

    private bool useColor;
    private bool useAnimal;
    private bool useSeparation;
    private bool usePursue;

    public Vector3 Velocity => velocity;
    public Vector3 InitialPosition { get; private set; }
    public SteeringSensor Sensor => sensor;

    public float MaxSpeed { get => maxSpeed; set => maxSpeed = Mathf.Max(0f, value); }
    public float MaxAcceleration { get => maxAcceleration; set => maxAcceleration = Mathf.Max(0f, value); }
    public float SlowRadius { get => slowRadius; set => slowRadius = Mathf.Max(0f, value); }
    public float StopRadius { get => stopRadius; set => stopRadius = Mathf.Max(0f, value); }
    public float AvoidanceWeight { get => avoidanceWeight; set => avoidanceWeight = Mathf.Max(0f, value); }

    private void Start()
    {
        InitialPosition = transform.position;
        wanderDirection = transform.forward;
        wanderTimer = wanderChangeInterval;
        cachedRenderer = GetComponent<Renderer>();
    }

    public void SetMode(bool color, bool animal, bool separation, bool pursue)
    {
        useColor = color;
        useAnimal = animal;
        useSeparation = separation;
        usePursue = pursue;

        if (!useColor)
        {
            ResetColor();
        }
    }

    private void Update()
    {
        Vector3 desiredVelocity = CalculateDesiredVelocity();

        desiredVelocity = ApplyObstacleAvoidance(desiredVelocity);

        if (useSeparation)
        {
            Vector3 separation = CalculateSeparation();
            desiredVelocity += separation * separationWeight;
        }

        velocity = Vector3.MoveTowards(
            velocity,
            desiredVelocity,
            maxAcceleration * Time.deltaTime
        );

        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        ApplyMovement();
        UpdateRotation();

        if (useColor)
        {
            UpdateColor(desiredVelocity);
        }
    }

    private Vector3 CalculateDesiredVelocity()
    {
        if (useAnimal)
        {
            return CalculateAnimal();
        }

        if (usePursue && useTarget && target != null)
        {
            return CalculatePursue();
        }

        if (useTarget && target != null)
        {
            return CalculateArrive();
        }

        return CalculateWander();
    }

    private Vector3 CalculateArrive()
    {
        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;

        if (distance <= stopRadius)
        {
            return Vector3.zero;
        }

        float desiredSpeed = maxSpeed;

        if (distance < slowRadius)
        {
            float range = Mathf.Max(slowRadius - stopRadius, 0.001f);
            float normalizedDistance = (distance - stopRadius) / range;
            desiredSpeed = maxSpeed * Mathf.Clamp01(normalizedDistance);
        }

        return toTarget.normalized * desiredSpeed;
    }

    private Vector3 CalculateWander()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            float randomAngle = Random.Range(-wanderAngleChange, wanderAngleChange);

            wanderDirection = Quaternion.Euler(0f, randomAngle, 0f) * transform.forward;
            wanderDirection.y = 0f;
            wanderDirection.Normalize();

            wanderTimer = wanderChangeInterval;
        }

        return wanderDirection * wanderSpeed;
    }

    private Vector3 CalculateFlee()
    {
        Vector3 awayFromTarget = transform.position - target.position;
        awayFromTarget.y = 0f;

        float distance = awayFromTarget.magnitude;

        if (distance > panicRadius)
        {
            return Vector3.zero;
        }

        float fleeFactor = 1f - (distance / Mathf.Max(panicRadius, 0.01f));

        return awayFromTarget.normalized * fleeSpeed * Mathf.Clamp01(fleeFactor);
    }

    private Vector3 CalculateAnimal()
    {
        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;

        if (distance < panicRadius)
        {
            return CalculateFlee();
        }

        return CalculateWander();
    }

    private Vector3 CalculatePursue()
    {
        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;

        Vector3 targetVelocity = Vector3.zero;
        CharacterController cc = target.GetComponent<CharacterController>();

        if (cc != null)
        {
            targetVelocity = cc.velocity;
        }
        else
        {
            SteeringAgent targetAgent = target.GetComponent<SteeringAgent>();

            if (targetAgent != null)
            {
                targetVelocity = targetAgent.Velocity;
            }
        }

        targetVelocity.y = 0f;

        Vector3 predictedPosition = target.position + targetVelocity * predictionTime;
        Vector3 predictedDirection = predictedPosition - transform.position;
        predictedDirection.y = 0f;

        float predictedDistance = predictedDirection.magnitude;

        if (predictedDistance <= stopRadius)
        {
            return Vector3.zero;
        }

        float desiredSpeed = maxSpeed;

        if (predictedDistance < slowRadius)
        {
            float range = Mathf.Max(slowRadius - stopRadius, 0.001f);
            float normalizedDistance = (predictedDistance - stopRadius) / range;
            desiredSpeed = maxSpeed * Mathf.Clamp01(normalizedDistance);
        }

        return predictedDirection.normalized * desiredSpeed;
    }

    private Vector3 ApplyObstacleAvoidance(Vector3 desiredVelocity)
    {
        if (sensor == null)
        {
            return desiredVelocity;
        }

        Vector3 checkDirection =
            desiredVelocity.sqrMagnitude > 0.001f
                ? desiredVelocity.normalized
                : transform.forward;

        Vector3 avoidanceDirection = sensor.GetAvoidanceDirection(checkDirection);

        if (avoidanceDirection.sqrMagnitude > 0.001f)
        {
            Vector3 combinedDirection =
                checkDirection + avoidanceDirection * avoidanceWeight;

            combinedDirection.y = 0f;

            if (combinedDirection.sqrMagnitude > 0.001f)
            {
                combinedDirection.Normalize();
            }

            float desiredSpeed = Mathf.Max(desiredVelocity.magnitude, wanderSpeed);

            return combinedDirection * desiredSpeed;
        }

        return desiredVelocity;
    }

    private void UpdateColor(Vector3 desiredVelocity)
    {
        if (cachedRenderer == null)
        {
            return;
        }

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        cachedRenderer.GetPropertyBlock(block);

        Color color;

        if (sensor != null && sensor.ObstacleDetected)
        {
            color = avoidingColor;
        }
        else if (desiredVelocity.sqrMagnitude > 0.001f)
        {
            color = arriveColor;
        }
        else
        {
            color = wanderColor;
        }

        block.SetColor("_BaseColor", color);
        cachedRenderer.SetPropertyBlock(block);
    }

    private void ResetColor()
    {
        if (cachedRenderer == null)
        {
            return;
        }

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        cachedRenderer.GetPropertyBlock(block);
        block.SetColor("_BaseColor", Color.white);
        cachedRenderer.SetPropertyBlock(block);
    }

    private void ApplyMovement()
    {
        transform.position += velocity * Time.deltaTime;
    }

    private void UpdateRotation()
    {
        Vector3 horizontalVelocity = velocity;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, stopRadius);
        Gizmos.DrawWireSphere(transform.position, slowRadius);

        if (target != null)
        {
            Gizmos.DrawLine(transform.position, target.position);

            if (useAnimal)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(target.position, panicRadius);
            }
        }

        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }

    private Vector3 CalculateSeparation()
    {
        Collider[] neighbors =
            Physics.OverlapSphere(
                transform.position,
                separationRadius
            );

        Vector3 separation = Vector3.zero;
        int count = 0;

        foreach (Collider neighbor in neighbors)
        {
            if (neighbor.transform == transform)
            {
                continue;
            }

            if (neighbor.GetComponent<SteeringAgent>() == null)
            {
                continue;
            }

            Vector3 away =
                transform.position - neighbor.transform.position;

            away.y = 0f;

            float sqrDistance = away.sqrMagnitude;

            if (sqrDistance > 0.001f)
            {
                separation += away.normalized / Mathf.Max(sqrDistance, 0.01f);
                count++;
            }
        }

        if (count > 0)
        {
            separation /= count;
        }

        return separation;
    }
}
