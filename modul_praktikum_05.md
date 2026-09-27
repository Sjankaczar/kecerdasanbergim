# MODUL PRAKTIKUM 05  
# FINITE STATE MACHINE (FSM)

## Enemy AI: Patrol → Chase → Attack → Flee

**Mata Kuliah:** Game Cerdas  
**Program Studi:** S1 Teknik Informatika  
**Tools:** Unity 6 + C#  
**Materi:** Finite State Machine  
**Prasyarat:** Perception, Movement/Steering, Pathfinding & Navigation

---

# 1. Tujuan Praktikum

Pada praktikum ini mahasiswa akan membuat sebuah NPC enemy yang mampu mengambil keputusan menggunakan **Finite State Machine (FSM)**.

Enemy memiliki empat perilaku utama:

```text
Patrol
   ↓
Chase
   ↓
Attack
   ↓
Flee
```

serta satu state tambahan:

```text
Dead
```

Perilaku yang akan dibuat:

1. Enemy melakukan patroli di antara beberapa waypoint.
2. Enemy mendeteksi player menggunakan jarak, field of view, dan line of sight.
3. Jika player terlihat, enemy berpindah dari **Patrol** ke **Chase**.
4. Enemy mengejar player menggunakan `NavMeshAgent`.
5. Jika player berada dalam jarak serang, enemy berpindah ke **Attack**.
6. Enemy menyerang menggunakan damage dan attack cooldown.
7. Jika health enemy turun melewati batas tertentu, enemy masuk ke **Flee**.
8. Enemy menuju safe point ketika kabur.
9. Jika enemy sudah cukup aman, enemy kembali melakukan patrol.
10. Jika health mencapai 0, enemy masuk ke **Dead**.

---

# 2. Capaian Pembelajaran Praktikum

Setelah menyelesaikan praktikum, mahasiswa diharapkan mampu:

- memahami konsep **State**;
- memahami **Transition**;
- memahami **Condition**;
- membuat state menggunakan `enum`;
- membuat fungsi `ChangeState()`;
- memahami konsep `EnterState()`, `UpdateState()`, dan `ExitState()`;
- menerapkan priority transition;
- menggunakan `NavMeshAgent`;
- menggunakan waypoint;
- menggunakan `Physics.Raycast`;
- menggunakan `LayerMask`;
- menggunakan `SerializeField`;
- menerapkan Field of View sederhana;
- membuat sistem health;
- membuat attack cooldown;
- memahami hysteresis sederhana;
- melakukan debugging FSM menggunakan Console dan Gizmos;
- menghubungkan decision making dengan perception dan navigation.

---

# 3. Gambaran Sistem

Arsitektur AI yang dibuat:

```text
              PLAYER
                 │
                 ▼
         EnemyPerception
       distance + FOV + LOS
                 │
                 ▼
             EnemyFSM
                 │
        ┌────────┼─────────┐
        ▼        ▼         ▼
     Patrol    Chase     Attack
        │        │         │
        └────────┴────┬────┘
                     │
                HP Rendah
                     ▼
                    Flee

              HP <= 0
                  │
                  ▼
                 Dead
```

Hubungannya dengan materi sebelumnya:

```text
Perception
    ↓
Enemy dapat melihat player?
    ↓
FSM / Decision Making
    ↓
State = Chase
    ↓
NavMeshAgent
    ↓
Pathfinding / Navigation
    ↓
Enemy bergerak mengejar player
```

Dengan demikian FSM **tidak menggantikan NavMesh**.

FSM menentukan:

> Apa yang harus dilakukan NPC?

Sedangkan NavMesh menentukan:

> Bagaimana NPC mencapai tujuan tersebut?

---

# 4. Rekomendasi Implementasi Praktikum

Untuk praktikum ini digunakan:

```text
Enum-based FSM
+
NavMeshAgent
+
Raycast Perception
+
Health System
+
Waypoint Patrol
+
Safe Point Flee
```

## Mengapa menggunakan Enum FSM?

Contoh:

```csharp
public enum EnemyState
{
    Patrol,
    Chase,
    Attack,
    Flee,
    Dead
}
```

Pendekatan ini dipilih karena:

- struktur state terlihat jelas;
- mahasiswa mudah memahami hubungan state dan transition;
- kode belum terlalu abstrak;
- mudah diperiksa;
- mudah di-debug;
- jumlah state masih sedikit;
- sangat cocok sebagai pengantar FSM.

Untuk proyek yang lebih besar dapat dikembangkan menjadi:

```text
Class-based FSM
Hierarchical FSM
Behavior Tree
Utility AI
```

tetapi belum diperlukan sebagai implementasi utama praktikum ini.

---

# 5. Rekomendasi Nama Project

Nama project yang disarankan:

```text
GameCerdas_Praktikum05_EnemyFSM
```

Alternatif:

```text
Praktikum05_FSMEnemyAI
```

atau:

```text
GC05_EnemyFSM
```

**Rekomendasi utama:**

```text
GameCerdas_Praktikum05_EnemyFSM
```

Nama tersebut jelas menunjukkan:

- mata kuliah;
- nomor praktikum;
- konsep utama;
- objek implementasi.

---

# 6. Struktur Folder Project

Di dalam folder `Assets`, buat struktur:

```text
Assets/
│
├── Materials/
│
├── Scenes/
│   └── Praktikum05_FSM.unity
│
├── Scripts/
│   ├── EnemyFSM.cs
│   ├── EnemyPerception.cs
│   ├── EnemyHealth.cs
│   ├── PlayerHealth.cs
│   ├── SimplePlayerController.cs
│   └── PlayerAttackTest.cs
│
└── Prefabs/
```

Keuntungan struktur folder:

- script tidak bercampur dengan material;
- scene mudah ditemukan;
- project lebih rapi;
- mahasiswa terbiasa dengan organisasi project.

---

# 7. Persiapan Project Unity

## Langkah 1 — Membuat Project

Buka:

```text
Unity Hub
→ New Project
```

Pilih template 3D.

Beri nama:

```text
GameCerdas_Praktikum05_EnemyFSM
```

Kemudian buat project.

---

# 8. Memasang AI Navigation

Praktikum menggunakan NavMesh.

Buka:

```text
Window
→ Package Manager
```

Cari:

```text
AI Navigation
```

kemudian install jika belum tersedia.

AI Navigation menyediakan komponen untuk membuat NavMesh yang digunakan NPC dalam melakukan navigation.

---

# 9. Apa Itu NavMesh?

**Navigation Mesh**, atau **NavMesh**, adalah representasi area dalam game yang dapat dilalui NPC.

Contoh:

```text
+----------------------------------+
|                                  |
|      area yang dapat dilalui     |
|                                  |
|       █████ obstacle █████       |
|                                  |
|                                  |
+----------------------------------+
```

NPC tidak perlu menentukan gerakan satu per satu.

Sebaliknya:

```csharp
agent.SetDestination(target.position);
```

NPC akan meminta sistem navigation mencari jalur menuju target.

---

# 10. Apa Itu NavMeshAgent?

`NavMeshAgent` adalah komponen Unity yang membuat sebuah GameObject dapat bergerak di atas NavMesh.

Pada praktikum:

```text
Enemy
└── NavMeshAgent
```

FSM akan memberikan perintah kepada `NavMeshAgent`.

Contoh:

### Patrol

```csharp
agent.SetDestination(waypoint.position);
```

### Chase

```csharp
agent.SetDestination(player.position);
```

### Attack

```csharp
agent.isStopped = true;
```

### Flee

```csharp
agent.SetDestination(safePoint.position);
```

`SetDestination()` digunakan untuk mengatur atau memperbarui tujuan agent.

---

# 11. Membuat Scene

Buat scene:

```text
Assets
→ Scenes
→ Praktikum05_FSM
```

Struktur Hierarchy akhirnya kurang lebih:

```text
Praktikum05_FSM
│
├── Main Camera
├── Directional Light
│
├── Environment
│   ├── Ground
│   ├── Obstacle01
│   ├── Obstacle02
│   └── Obstacle03
│
├── Navigation
│
├── Player
│
├── Enemy
│
├── PatrolPoints
│   ├── W1
│   ├── W2
│   ├── W3
│   └── W4
│
└── SafePoint
```

---

# 12. Membuat Ground

Buat:

```text
GameObject
→ 3D Object
→ Plane
```

Rename:

```text
Ground
```

Contoh Transform:

```text
Position : (0, 0, 0)
Scale    : (3, 1, 3)
```

Plane akan menjadi area utama pergerakan.

---

# 13. Membuat Obstacle

Buat beberapa Cube.

Contoh:

```text
Obstacle01
Obstacle02
Obstacle03
```

Atur posisi secara bebas sehingga membentuk penghalang.

Contoh layout:

```text
W1 ● ------------------ ● W2


         ███████
         ███████

Player ●           Enemy ●


         ███████

W4 ● ------------------ ● W3
```

Obstacle akan membuat NavMeshAgent harus mencari jalur mengelilingi objek.

---

# 14. Membuat NavMeshSurface

Buat Empty GameObject:

```text
GameObject
→ Create Empty
```

Rename:

```text
Navigation
```

Kemudian:

```text
Add Component
→ NavMesh Surface
```

`NavMeshSurface` digunakan untuk menentukan area Scene yang akan digunakan untuk membangun NavMesh.

Pada Inspector:

```text
Navigation
└── NavMesh Surface
```

Klik:

```text
Bake
```

atau tombol/build workflow yang tersedia pada versi AI Navigation yang digunakan.

Setelah NavMesh berhasil dibuat, Scene View akan menampilkan area navigation.

---

# 15. Membuat Player

Buat:

```text
GameObject
→ 3D Object
→ Capsule
```

Rename:

```text
Player
```

Atur:

```text
Position = (0, 1, 0)
```

Buat material, misalnya:

```text
PlayerMaterial
```

gunakan warna yang mudah dibedakan.

---

# 16. Tag Player

Pilih Player.

Pada Inspector:

```text
Tag
→ Player
```

## Apa Itu Tag?

**Tag** adalah label untuk mengidentifikasi jenis GameObject.

Contoh:

```text
Player
Enemy
MainCamera
```

Tag biasanya digunakan untuk mengetahui **identitas atau kategori objek**.

Contoh:

```csharp
if (other.CompareTag("Player"))
{
    ...
}
```

---

# 17. Tag dan Layer Tidak Sama

Perbedaan penting:

| Tag | Layer |
|---|---|
| Mengidentifikasi objek | Mengelompokkan objek untuk sistem tertentu |
| Biasanya satu kategori | Digunakan oleh Physics/Camera/Raycast |
| Contoh: Player | Contoh: Obstacle |
| Dipakai `CompareTag()` | Dipakai `LayerMask` |

Secara sederhana:

```text
Tag
→ "Objek ini siapa?"

Layer
→ "Objek ini termasuk kelompok physics/rendering apa?"
```

---

# 18. Membuat Layer Obstacle

Buat Layer:

```text
Obstacle
```

Assign ke:

```text
Obstacle01
Obstacle02
Obstacle03
```

Layer ini akan digunakan oleh perception system.

---

# 19. Membuat Player Controller

Tambahkan:

```text
Character Controller
```

ke Player.

## Apa Itu CharacterController?

`CharacterController` adalah komponen untuk menggerakkan karakter sambil tetap memperoleh collision constraint tanpa harus mengendalikan karakter sebagai Rigidbody penuh. Unity mendokumentasikannya terutama untuk kontrol karakter first-person atau third-person.

Buat:

```text
Scripts/SimplePlayerController.cs
```

Isi:

```csharp
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimplePlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController controller;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement =
            new Vector3(horizontal, 0f, vertical).normalized;

        controller.Move(
            movement * moveSpeed * Time.deltaTime
        );

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        controller.Move(
            Vector3.up *
            verticalVelocity *
            Time.deltaTime
        );

        if (movement.sqrMagnitude > 0.01f)
        {
            transform.forward = movement;
        }
    }
}
```

Attach ke Player.

Player dapat digerakkan menggunakan:

```text
W A S D
```

atau:

```text
Arrow Keys
```

---

# 20. Penjelasan Script Player

## `RequireComponent`

```csharp
[RequireComponent(typeof(CharacterController))]
```

Memberi tahu Unity bahwa script membutuhkan komponen:

```text
CharacterController
```

Jika belum ada, Unity dapat memastikan komponen tersebut tersedia ketika script dipasang.

---

## `SerializeField`

Contoh:

```csharp
[SerializeField] private float moveSpeed = 5f;
```

Variabel tetap:

```csharp
private
```

tetapi nilainya dapat diatur melalui Inspector.

Ini sangat berguna untuk:

```text
parameter tuning
```

Mahasiswa tidak perlu mengubah source code hanya untuk mengganti speed.

---

## `Time.deltaTime`

Contoh:

```csharp
movement * moveSpeed * Time.deltaTime
```

`Time.deltaTime` adalah waktu antara frame sekarang dan frame sebelumnya.

Digunakan agar kecepatan gerakan tidak secara langsung bergantung pada frame rate.

---

# 21. Membuat Patrol Waypoints

Buat Empty GameObject:

```text
PatrolPoints
```

Di dalamnya buat:

```text
W1
W2
W3
W4
```

Setiap waypoint dibuat menggunakan:

```text
Create Empty
```

Contoh:

```text
PatrolPoints
├── W1
├── W2
├── W3
└── W4
```

Posisikan membentuk rute.

Contoh:

```text
W1 ● ---------------- ● W2
 |                       |
 |                       |
 |                       |
W4 ● ---------------- ● W3
```

---

# 22. Apa Itu Waypoint?

Waypoint adalah sebuah titik referensi posisi.

Pada Unity biasanya berupa:

```text
Empty GameObject
```

Karena setiap GameObject mempunyai:

```text
Transform
```

maka waypoint dapat menyediakan posisi:

```csharp
waypoint.position
```

Enemy dapat bergerak:

```text
W1
 ↓
W2
 ↓
W3
 ↓
W4
 ↓
W1
```

---

# 23. Membuat Enemy

Buat:

```text
GameObject
→ 3D Object
→ Capsule
```

Rename:

```text
Enemy
```

Atur:

```text
Position = (8, 1, 5)
```

Berikan material berbeda dari Player.

---

# 24. Menambahkan NavMeshAgent

Pilih Enemy.

Tambahkan:

```text
Add Component
→ Nav Mesh Agent
```

Parameter awal yang dapat digunakan:

```text
Speed             = 2
Angular Speed     = 360
Acceleration      = 8
Stopping Distance = 0
```

Nilai speed nantinya akan dikontrol FSM.

---

# 25. Membuat Perception System

Buat:

```text
EnemyPerception.cs
```

Isi:

```csharp
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
```

Attach script ke Enemy.

---

# 26. Cara Kerja Perception

Enemy tidak langsung dianggap melihat Player hanya karena dekat.

Tiga condition diperiksa:

```text
1. Distance
2. Field of View
3. Line of Sight
```

Alurnya:

```text
Player dalam visionRange?
        │
       Yes
        ▼
Player berada dalam visionAngle?
        │
       Yes
        ▼
Ada obstacle?
      /   \
    Yes    No
     │      │
 Tidak    Terlihat
 terlihat
```

---

# 27. Apa Itu Physics.Raycast?

Raycast dapat dibayangkan sebagai sebuah garis virtual yang ditembakkan dari suatu posisi ke arah tertentu.

Contoh:

```text
Enemy ● ----------------------> Player
```

Jika ada tembok:

```text
Enemy ● -------- █ -------- Player
                 ↑
             obstacle
```

maka Enemy tidak dapat melihat Player.

Unity menyediakan `Physics.Raycast` untuk melakukan query tersebut, dan raycast dapat difilter menggunakan LayerMask.

---

# 28. Apa Itu LayerMask?

`LayerMask` menentukan layer mana yang diperiksa oleh fungsi physics.

Contoh:

```csharp
[SerializeField]
private LayerMask obstacleMask;
```

Inspector:

```text
Obstacle Mask
☑ Obstacle
```

Artinya Raycast hanya memperhatikan object pada layer yang dipilih.

Ini lebih baik daripada memperlakukan semua collider sebagai obstacle.

---

# 29. Setting EnemyPerception

Pada Inspector Enemy:

```text
Enemy Perception
```

Isi:

```text
Player        = Player
Vision Range  = 10
Vision Angle  = 90
Eye Height    = 1
Obstacle Mask = Obstacle
```

---

# 30. Membuat Enemy Health

Buat:

```text
EnemyHealth.cs
```

Isi:

```csharp
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }

    public float MaxHealth => maxHealth;

    public bool IsDead =>
        CurrentHealth <= 0f;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead)
            return;

        CurrentHealth -= damage;

        CurrentHealth = Mathf.Clamp(
            CurrentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            "Enemy HP: " + CurrentHealth
        );
    }
}
```

Attach ke Enemy.

---

# 31. Membuat Player Health

Buat:

```text
PlayerHealth.cs
```

Isi:

```csharp
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        CurrentHealth = Mathf.Clamp(
            CurrentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            "Player HP: " + CurrentHealth
        );

        if (CurrentHealth <= 0f)
        {
            Debug.Log("Player Dead");
        }
    }
}
```

Attach ke Player.

---

# 32. Membuat Safe Point

Buat:

```text
Create Empty
```

Rename:

```text
SafePoint
```

Letakkan di area yang cukup jauh dari Player.

Contoh:

```text
Player ●


         █ obstacle


Enemy ● ---------------- SafePoint ●
```

SafePoint harus berada di area yang dapat dicapai oleh NavMeshAgent.

---

# 33. State FSM

Sekarang kita membuat inti praktikum.

FSM memiliki:

```text
Patrol
Chase
Attack
Flee
Dead
```

Transition utama:

```text
Patrol
  │ melihat Player
  ▼
Chase
  │ Player cukup dekat
  ▼
Attack

Patrol / Chase / Attack
        │
        │ HP rendah
        ▼
       Flee

Flee
  │ sudah aman
  ▼
Patrol
```

Global transition:

```text
ANY STATE
    │
 HP <= 0
    ▼
   Dead
```

---

# 34. Membuat EnemyFSM

Buat:

```text
EnemyFSM.cs
```

Isi script berikut.

```csharp
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Flee,
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

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float lostPlayerDelay = 2f;

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

    [Header("Debug")]
    [SerializeField] private EnemyState currentState;

    private int currentPatrolIndex = 0;

    private float lostPlayerTimer = 0f;
    private float nextAttackTime = 0f;

    private bool fleeTriggered = false;

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
                UpdatePatrol();
                break;

            case EnemyState.Chase:
                UpdateChase();
                break;

            case EnemyState.Attack:
                UpdateAttack();
                break;

            case EnemyState.Flee:
                UpdateFlee();
                break;

            case EnemyState.Dead:
                UpdateDead();
                break;
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

            case EnemyState.Chase:

                agent.isStopped = false;
                agent.speed = chaseSpeed;

                lostPlayerTimer = 0f;

                break;

            case EnemyState.Attack:

                agent.isStopped = true;
                agent.ResetPath();

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

    private void UpdatePatrol()
    {
        if (perception.CanSeePlayer)
        {
            ChangeState(EnemyState.Chase);
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
                    EnemyState.Patrol
                );

                return;
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

        float safePointDistance =
            Vector3.Distance(
                transform.position,
                safePoint.position
            );

        if (playerDistance >= safeDistance ||
            safePointDistance <= 1f)
        {
            ChangeState(
                EnemyState.Patrol
            );

            return;
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
    }
}
```

---

# 35. Memahami Enum

Bagian:

```csharp
public enum EnemyState
{
    Patrol,
    Chase,
    Attack,
    Flee,
    Dead
}
```

`enum` digunakan untuk mendefinisikan sekumpulan pilihan nilai yang terbatas.

Daripada menggunakan:

```text
0 = Patrol
1 = Chase
2 = Attack
3 = Flee
```

kita menggunakan:

```csharp
EnemyState.Patrol
EnemyState.Chase
EnemyState.Attack
EnemyState.Flee
```

Kode menjadi jauh lebih mudah dibaca.

---

# 36. currentState

Variabel:

```csharp
private EnemyState currentState;
```

menyimpan state yang sedang aktif.

Contoh:

```text
currentState = Chase
```

berarti:

```text
Enemy sedang menjalankan perilaku Chase.
```

Pada satu waktu hanya satu state utama yang aktif.

---

# 37. Fungsi ChangeState()

Bagian paling penting:

```csharp
private void ChangeState(EnemyState newState)
```

Alurnya:

```text
State lama
   ↓
ExitState()
   ↓
currentState = state baru
   ↓
EnterState()
```

Contoh:

```text
Patrol
   │
player terlihat
   ▼
ExitState(Patrol)
   ↓
currentState = Chase
   ↓
EnterState(Chase)
```

Keuntungan menggunakan fungsi khusus ini adalah semua perubahan state melalui satu jalur yang terkontrol.

---

# 38. EnterState()

`EnterState()` hanya dijalankan ketika NPC **baru memasuki state**.

Contoh Chase:

```csharp
agent.speed = chaseSpeed;
```

Tidak perlu mengatur speed tersebut setiap frame.

Contoh Attack:

```csharp
agent.isStopped = true;
```

Enemy berhenti ketika mulai menyerang.

---

# 39. Update State

Berbeda dengan `EnterState()`:

```text
EnterState
→ sekali saat masuk
```

sementara:

```text
UpdatePatrol()
UpdateChase()
UpdateAttack()
UpdateFlee()
```

dijalankan setiap frame selama state tersebut aktif.

Contoh:

```text
State = Chase
```

maka:

```csharp
UpdateChase();
```

terus dijalankan.

---

# 40. ExitState()

`ExitState()` dijalankan ketika NPC meninggalkan sebuah state.

Contoh:

```csharp
case EnemyState.Attack:
    agent.isStopped = false;
    break;
```

Artinya setelah keluar dari Attack, NavMeshAgent diperbolehkan bergerak kembali.

---

# 41. State Patrol

FSM menjalankan:

```csharp
UpdatePatrol();
```

Enemy menuju waypoint.

Ketika sampai:

```text
W1 → W2
W2 → W3
W3 → W4
W4 → W1
```

Transition:

```text
IF CanSeePlayer
    Patrol → Chase
```

kode:

```csharp
if (perception.CanSeePlayer)
{
    ChangeState(EnemyState.Chase);
}
```

---

# 42. State Chase

Dalam Chase:

```csharp
agent.SetDestination(
    player.position
);
```

Destination selalu diperbarui mengikuti Player.

Transition:

```text
IF distance <= attackRange
    Chase → Attack
```

---

# 43. Player Hilang dan Timer

Jika Player tidak terlihat:

```csharp
lostPlayerTimer += Time.deltaTime;
```

Enemy tidak langsung kembali ke Patrol.

Enemy menunggu:

```text
lostPlayerDelay = 2 detik
```

Jika selama dua detik Player masih tidak terlihat:

```text
Chase → Patrol
```

Ini menerapkan konsep:

```text
Timer-based transition
```

yang membantu mencegah perubahan state terlalu cepat.

---

# 44. State Attack

Enemy masuk Attack jika:

```text
distance <= attackRange
```

Contoh:

```text
attackRange = 2 meter
```

Pada Attack:

```text
Enemy berhenti
↓
Enemy menghadap Player
↓
Enemy menyerang
↓
Menunggu cooldown
↓
Menyerang lagi
```

---

# 45. Mengapa Attack Membutuhkan Cooldown?

Unity menjalankan `Update()` setiap frame.

Tanpa cooldown:

```text
60 FPS
→ Attack dapat terjadi ±60 kali/detik
```

Karena itu digunakan:

```csharp
if (Time.time >= nextAttackTime)
```

Setelah menyerang:

```csharp
nextAttackTime =
    Time.time + attackCooldown;
```

Jika:

```text
attackCooldown = 1.5
```

enemy hanya dapat menyerang setiap 1,5 detik.

---

# 46. Hysteresis pada Attack Range

Perhatikan dua parameter:

```text
attackRange     = 2.0
attackExitRange = 2.75
```

Enemy masuk Attack jika:

```text
distance <= 2.0
```

tetapi baru keluar Attack jika:

```text
distance > 2.75
```

Ini disebut:

# Hysteresis

Tanpa hysteresis:

```text
1.99 → Attack
2.01 → Chase
1.99 → Attack
2.01 → Chase
```

NPC dapat berganti state sangat cepat.

Dengan hysteresis:

```text
Masuk Attack = 2.0
Keluar Attack = 2.75
```

tersedia daerah toleransi.

---

# 47. State Flee

Condition:

```csharp
health.CurrentHealth <=
lowHealthThreshold
```

Misalnya:

```text
Max HP = 100
Low Health Threshold = 30
```

Ketika HP:

```text
30 atau lebih rendah
```

maka:

```text
Attack / Chase / Patrol
          ↓
         Flee
```

---

# 48. Mengapa Digunakan `fleeTriggered`?

Ada permasalahan menarik.

Misalnya:

```text
HP = 20
```

Enemy Flee dan setelah aman kembali Patrol.

Tetapi HP masih:

```text
20
```

Jika hanya menggunakan condition:

```text
HP <= 30 → Flee
```

maka pada frame berikutnya:

```text
Patrol → Flee
```

lagi.

Karena itu digunakan:

```csharp
private bool fleeTriggered = false;
```

Flee hanya dipicu satu kali dalam skenario praktikum dasar.

Ini sekaligus menunjukkan kepada mahasiswa bahwa transition FSM perlu mempertimbangkan **state history dan kondisi keluar**.

Untuk versi lanjutan dapat ditambahkan sistem Heal sehingga `fleeTriggered` di-reset ketika health pulih.

---

# 49. State Dead

Condition global:

```csharp
if (health.IsDead)
```

akan memiliki prioritas tertinggi.

Alur:

```text
Patrol ─┐
Chase  ─┤
Attack ─┼── HP <= 0 ──→ Dead
Flee   ─┘
```

Ini merupakan contoh:

# Global Transition

Condition tersebut tidak hanya berlaku pada satu state.

---

# 50. Transition Priority

Urutan evaluasi sangat penting.

Pada script:

```text
1. Dead
2. Flee
3. State-specific transition
```

Contoh:

Enemy sedang berada dekat Player sekaligus HP = 0.

Ada dua kemungkinan:

```text
Attack
Dead
```

Tetapi yang seharusnya dipilih:

```text
Dead
```

Karena itu:

```text
Dead
```

mempunyai prioritas tertinggi.

---

# 51. Membuat Test Attack Player

Kita membutuhkan cara sederhana untuk mengurangi health Enemy sehingga state Flee dapat diuji.

Buat:

```text
PlayerAttackTest.cs
```

Isi:

```csharp
using UnityEngine;

public class PlayerAttackTest : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemy;
    [SerializeField] private float attackDistance = 3f;
    [SerializeField] private float damage = 20f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryAttackEnemy();
        }
    }

    private void TryAttackEnemy()
    {
        if (enemy == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

        if (distance <= attackDistance)
        {
            enemy.TakeDamage(damage);

            Debug.Log(
                "Player attacks Enemy!"
            );
        }
    }
}
```

Attach ke Player.

Atur:

```text
Enemy = Enemy
Attack Distance = 3
Damage = 20
```

Sekarang:

```text
SPACE
```

digunakan sebagai serangan testing.

---

# 52. Konfigurasi EnemyFSM di Inspector

Pilih Enemy.

Isi:

## References

```text
Player      = Player
Agent       = Enemy
Perception  = Enemy
Health      = Enemy
```

---

## Patrol

```text
Patrol Points Size = 4

Element 0 = W1
Element 1 = W2
Element 2 = W3
Element 3 = W4

Patrol Speed       = 2
Waypoint Tolerance = 0.5
```

---

## Chase

```text
Chase Speed      = 4
Lost Player Delay = 2
```

---

## Attack

```text
Attack Range      = 2
Attack Exit Range = 2.75
Attack Damage     = 10
Attack Cooldown   = 1.5
```

---

## Flee

```text
Safe Point           = SafePoint
Flee Speed           = 5
Low Health Threshold = 30
Safe Distance        = 12
```

---

# 53. Parameter Rekomendasi

Gunakan nilai awal:

| Parameter | Nilai |
|---|---:|
| Patrol Speed | 2 |
| Chase Speed | 4 |
| Flee Speed | 5 |
| Vision Range | 10 |
| Vision Angle | 90° |
| Attack Range | 2 |
| Attack Exit Range | 2.75 |
| Attack Damage | 10 |
| Attack Cooldown | 1.5 s |
| Max Enemy HP | 100 |
| Low Health Threshold | 30 |
| Safe Distance | 12 |
| Lost Player Delay | 2 s |

Parameter tersebut hanya nilai awal untuk praktikum.

Mahasiswa dianjurkan melakukan tuning.

---

# 54. Cara Pengujian Praktikum

Tekan:

```text
Play
```

---

## Test 1 — Patrol

Jauhkan Player dari Enemy.

Expected result:

```text
Enemy:
W1 → W2 → W3 → W4 → W1
```

Console:

```text
Enemy → State: Patrol
```

---

# 55. Test 2 — Detection

Gerakkan Player menggunakan:

```text
WASD
```

masuk ke vision range Enemy.

Pastikan:

- Player berada di depan Enemy;
- tidak ada obstacle.

Expected:

```text
Patrol → Chase
```

Console:

```text
Enemy → State: Chase
```

---

# 56. Test 3 — Field of View

Letakkan Player:

```text
di belakang Enemy
```

walaupun jaraknya dekat.

Expected:

```text
Enemy tidak langsung melihat Player
```

karena Player berada di luar:

```text
visionAngle
```

---

# 57. Test 4 — Line of Sight

Letakkan obstacle:

```text
Enemy → Obstacle → Player
```

Expected:

```text
Enemy tidak dapat melihat Player
```

walaupun Player:

- berada dalam visionRange;
- berada dalam visionAngle.

Penyebab:

```text
Raycast mengenai Obstacle.
```

---

# 58. Test 5 — Chase

Masuk ke area vision.

Expected:

```text
Enemy mengejar Player
```

Jika Player bergerak melewati obstacle:

```text
NavMeshAgent
```

akan mengikuti jalur navigasi yang tersedia.

---

# 59. Test 6 — Attack

Biarkan Enemy mendekati Player.

Ketika:

```text
distance <= attackRange
```

expected:

```text
Chase → Attack
```

Enemy berhenti dan menyerang Player.

Console:

```text
Enemy attacks Player!
Player HP: 90
```

kemudian:

```text
Player HP: 80
Player HP: 70
...
```

sesuai cooldown.

---

# 60. Test 7 — Attack Hysteresis

Saat Enemy menyerang, gerakkan Player sedikit.

Misalnya:

```text
distance = 2.2
```

Enemy tidak langsung berpindah ke Chase.

Enemy baru keluar jika:

```text
distance > 2.75
```

Ini menunjukkan hysteresis.

---

# 61. Test 8 — Lost Player Timer

Biarkan Enemy Chase.

Kemudian sembunyikan Player di balik obstacle.

Expected:

```text
Player hilang
↓
Enemy tidak langsung Patrol
↓
menunggu ±2 detik
↓
Chase → Patrol
```

---

# 62. Test 9 — Flee

Dekati Enemy.

Tekan:

```text
SPACE
```

berulang kali.

Damage:

```text
20
```

Contoh:

```text
HP 100
 ↓
80
 ↓
60
 ↓
40
 ↓
20
```

Ketika:

```text
HP <= 30
```

expected:

```text
Attack / Chase
      ↓
     Flee
```

Enemy menuju:

```text
SafePoint
```

---

# 63. Test 10 — Dead

Terus serang Enemy sampai:

```text
HP = 0
```

Expected:

```text
ANY STATE
   ↓
Dead
```

Enemy berhenti bergerak.

---

# 64. Visual Debugging dengan Gizmos

Saat Enemy dipilih di Scene View, script perception menggambar:

```text
Vision Range
```

dan arah batas:

```text
Vision Angle
```

EnemyFSM juga menggambar:

```text
Attack Range
Attack Exit Range
```

Contoh visual konseptual:

```text
          Vision Range
        /              \
       /                \
      /                  \
     ● Enemy

       ○ Attack Range
       ◯ Attack Exit Range
```

---

# 65. Apa Itu Gizmos?

**Gizmos** adalah visual debugging yang ditampilkan di Unity Scene View.

Gizmos:

- tidak menjadi object gameplay;
- membantu developer memahami parameter;
- sangat cocok untuk AI;
- dapat menunjukkan radius dan arah.

Contoh:

```csharp
Gizmos.DrawWireSphere(
    transform.position,
    attackRange
);
```

---

# 66. Debugging FSM

State aktif juga muncul di Inspector:

```text
Current State
```

dan Console:

```text
Enemy → State: Patrol
Enemy → State: Chase
Enemy → State: Attack
Enemy → State: Flee
```

Ini sangat dianjurkan ketika belajar AI.

Jangan hanya mengamati gerakan NPC.

Amati juga:

```text
State apa yang aktif?
Condition apa yang menyebabkan transition?
```

---

# 67. Tabel Transition Praktikum

| Current State | Condition | Next State |
|---|---|---|
| Patrol | Player terlihat | Chase |
| Chase | Player dalam attack range | Attack |
| Chase | Player hilang selama 2 detik | Patrol |
| Attack | Player keluar attack exit range | Chase |
| Attack | Player tidak terlihat | Chase |
| Patrol | HP rendah | Flee |
| Chase | HP rendah | Flee |
| Attack | HP rendah | Flee |
| Flee | Sudah aman | Patrol |
| Any | HP <= 0 | Dead |

---

# 68. Diagram FSM Final

```text
                   PLAYER TERLIHAT
            ┌─────────────────────────┐
            │                         ▼
        ┌────────┐                ┌────────┐
        │ PATROL │                │ CHASE  │
        └────────┘                └────────┘
             ▲                         │
             │                         │
             │ Player hilang           │ distance
             │ selama 2 detik          │ <= attackRange
             │                         ▼
             │                    ┌────────┐
             │                    │ ATTACK │
             │                    └────────┘
             │                         │
             │                         │
             │                    distance >
             │                  attackExitRange
             │                         │
             │                         └──→ CHASE
             │
             │      sudah aman
             │          ▲
             │          │
             │      ┌────────┐
             └──────│  FLEE  │
                    └────────┘
                         ▲
                         │
                      HP rendah
                         │
             PATROL / CHASE / ATTACK


        Dari STATE mana pun:

               HP <= 0
                  │
                  ▼
              ┌────────┐
              │  DEAD  │
              └────────┘
```

---

# 69. Hubungan Setiap Script

Struktur:

```text
Enemy
│
├── NavMeshAgent
│
├── EnemyPerception.cs
│       │
│       └── CanSeePlayer
│
├── EnemyHealth.cs
│       │
│       └── CurrentHealth
│
└── EnemyFSM.cs
        │
        ├── membaca Perception
        ├── membaca Health
        └── mengontrol NavMeshAgent


Player
│
├── CharacterController
├── SimplePlayerController.cs
├── PlayerHealth.cs
└── PlayerAttackTest.cs
```

Pemisahan ini penting.

Jangan membuat satu script dengan ribuan baris yang melakukan semuanya.

---

# 70. Separation of Concern

Setiap script mempunyai tanggung jawab.

## EnemyPerception

Menjawab:

```text
Apakah Enemy melihat Player?
```

---

## EnemyHealth

Menjawab:

```text
Berapa HP Enemy?
```

---

## EnemyFSM

Menjawab:

```text
Enemy harus melakukan apa?
```

---

## NavMeshAgent

Menjawab:

```text
Bagaimana Enemy bergerak menuju target?
```

Ini membuat sistem lebih mudah:

- dibaca;
- diuji;
- dikembangkan;
- diperbaiki.

---

# 71. Istilah Teknis Unity yang Digunakan

## GameObject

Objek dasar di dalam Unity Scene.

Contoh:

```text
Player
Enemy
Ground
Waypoint
SafePoint
```

---

## Component

Fungsi atau kemampuan yang ditempelkan pada GameObject.

Contoh Enemy:

```text
Enemy GameObject
├── Transform
├── Capsule Collider
├── NavMeshAgent
├── EnemyPerception
├── EnemyHealth
└── EnemyFSM
```

Unity menggunakan pendekatan:

```text
GameObject + Components
```

---

## Transform

Setiap GameObject mempunyai Transform.

Transform menyimpan:

```text
Position
Rotation
Scale
```

Contoh:

```csharp
player.position
transform.position
transform.forward
```

---

## Inspector

Panel Unity untuk melihat dan mengatur:

- Component;
- parameter;
- reference;
- material;
- transform;
- script field.

---

## SerializeField

Membuat field private dapat diedit melalui Inspector:

```csharp
[SerializeField]
private float chaseSpeed = 4f;
```

---

## MonoBehaviour

Class dasar yang digunakan oleh script Unity yang ditempelkan pada GameObject.

Contoh:

```csharp
public class EnemyFSM : MonoBehaviour
```

---

## Awake()

Dipanggil ketika object/script diinisialisasi.

Pada praktikum digunakan untuk mengambil reference:

```csharp
agent = GetComponent<NavMeshAgent>();
```

---

## Start()

Biasanya digunakan untuk initialization sebelum gameplay dimulai.

Contoh:

```csharp
ChangeState(EnemyState.Patrol);
```

---

## Update()

Dipanggil setiap frame.

FSM menggunakan Update untuk:

```text
membaca global condition
↓
menjalankan state aktif
```

---

## GetComponent

Digunakan untuk mengambil component pada GameObject.

Contoh:

```csharp
GetComponent<NavMeshAgent>();
```

---

## Vector3

Representasi nilai 3 dimensi:

```text
X
Y
Z
```

Contoh posisi:

```csharp
Vector3 position;
```

---

## Vector3.Distance

Menghitung jarak antara dua posisi.

```csharp
Vector3.Distance(
    transform.position,
    player.position
);
```

---

## Quaternion

Digunakan untuk merepresentasikan rotasi.

Pada praktikum digunakan ketika Enemy menghadap Player.

---

## Physics.Raycast

Menguji apakah terdapat object pada sebuah arah tertentu.

Digunakan untuk:

```text
Line of Sight
```

---

## LayerMask

Filter layer untuk physics query seperti Raycast.

---

## NavMeshSurface

Komponen untuk mendefinisikan dan membangun area NavMesh.

---

## NavMeshAgent

Component navigation yang digunakan Enemy untuk mengikuti jalur pada NavMesh.

---

## SetDestination()

Memberikan destination kepada NavMeshAgent:

```csharp
agent.SetDestination(
    player.position
);
```

---

## isStopped

Menghentikan atau melanjutkan pergerakan agent.

Contoh:

```csharp
agent.isStopped = true;
```

digunakan saat Attack.

---

## pathPending

Menunjukkan bahwa perhitungan path masih diproses.

---

## remainingDistance

Mengukur sisa jarak agent menuju destination.

Digunakan untuk menentukan apakah Enemy telah mencapai waypoint.

---

# 72. Masalah Umum dan Troubleshooting

## Masalah 1

Enemy tidak bergerak.

### Periksa:

```text
Apakah NavMesh sudah dibangun?
```

dan:

```text
Apakah Enemy berada di atas NavMesh?
```

---

# 73. Error SetDestination

Jika muncul masalah terkait agent tidak berada pada NavMesh:

periksa:

```text
1. NavMeshSurface sudah dibuild.
2. Enemy tepat berada pada area NavMesh.
3. NavMeshAgent aktif.
4. Agent Type sesuai dengan NavMesh.
```

---

# 74. Enemy Tidak Melihat Player

Periksa:

```text
Player reference
Vision Range
Vision Angle
Obstacle Mask
```

Pastikan Player tidak berada:

```text
di belakang Enemy
```

atau:

```text
terhalang obstacle
```

---

# 75. Enemy Bisa Melihat Menembus Tembok

Periksa Layer dari obstacle:

```text
Obstacle
```

Kemudian periksa:

```text
EnemyPerception
→ Obstacle Mask
→ Obstacle ☑
```

Jika layer tidak masuk LayerMask, Raycast tidak akan memperlakukannya sebagai penghalang.

---

# 76. Enemy Tidak Patrol

Periksa:

```text
Patrol Points Size
```

harus lebih besar dari 0.

Contoh:

```text
Size = 4
```

dan semua:

```text
Element 0
Element 1
Element 2
Element 3
```

harus diisi.

---

# 77. Enemy Tidak Attack

Periksa:

```text
Attack Range
```

Gunakan Gizmos.

Pastikan Player masuk lingkaran:

```text
Attack Range
```

---

# 78. Enemy Tidak Flee

Periksa:

```text
Low Health Threshold
```

Contoh:

```text
30
```

Pastikan Enemy mendapat damage hingga:

```text
Current Health <= 30
```

---

# 79. Enemy Tidak Menuju SafePoint

Periksa:

```text
SafePoint reference
```

dan pastikan:

```text
SafePoint
```

berada di atas area yang dapat dicapai NavMesh.

---

# 80. NPC Berganti State Terlalu Cepat

Gejala:

```text
Chase
Attack
Chase
Attack
Chase
Attack
```

Solusi yang telah digunakan:

```text
attackRange     = 2
attackExitRange = 2.75
```

Ini adalah hysteresis.

---

# 81. Eksperimen Mahasiswa 1 — Vision Range

Ubah:

```text
Vision Range
```

dari:

```text
10
```

menjadi:

```text
5
15
20
```

Pertanyaan:

> Bagaimana perubahan vision range memengaruhi perilaku Enemy?

---

# 82. Eksperimen Mahasiswa 2 — Vision Angle

Ubah:

```text
90°
```

menjadi:

```text
45°
180°
360°
```

Bandingkan perilaku.

---

# 83. Eksperimen Mahasiswa 3 — Patrol vs Chase Speed

Coba:

```text
Patrol Speed = 2
Chase Speed  = 2
```

kemudian:

```text
Patrol Speed = 2
Chase Speed  = 6
```

Diskusikan pengaruhnya terhadap gameplay.

---

# 84. Eksperimen Mahasiswa 4 — Attack Range

Bandingkan:

```text
Attack Range = 1
```

dengan:

```text
Attack Range = 4
```

Apa dampaknya?

---

# 85. Eksperimen Mahasiswa 5 — Low Health Threshold

Bandingkan:

```text
20
30
50
80
```

Enemy dengan:

```text
Low Health Threshold = 80
```

akan terlihat jauh lebih defensif.

---

# 86. Eksperimen Mahasiswa 6 — Attack Cooldown

Bandingkan:

```text
0.5
1.5
3
```

Parameter ini memengaruhi agresivitas Enemy.

---

# 87. Tugas Pengembangan Opsional — Search State

Tambahkan:

```text
Search
```

sehingga alur berubah dari:

```text
Chase
↓ Player hilang
Patrol
```

menjadi:

```text
Chase
↓ Player hilang
Search
↓ gagal menemukan Player
Patrol
```

Search dapat menggunakan:

```text
lastSeenPosition
```

yang merupakan integrasi dengan konsep Memory.

---

# 88. Tugas Pengembangan Opsional — Alert State

Tambahkan:

```text
Alert
```

Contoh:

```text
Patrol
↓ mendeteksi sesuatu
Alert
↓ Player dikonfirmasi
Chase
```

---

# 89. Tugas Pengembangan Opsional — Heal

Saat Enemy sampai SafePoint:

```text
Flee
↓
Heal
↓ HP pulih
Patrol
```

Contoh hierarchical behavior:

```text
Alive
├── Normal
│   └── Patrol
│
└── Combat
    ├── Chase
    ├── Attack
    └── Flee
```

---

# 90. Tugas Pengembangan Opsional — Animation

Hubungkan state dengan Animator.

Contoh:

```text
Patrol → Walk
Chase  → Run
Attack → Attack Animation
Flee   → Run
Dead   → Death
```

FSM tetap menangani keputusan.

Animator hanya menangani visual animation.

---

# 91. Evaluasi Praktikum

Mahasiswa dinyatakan berhasil jika:

- [ ] Enemy melakukan Patrol.
- [ ] Patrol menggunakan waypoint.
- [ ] Enemy memiliki Field of View.
- [ ] Obstacle menghalangi penglihatan.
- [ ] Player terlihat menyebabkan Patrol → Chase.
- [ ] Enemy mengejar menggunakan NavMeshAgent.
- [ ] Player dekat menyebabkan Chase → Attack.
- [ ] Attack memiliki cooldown.
- [ ] Player menjauh menyebabkan Attack → Chase.
- [ ] Enemy tidak langsung lupa Player.
- [ ] HP rendah menyebabkan Flee.
- [ ] Flee menuju SafePoint.
- [ ] Flee dapat selesai.
- [ ] HP 0 menyebabkan Dead.
- [ ] State dapat dilihat melalui debug.
- [ ] Vision dan attack range terlihat melalui Gizmos.

---

# 92. Pertanyaan Analisis

Jawab setelah praktikum.

### 1.
Apa perbedaan antara **State**, **Condition**, dan **Transition**?

### 2.
Mengapa `NavMeshAgent` bukan merupakan FSM?

### 3.
Apa fungsi `EnemyPerception`?

### 4.
Mengapa Line of Sight menggunakan Raycast?

### 5.
Apa perbedaan `Tag` dan `Layer`?

### 6.
Mengapa Attack membutuhkan cooldown?

### 7.
Mengapa `attackRange` dan `attackExitRange` dibuat berbeda?

### 8.
Apa masalah yang terjadi jika keduanya bernilai sama?

### 9.
Mengapa condition Dead diperiksa sebelum Flee?

### 10.
Mengapa FSM sebaiknya tidak seluruhnya ditulis dalam satu `if-else` besar?

---

# 93. Alur Keseluruhan Praktikum

```text
START
  │
  ▼
PATROL
  │
  │ CanSeePlayer
  ▼
CHASE
  │
  │ distance <= attackRange
  ▼
ATTACK
  │
  ├── Player menjauh ───→ CHASE
  │
  │ HP rendah
  ▼
FLEE
  │
  │ Safe
  ▼
PATROL


GLOBAL:

HP <= 0
   │
   ▼
 DEAD
```

---

# 94. Konsep AI yang Telah Digabungkan

Praktikum ini sebenarnya menggabungkan beberapa materi.

```text
Perception
      │
      ▼
Field of View
Raycast
Distance
      │
      ▼
Finite State Machine
      │
      ▼
Decision Making
      │
      ▼
NavMesh Navigation
      │
      ▼
Movement
      │
      ▼
Action / Attack
```

Dengan demikian mahasiswa dapat melihat bahwa sistem Game AI bukan sekadar satu algoritma.

Beberapa subsistem bekerja bersama.

---

# 95. Rekomendasi Praktikum Terbaik

Untuk Praktikum 05, implementasi yang paling direkomendasikan adalah:

## **Core Praktikum**

```text
Enum FSM
+
Patrol
+
Chase
+
Attack
+
Flee
+
Dead
+
NavMeshAgent
+
FOV + Raycast
+
Health
+
Attack Cooldown
+
Hysteresis
+
Debug Gizmos
```

### Tidak disarankan sebagai requirement utama:

```text
Class-based FSM
Hierarchical FSM penuh
Animation kompleks
Search
Alert
Utility AI
Behavior Tree
```

Bukan karena konsep tersebut tidak penting, tetapi karena akan menggeser fokus praktikum dari:

```text
memahami FSM
```

menjadi:

```text
mengelola kompleksitas software.
```

---

# 96. Pembagian Level Praktikum yang Direkomendasikan

## LEVEL A — Wajib

```text
Patrol
Chase
Attack
Flee
Dead
```

Semua mahasiswa harus berhasil menyelesaikannya.

---

## LEVEL B — Penguatan

```text
FOV
Raycast
Cooldown
Hysteresis
Lost Player Timer
Gizmos
```

Direkomendasikan masuk ke modul utama karena konsep AI menjadi jauh lebih jelas.

---

## LEVEL C — Tantangan

Pilih satu:

```text
Search State
Alert State
Heal State
Animation
Multiple Safe Points
Dynamic Flee Destination
UI State Display
```

Level ini dapat menjadi tugas pengembangan.

---

# 97. Mengapa Pilihan Ini Direkomendasikan?

Karena praktikum memberikan hubungan yang sangat jelas:

```text
Perception
↓
Condition
↓
Transition
↓
State
↓
Navigation
↓
Action
```

Mahasiswa tidak hanya melihat NPC berjalan.

Mahasiswa dapat menjelaskan:

> Enemy mengejar Player karena `CanSeePlayer` menjadi true sehingga FSM melakukan transition dari Patrol ke Chase, kemudian state Chase memerintahkan NavMeshAgent memperbarui destination ke posisi Player.

Itulah tujuan pembelajaran yang jauh lebih penting daripada hanya menghasilkan NPC yang tampak pintar.

---

# 98. Hasil Akhir yang Diharapkan

Saat praktikum selesai:

```text
1. Enemy berpatroli.

2. Player mendekati Enemy.

3. Enemy melihat Player.

4. FSM:
   Patrol → Chase

5. Enemy mengikuti Player
   menggunakan NavMeshAgent.

6. Player mendekat.

7. FSM:
   Chase → Attack

8. Enemy menyerang dengan cooldown.

9. Player menyerang Enemy.

10. Enemy HP <= threshold.

11. FSM:
    Attack → Flee

12. Enemy menuju SafePoint.

13. Enemy merasa aman.

14. FSM:
    Flee → Patrol

15. Jika HP = 0:
    Any State → Dead
```

---

# 99. Kesimpulan

Pada praktikum ini mahasiswa telah mengimplementasikan **Finite State Machine** sebagai mekanisme decision making NPC.

Konsep utama yang digunakan:

```text
State
Transition
Condition
Priority
Enter State
Update State
Exit State
Timer
Hysteresis
Global Transition
```

FSM kemudian dihubungkan dengan:

```text
Perception
Navigation
Movement
Health
Combat
Debugging
```

Sehingga menghasilkan Enemy AI:

```text
Patrol
   ↓
Chase
   ↓
Attack
   ↓
Flee
```

dengan:

```text
Dead
```

sebagai global/final state.

Praktikum ini menjadi dasar yang baik sebelum mahasiswa mempelajari pendekatan decision making yang lebih kompleks seperti:

```text
Hierarchical FSM
Behavior Tree
Utility AI
```

---

# Ringkasan Script

```text
Scripts/
│
├── SimplePlayerController.cs
│   └── Menggerakkan Player
│
├── PlayerHealth.cs
│   └── Menyimpan health Player
│
├── PlayerAttackTest.cs
│   └── Memberi damage ke Enemy untuk testing
│
├── EnemyPerception.cs
│   ├── Vision Range
│   ├── Vision Angle
│   └── Raycast Line of Sight
│
├── EnemyHealth.cs
│   └── Menyimpan health Enemy
│
└── EnemyFSM.cs
    ├── Patrol
    ├── Chase
    ├── Attack
    ├── Flee
    └── Dead
```

# Checklist Akhir Sebelum Play

- [ ] AI Navigation tersedia.
- [ ] Ground sudah masuk proses NavMesh build.
- [ ] NavMesh sudah berhasil dibuat.
- [ ] Enemy berada di atas NavMesh.
- [ ] Enemy memiliki NavMeshAgent.
- [ ] Player reference sudah terpasang.
- [ ] Patrol Points sudah diisi.
- [ ] SafePoint sudah diisi.
- [ ] EnemyPerception sudah terpasang.
- [ ] Obstacle menggunakan Layer `Obstacle`.
- [ ] Obstacle Mask sudah memilih `Obstacle`.
- [ ] EnemyHealth sudah terpasang.
- [ ] PlayerHealth sudah terpasang.
- [ ] EnemyFSM sudah terpasang.
- [ ] PlayerAttackTest sudah mendapatkan Enemy reference.
- [ ] Vision Range terlihat melalui Gizmos.
- [ ] Attack Range terlihat melalui Gizmos.

Jika seluruh checklist terpenuhi, project siap diuji.