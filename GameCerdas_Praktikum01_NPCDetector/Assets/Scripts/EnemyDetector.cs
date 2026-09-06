using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public enum EnemyState { Idle, Suspicious, Alert }

    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("AI Parameters")]
    [SerializeField] [Min(0f)] private float alertRadius = 4f;
    [SerializeField] [Min(0f)] private float suspiciousRadius = 8f;

    [Header("Visual Colors")]
    [SerializeField] private Color idleColor = Color.blue;
    [SerializeField] private Color suspiciousColor = Color.yellow;
    [SerializeField] private Color alertColor = Color.red;

    [Header("Debug")]
    [SerializeField] private EnemyState currentState;
    
    // Variabel ini akan dibaca oleh UI
    public float currentDistance { get; private set; }
    public bool isAdvancedMode { get; private set; } = false; // False = Mode 1, True = Mode 2

    private Renderer enemyRenderer;

    void Start()
    {
        enemyRenderer = GetComponent<Renderer>();
        SetState(EnemyState.Idle);
    }

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        if (player == null) return;

        currentDistance = Vector3.Distance(transform.position, player.position);

        if (isAdvancedMode)
        {
            // Mode 2: Alert, Suspicious, Idle
            if (currentDistance <= alertRadius) SetState(EnemyState.Alert);
            else if (currentDistance <= suspiciousRadius) SetState(EnemyState.Suspicious);
            else SetState(EnemyState.Idle);
        }
        else
        {
            // Mode 1: Alert, Idle
            if (currentDistance <= alertRadius) SetState(EnemyState.Alert);
            else SetState(EnemyState.Idle);
        }
    }

    void SetState(EnemyState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        if (enemyRenderer == null) return;

        switch (currentState)
        {
            case EnemyState.Alert: enemyRenderer.material.color = alertColor; break;
            case EnemyState.Suspicious: enemyRenderer.material.color = suspiciousColor; break;
            case EnemyState.Idle: enemyRenderer.material.color = idleColor; break;
        }
    }

    // Fungsi ini akan dipanggil oleh tombol UI
    public void ToggleMode()
    {
        isAdvancedMode = !isAdvancedMode;
        
        // Paksa perbarui state saat mode diganti agar warnanya langsung menyesuaikan
        currentState = EnemyState.Idle; // Reset sementara
        DetectPlayer(); 
    }

    void OnDrawGizmos()
    {
        if (isAdvancedMode)
        {
            // 3 States: Radius dalam (merah) dan radius luar (kuning)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, alertRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, suspiciousRadius);
        }
        else
        {
            // 2 States: Hanya satu radius, diubah warnanya menjadi kuning
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, alertRadius);
        }
    }
}
