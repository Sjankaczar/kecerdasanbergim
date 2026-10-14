using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyFSM : MonoBehaviour
{
    public enum IdleBehavior
    {
        Patrol,
        LookAround
    }

    public enum EnemyState
    {
        Patrol,
        Alert,
        Chase,
        Attack,
        Search
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyPerception perception;

    [Header("Patrol")]
    [SerializeField] private IdleBehavior idleBehavior = IdleBehavior.Patrol;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waypointTolerance = 0.5f;

    [Header("Patrol Footsteps")]
    [SerializeField] private AudioClip footstepsClip;
    [SerializeField, Range(0f, 1f)] private float footstepsVolume = 0.7f;

    [Header("Alert State Audio")]
    [SerializeField] private AudioClip alarmClip;
    [SerializeField] private AudioClip intenseTrackClip;
    [SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField, Range(0f, 1f)] private float alarmVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float intenseTrackVolume = 0.65f;
    [SerializeField, Range(0f, 1f)] private float backgroundMusicVolume = 0.5f;

    [Header("Look Around")]
    [SerializeField, Range(0f, 90f)] private float lookAroundAngle = 45f;
    [SerializeField, Min(0f)] private float lookAroundPause = 1.5f;
    [SerializeField, Min(1f)] private float lookAroundTurnSpeed = 90f;

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
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Debug")]
    [SerializeField] private EnemyState currentState;

    private int currentPatrolIndex = 0;

    private float lostPlayerTimer = 0f;
    private float nextAttackTime = 0f;

    private Vector3 lastKnownPosition;
    private bool hasLastKnownPosition;
    private float searchTimer;
    private bool wasPlayerInAlertCone;
    private Quaternion lookAroundCenterRotation;
    private bool lookingLeft = true;
    private float lookAroundPauseTimer;
    private bool reachedLookAroundTarget;
    private AudioSource footstepsSource;
    private Renderer[] childRenderers;

    private static readonly HashSet<EnemyFSM> AlertingEnemies = new HashSet<EnemyFSM>();
    private static AudioSource sharedAlarmSource;
    private static AudioSource sharedMusicSource;
    private static AudioSource sharedBackgroundMusicSource;
    private static AudioClip sharedAlarmClip;
    private static AudioClip sharedMusicClip;
    private static AudioClip sharedBackgroundMusicClip;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSharedAudioState()
    {
        AlertingEnemies.Clear();
        sharedAlarmSource = null;
        sharedMusicSource = null;
        sharedBackgroundMusicSource = null;
        sharedAlarmClip = null;
        sharedMusicClip = null;
        sharedBackgroundMusicClip = null;
    }

    public EnemyState CurrentState => currentState;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (perception == null)
            perception = GetComponent<EnemyPerception>();

        childRenderers = GetComponentsInChildren<Renderer>();
        EnsureSharedAlertAudio();
        if (footstepsClip != null)
        {
            footstepsSource = GetComponent<AudioSource>();
            if (footstepsSource == null)
                footstepsSource = gameObject.AddComponent<AudioSource>();

            footstepsSource.clip = footstepsClip;
            footstepsSource.playOnAwake = false;
            footstepsSource.loop = true;
            footstepsSource.volume = footstepsVolume;
            footstepsSource.spatialBlend = 1f;
        }
    }

    private void Start()
    {
        lookAroundCenterRotation = transform.rotation;
        currentState = EnemyState.Patrol;
        EnterState(currentState);
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
        // UPDATE CURRENT STATE
        // ================================

        switch (currentState)
        {
            case EnemyState.Patrol:
                if (idleBehavior == IdleBehavior.LookAround)
                    UpdateLookAround(playerEnteredAlertCone);
                else
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
        }

        UpdatePatrolFootsteps();
    }

    private void UpdatePatrolFootsteps()
    {
        if (footstepsSource == null)
            return;

        Camera viewCamera = Camera.main;
        bool isWalkingPatrol = currentState == EnemyState.Patrol &&
            idleBehavior == IdleBehavior.Patrol &&
            agent != null && !agent.isStopped && agent.velocity.sqrMagnitude > 0.04f;

        bool isVisible = false;
        if (viewCamera != null && childRenderers != null)
        {
            foreach (Renderer childRenderer in childRenderers)
            {
                if (childRenderer == null || !childRenderer.enabled)
                    continue;

                Vector3 viewportPoint = viewCamera.WorldToViewportPoint(childRenderer.bounds.center);
                if (viewportPoint.z > 0f && viewportPoint.x >= 0f && viewportPoint.x <= 1f &&
                    viewportPoint.y >= 0f && viewportPoint.y <= 1f)
                {
                    isVisible = true;
                    break;
                }
            }
        }

        bool shouldPlay = isWalkingPatrol && isVisible;
        if (shouldPlay && !footstepsSource.isPlaying)
            footstepsSource.Play();
        else if (!shouldPlay && footstepsSource.isPlaying)
            footstepsSource.Stop();
    }

    private void OnDisable()
    {
        if (AlertingEnemies.Contains(this))
            UnregisterAlertingEnemy();

        if (footstepsSource != null && footstepsSource.isPlaying)
            footstepsSource.Stop();
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

        bool wasAlertState = IsAlertAudioState(currentState);
        bool entersAlertState = IsAlertAudioState(newState);
        if (wasAlertState && !entersAlertState)
            UnregisterAlertingEnemy();

        ExitState(currentState);

        currentState = newState;

        Debug.Log(
            gameObject.name +
            " → State: " +
            currentState
        );

        EnterState(currentState);

        if (entersAlertState && !wasAlertState)
            RegisterAlertingEnemy();
    }

    private static bool IsAlertAudioState(EnemyState state)
    {
        return state == EnemyState.Chase || state == EnemyState.Search;
    }

    private void RegisterAlertingEnemy()
    {
        bool wasAnyEnemyAlerting = AlertingEnemies.Count > 0;
        AlertingEnemies.Add(this);

        if (!wasAnyEnemyAlerting)
        {
            if (sharedAlarmSource != null && sharedAlarmClip != null)
            {
                sharedAlarmSource.clip = sharedAlarmClip;
                sharedAlarmSource.loop = false;
                sharedAlarmSource.Play();
            }

            if (sharedMusicSource != null && sharedMusicClip != null)
            {
                sharedMusicSource.clip = sharedMusicClip;
                sharedMusicSource.Play();
            }

            if (sharedBackgroundMusicSource != null)
                sharedBackgroundMusicSource.Stop();
        }
    }

    private void UnregisterAlertingEnemy()
    {
        AlertingEnemies.Remove(this);
        if (AlertingEnemies.Count == 0)
        {
            if (sharedAlarmSource != null)
                sharedAlarmSource.Stop();
            if (sharedMusicSource != null)
                sharedMusicSource.Stop();
            if (sharedBackgroundMusicSource != null && sharedBackgroundMusicClip != null)
                sharedBackgroundMusicSource.Play();
        }
    }

    private void EnsureSharedAlertAudio()
    {
        if (sharedAlarmClip == null && alarmClip != null)
            sharedAlarmClip = alarmClip;
        if (sharedMusicClip == null && intenseTrackClip != null)
            sharedMusicClip = intenseTrackClip;
        if (sharedBackgroundMusicClip == null && backgroundMusicClip != null)
            sharedBackgroundMusicClip = backgroundMusicClip;

        GameObject audioObject = sharedAlarmSource != null
            ? sharedAlarmSource.gameObject
            : new GameObject("Enemy State Audio (Runtime)");

        if (sharedAlarmSource == null)
        {
            sharedAlarmSource = audioObject.AddComponent<AudioSource>();
            sharedAlarmSource.playOnAwake = false;
            sharedAlarmSource.spatialBlend = 0f;
            sharedAlarmSource.volume = alarmVolume;
        }

        if (sharedMusicSource == null)
        {
            sharedMusicSource = audioObject.AddComponent<AudioSource>();
            sharedMusicSource.playOnAwake = false;
            sharedMusicSource.loop = true;
            sharedMusicSource.spatialBlend = 0f;
            sharedMusicSource.volume = intenseTrackVolume;
            sharedMusicSource.clip = sharedMusicClip;
        }

        if (sharedBackgroundMusicSource == null)
        {
            sharedBackgroundMusicSource = audioObject.AddComponent<AudioSource>();
            sharedBackgroundMusicSource.playOnAwake = false;
            sharedBackgroundMusicSource.loop = true;
            sharedBackgroundMusicSource.spatialBlend = 0f;
            sharedBackgroundMusicSource.volume = backgroundMusicVolume;
            sharedBackgroundMusicSource.clip = sharedBackgroundMusicClip;
        }

        if (AlertingEnemies.Count == 0 && sharedBackgroundMusicClip != null && !sharedBackgroundMusicSource.isPlaying)
            sharedBackgroundMusicSource.Play();
    }

    // =====================================
    // ENTER STATE
    // =====================================

    private void EnterState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Patrol:

                if (idleBehavior == IdleBehavior.LookAround)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                    lookAroundCenterRotation = transform.rotation;
                    lookingLeft = true;
                    reachedLookAroundTarget = false;
                    lookAroundPauseTimer = 0f;
                }
                else
                {
                    agent.isStopped = false;
                    agent.speed = patrolSpeed;
                    SetPatrolDestination();
                }

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

    private void UpdateLookAround(bool playerEnteredAlertCone)
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

        float yawOffset = lookingLeft ? -lookAroundAngle : lookAroundAngle;
        Quaternion targetRotation = lookAroundCenterRotation * Quaternion.Euler(0f, yawOffset, 0f);

        if (!reachedLookAroundTarget)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                lookAroundTurnSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, targetRotation) <= 0.5f)
            {
                transform.rotation = targetRotation;
                reachedLookAroundTarget = true;
                lookAroundPauseTimer = lookAroundPause;
            }

            return;
        }

        lookAroundPauseTimer -= Time.deltaTime;
        if (lookAroundPauseTimer <= 0f)
        {
            lookingLeft = !lookingLeft;
            reachedLookAroundTarget = false;
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
