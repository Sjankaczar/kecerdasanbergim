using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Alert,
        Chase,
        Attack,
        Search,
        Flee,
        Heal,
        Dead
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyPerception perception;
    [SerializeField] private EnemyHealth health;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waypointTolerance = 0.5f;

    [Header("Alert")]
    [SerializeField] private float alertRange = 14f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float lostPlayerDelay = 2f;

    [Header("Search")]
    [SerializeField] private float searchDuration = 4f;
    [SerializeField] private float searchTolerance = 0.8f;
    [SerializeField] private float searchLookSpeed = 120f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackExitRange = 2.75f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Flee")]
    [SerializeField] private Transform safePoint;
    [SerializeField] private float fleeSpeed = 5f;
    [SerializeField] private float lowHealthThreshold = 30f;
    [SerializeField] private float safeDistance = 12f;

    [Header("Heal")]
    [SerializeField] private float healRate = 10f;

    [Header("Debug")]
    [SerializeField] private EnemyState currentState;

    private int currentPatrolIndex = 0;

    private float lostPlayerTimer = 0f;
    private float nextAttackTime = 0f;

    private bool fleeTriggered = false;

    private Vector3 lastKnownPosition;
    private bool hasLastKnownPosition;
    private float searchTimer;
    private bool wasPlayerInAlertCone;

    private PlayerHealth playerHealth;

    public EnemyState CurrentState => currentState;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (perception == null)
            perception = GetComponent<EnemyPerception>();

        if (health == null)
            health = GetComponent<EnemyHealth>();

        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        ChangeState(EnemyState.Patrol);
    }

    private void Update()
    {
        // ================================
        // MEMORY
        // ================================

        UpdateMemory();

        // ================================
        // ALERT PROXIMITY
        // ================================

        bool playerInAlertCone =
            perception.IsInViewCone(player, alertRange);

        bool playerEnteredAlertCone =
            playerInAlertCone &&
            !wasPlayerInAlertCone;

        wasPlayerInAlertCone = playerInAlertCone;

        // ================================
        // GLOBAL TRANSITION PRIORITY
        // ================================

        if (health.IsDead)
        {
            ChangeState(EnemyState.Dead);
            return;
        }

        if (!fleeTriggered &&
            health.CurrentHealth <= lowHealthThreshold &&
            currentState != EnemyState.Flee)
        {
            fleeTriggered = true;
            ChangeState(EnemyState.Flee);
        }

        // ================================
        // UPDATE CURRENT STATE
        // ================================

        switch (currentState)
        {
            case EnemyState.Patrol:
                UpdatePatrol(playerEnteredAlertCone);
                break;

            case EnemyState.Alert:
                UpdateAlert();
                break;

            case EnemyState.Chase:
                UpdateChase();
                break;

            case EnemyState.Attack:
                UpdateAttack();
                break;

            case EnemyState.Search:
                UpdateSearch();
                break;

            case EnemyState.Flee:
                UpdateFlee();
                break;

            case EnemyState.Heal:
                UpdateHeal();
                break;

            case EnemyState.Dead:
                UpdateDead();
                break;
        }
    }

    // =====================================
    // MEMORY
    // =====================================

    private void UpdateMemory()
    {
        if (perception == null ||
            player == null)
            return;

        if (perception.CanSeePlayer)
        {
            lastKnownPosition =
                player.position;

            hasLastKnownPosition = true;
        }
    }

    // =====================================
    // CHANGE STATE
    // =====================================

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        ExitState(currentState);

        currentState = newState;

        Debug.Log(
            gameObject.name +
            " → State: " +
            currentState
        );

        EnterState(currentState);
    }

    // =====================================
    // ENTER STATE
    // =====================================

    private void EnterState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Patrol:

                agent.isStopped = false;
                agent.speed = patrolSpeed;

                SetPatrolDestination();

                break;

            case EnemyState.Alert:

                agent.isStopped = false;
                agent.speed = patrolSpeed;

                agent.SetDestination(
                    player.position
                );

                break;

            case EnemyState.Chase:

                agent.isStopped = false;
                agent.speed = chaseSpeed;

                lostPlayerTimer = 0f;

                break;

            case EnemyState.Attack:

                agent.isStopped = true;
                agent.ResetPath();

                break;

            case EnemyState.Search:

                agent.isStopped = false;
                agent.speed = patrolSpeed;

                searchTimer = searchDuration;

                if (hasLastKnownPosition)
                {
                    agent.SetDestination(
                        lastKnownPosition
                    );
                }

                break;

            case EnemyState.Flee:

                agent.isStopped = false;
                agent.speed = fleeSpeed;

                if (safePoint != null)
                {
                    agent.SetDestination(
                        safePoint.position
                    );
                }

                break;

            case EnemyState.Heal:

                agent.isStopped = true;
                agent.ResetPath();

                break;

            case EnemyState.Dead:

                agent.isStopped = true;
                agent.ResetPath();

                break;
        }
    }

    // =====================================
    // EXIT STATE
    // =====================================

    private void ExitState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Attack:
                agent.isStopped = false;
                break;
        }
    }

    // =====================================
    // PATROL
    // =====================================

    private void UpdatePatrol(
        bool playerEnteredAlertCone)
    {
        if (perception.CanSeePlayer)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (playerEnteredAlertCone)
        {
            ChangeState(EnemyState.Alert);
            return;
        }

        if (patrolPoints == null ||
            patrolPoints.Length == 0)
            return;

        if (!agent.pathPending &&
            agent.remainingDistance <=
            waypointTolerance)
        {
            currentPatrolIndex++;

            if (currentPatrolIndex >=
                patrolPoints.Length)
            {
                currentPatrolIndex = 0;
            }

            SetPatrolDestination();
        }
    }

    private void SetPatrolDestination()
    {
        if (patrolPoints == null ||
            patrolPoints.Length == 0)
            return;

        Transform point =
            patrolPoints[currentPatrolIndex];

        if (point != null)
        {
            agent.SetDestination(
                point.position
            );
        }
    }

    // =====================================
    // ALERT
    // =====================================

    private void UpdateAlert()
    {
        if (perception.CanSeePlayer)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (!perception.IsInViewCone(
            player,
            alertRange))
        {
            ChangeState(EnemyState.Patrol);
            return;
        }

        agent.SetDestination(
            player.position
        );
    }

    // =====================================
    // CHASE
    // =====================================

    private void UpdateChase()
    {
        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (perception.CanSeePlayer)
        {
            lostPlayerTimer = 0f;

            agent.SetDestination(
                player.position
            );

            if (distance <= attackRange)
            {
                ChangeState(
                    EnemyState.Attack
                );

                return;
            }
        }
        else
        {
            lostPlayerTimer +=
                Time.deltaTime;

            if (lostPlayerTimer >=
                lostPlayerDelay)
            {
                ChangeState(
                    EnemyState.Search
                );

                return;
            }
        }
    }

    // =====================================
    // SEARCH
    // =====================================

    private void UpdateSearch()
    {
        if (perception.CanSeePlayer)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (!hasLastKnownPosition)
        {
            ChangeState(EnemyState.Patrol);
            return;
        }

        if (!agent.pathPending &&
            agent.remainingDistance <=
            searchTolerance)
        {
            agent.isStopped = true;

            transform.Rotate(
                0f,
                searchLookSpeed *
                Time.deltaTime,
                0f
            );

            searchTimer -= Time.deltaTime;

            if (searchTimer <= 0f)
            {
                hasLastKnownPosition = false;

                ChangeState(EnemyState.Patrol);
            }
        }
    }

    // =====================================
    // ATTACK
    // =====================================

    private void UpdateAttack()
    {
        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        FacePlayer();

        if (!perception.CanSeePlayer ||
            distance > attackExitRange)
        {
            ChangeState(
                EnemyState.Chase
            );

            return;
        }

        if (Time.time >= nextAttackTime)
        {
            AttackPlayer();

            nextAttackTime =
                Time.time +
                attackCooldown;
        }
    }

    private void AttackPlayer()
    {
        Debug.Log("Enemy attacks Player!");

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(
                attackDamage
            );
        }
    }

    private void FacePlayer()
    {
        Vector3 direction =
            player.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
    }

    // =====================================
    // FLEE
    // =====================================

    private void UpdateFlee()
    {
        if (safePoint == null)
            return;

        float playerDistance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        Vector3 safePointOffset =
            safePoint.position -
            transform.position;

        safePointOffset.y = 0f;

        float safePointDistance =
            safePointOffset.magnitude;

        if (safePointDistance <= 1f)
        {
            ChangeState(EnemyState.Heal);
            return;
        }

        if (playerDistance >= safeDistance)
        {
            ChangeState(
                EnemyState.Patrol
            );

            return;
        }
    }

    // =====================================
    // HEAL
    // =====================================

    private void UpdateHeal()
    {
        health.Heal(
            healRate *
            Time.deltaTime
        );

        if (health.IsFull)
        {
            fleeTriggered = false;

            ChangeState(EnemyState.Patrol);
        }
    }

    // =====================================
    // DEAD
    // =====================================

    private void UpdateDead()
    {
        // Tidak melakukan action.
    }

    // =====================================
    // DEBUG GIZMOS
    // =====================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );

        Gizmos.color = Color.magenta;

        Gizmos.DrawWireSphere(
            transform.position,
            attackExitRange
        );

        Gizmos.color = new Color(1f, 0.5f, 0f);

        Gizmos.DrawWireSphere(
            transform.position,
            alertRange
        );

        if (perception != null)
        {
            float halfAlertAngle =
                perception.VisionAngle * 0.5f;

            Vector3 alertLeftDirection =
                Quaternion.Euler(
                    0f,
                    -halfAlertAngle,
                    0f
                ) * transform.forward;

            Vector3 alertRightDirection =
                Quaternion.Euler(
                    0f,
                    halfAlertAngle,
                    0f
                ) * transform.forward;

            Gizmos.DrawRay(
                transform.position,
                alertLeftDirection * alertRange
            );

            Gizmos.DrawRay(
                transform.position,
                alertRightDirection * alertRange
            );
        }

        if (hasLastKnownPosition)
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawSphere(
                lastKnownPosition,
                0.3f
            );

            Gizmos.DrawLine(
                transform.position,
                lastKnownPosition
            );
        }
    }
}
