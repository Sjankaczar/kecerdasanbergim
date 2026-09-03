using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    // 1. Tambahkan state Suspicious di enum
    public enum EnemyState
    {
        Idle,
        Suspicious,
        Alert
    }

    [Header("Target")]
    [SerializeField]
    private Transform player;

    [Header("AI Parameters")]
    [SerializeField]
    [Min(0f)]
    private float alertRadius = 4f; // Radius Alert (< 4 atau <= 4)

    [SerializeField]
    [Min(0f)]
    private float suspiciousRadius = 8f; // Radius Suspicious (<= 8)

    [Header("Visual Colors")]
    [SerializeField]
    private Color idleColor = Color.blue;

    [SerializeField]
    private Color suspiciousColor = Color.yellow;

    [SerializeField]
    private Color alertColor = Color.red;

    [Header("Debug")]
    [SerializeField]
    private EnemyState currentState;

    [SerializeField]
    private float currentDistance;

    private Renderer enemyRenderer;

    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
        currentState = EnemyState.Alert;
        SetState(EnemyState.Idle);
    }

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        if (player == null)
            return;

        // Hitung jarak dari Enemy ke Player
        currentDistance = Vector3.Distance(transform.position, player.position);

        // 2. Logika Decision bertingkat
        if (currentDistance <= alertRadius)
        {
            SetState(EnemyState.Alert);
        }
        else if (currentDistance <= suspiciousRadius)
        {
            SetState(EnemyState.Suspicious);
        }
        else
        {
            SetState(EnemyState.Idle);
        }
    }

    void SetState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        Debug.Log("Enemy State → " + currentState);

        if (enemyRenderer == null)
            return;

        // 3. Ubah warna material sesuai state
        switch (currentState)
        {
            case EnemyState.Alert:
                enemyRenderer.material.color = alertColor;
                break;
            case EnemyState.Suspicious:
                enemyRenderer.material.color = suspiciousColor;
                break;
            case EnemyState.Idle:
                enemyRenderer.material.color = idleColor;
                break;
        }
    }

    // 4. Menggambar 2 lingkaran Gizmos dengan warna berbeda
    void OnDrawGizmos()
    {
        // Lingkaran Dalam (Alert Radius) - Warna Merah
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        // Lingkaran Luar (Suspicious Radius) - Warna Kuning
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, suspiciousRadius);
    }
}
