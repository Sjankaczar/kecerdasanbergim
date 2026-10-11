
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBTController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Transform safePoint;

    [Header("Perception")]
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float visionAngle = 90f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Combat")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 10;

    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int lowHealthThreshold = 30;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float fleeSpeed = 5f;

    [Header("Debug")]
    [SerializeField] private string currentAction = "None";

    private NavMeshAgent agent;
    private BTNode rootNode;
    private int currentHealth;
    private int currentPatrolIndex = 0;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        BuildBehaviorTree();
    }

    private void Update()
    {
        if (rootNode != null)
            rootNode.Tick();
    }

    private void BuildBehaviorTree()
    {
        BTNode fleeSequence = new SequenceNode(new List<BTNode>
        {
            new ConditionNode(IsHealthLow),
            new ActionNode(Flee)
        });

        BTNode attackSequence = new SequenceNode(new List<BTNode>
        {
            new ConditionNode(CanSeePlayer),
            new ConditionNode(IsPlayerInAttackRange),
            new CooldownDecorator(
                new ActionNode(AttackPlayer),
                attackCooldown
            )
        });

        BTNode chaseSequence = new SequenceNode(new List<BTNode>
        {
            new ConditionNode(CanSeePlayer),
            new ActionNode(ChasePlayer)
        });

        rootNode = new SelectorNode(new List<BTNode>
        {
            fleeSequence,
            attackSequence,
            chaseSequence,
            new ActionNode(Patrol)
        });
    }

    private bool IsHealthLow()
    {
        return currentHealth <= lowHealthThreshold;
    }

    private bool IsPlayerInAttackRange()
    {
        if (player == null) return false;

        return Vector3.Distance(transform.position, player.position)
            <= attackRange;
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 eyePosition = transform.position + Vector3.up * 1.5f;
        Vector3 targetPosition = player.position + Vector3.up;
        Vector3 direction = targetPosition - eyePosition;
        float distance = direction.magnitude;

        if (distance > visionRange)
            return false;

        float angle = Vector3.Angle(transform.forward, direction);

        if (angle > visionAngle * 0.5f)
            return false;

        bool blocked = Physics.Raycast(
            eyePosition,
            direction.normalized,
            distance,
            obstacleMask
        );

        return !blocked;
    }

    private NodeState Patrol()
    {
        currentAction = "PATROL";

        if (patrolPoints == null || patrolPoints.Length == 0)
            return NodeState.Failure;

        agent.isStopped = false;
        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0.2f;

        Transform target = patrolPoints[currentPatrolIndex];
        agent.SetDestination(target.position);

        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            currentPatrolIndex++;

            if (currentPatrolIndex >= patrolPoints.Length)
                currentPatrolIndex = 0;
        }

        return NodeState.Running;
    }

    private NodeState ChasePlayer()
    {
        if (player == null) return NodeState.Failure;

        currentAction = "CHASE";
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.stoppingDistance = attackRange * 0.8f;
        agent.SetDestination(player.position);

        return NodeState.Running;
    }

    private NodeState AttackPlayer()
    {
        if (player == null) return NodeState.Failure;

        currentAction = "ATTACK";
        agent.isStopped = true;
        FacePlayer();

        Debug.Log(name + " attacks Player! Damage = " + attackDamage);

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.TakeDamage(attackDamage);

        return NodeState.Success;
    }

    private NodeState Flee()
    {
        if (safePoint == null) return NodeState.Failure;

        currentAction = "FLEE";
        agent.isStopped = false;
        agent.speed = fleeSpeed;
        agent.stoppingDistance = 0.5f;
        agent.SetDestination(safePoint.position);

        if (!agent.pathPending && agent.remainingDistance <= 0.7f)
        {
            agent.isStopped = true;
            return NodeState.Success;
        }

        return NodeState.Running;
    }

    private void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            10f * Time.deltaTime
        );
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(
            currentHealth - damage, 0, maxHealth
        );

        Debug.Log(name + " Health = " + currentHealth);
    }

    [ContextMenu("Test Damage 25")]
    private void TestDamage25()
    {
        TakeDamage(25);
    }

    [ContextMenu("Reset Health")]
    private void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, visionRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 leftDirection =
            Quaternion.Euler(0, -visionAngle * 0.5f, 0)
            * transform.forward;

        Vector3 rightDirection =
            Quaternion.Euler(0, visionAngle * 0.5f, 0)
            * transform.forward;

        Gizmos.DrawRay(transform.position, leftDirection * visionRange);
        Gizmos.DrawRay(transform.position, rightDirection * visionRange);
    }
}
