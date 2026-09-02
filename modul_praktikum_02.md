# MODUL PRAKTIKUM 2  
## NPC Guard: Sensor + Memory + Decision

**Mata Kuliah:** Game Cerdas  
**Program:** S1 Teknik Informatika – Semester 7  
**Game Engine:** Unity 6  
**Bahasa Pemrograman:** C#  

---

# 1. Tujuan Praktikum

Pada praktikum ini mahasiswa akan membuat **NPC Guard** sederhana yang dapat:

1. Berjalan patroli mengikuti beberapa waypoint.
2. Mendeteksi Player berdasarkan:
   - jarak,
   - Field of View,
   - Line of Sight.
3. Mengejar Player ketika Player terlihat.
4. Mengingat posisi terakhir Player.
5. Menuju posisi terakhir tersebut ketika Player menghilang.
6. Mencari Player selama beberapa detik.
7. Kembali patroli jika Player tidak ditemukan.
8. Kembali mengejar jika Player muncul lagi.
9. Menampilkan visual debugging melalui Gizmos.
10. Mengubah parameter AI melalui Inspector.

Arsitektur yang digunakan mengikuti konsep Pertemuan 2:

```text
Environment
     ↓
Perception
     ↓
Memory
     ↓
Decision
     ↓
Action
     ↓
Environment
```

Target perilaku akhir:

```text
PATROL
   │
   │ Player terlihat
   ▼
CHASE
   │
   │ Player menghilang
   ▼
SEARCH
   │
   ├──────── Player ditemukan ────────→ CHASE
   │
   │ Tidak ditemukan
   ▼
PATROL
```

Materi ini mengimplementasikan secara langsung konsep **Sensor, Memory, Decision, Action, parameter AI, dan debugging Gizmos** dari Pertemuan 2.

---

# 2. Hasil Akhir Praktikum

Scene akhir akan memiliki struktur seperti berikut:

```text
Scene
│
├── Ground
├── Navigation
│   └── NavMesh Surface
│
├── Player
│   └── PlayerController
│
├── NPC_Guard
│   ├── NavMeshAgent
│   ├── NPCSensor
│   ├── NPCBrain
│   └── DirectionMarker
│
├── Wall
│
└── PatrolPoints
    ├── Point1
    ├── Point2
    ├── Point3
    └── Point4
```

Player dapat dikontrol menggunakan:

```text
W / ↑ = maju
S / ↓ = mundur
A / ← = kiri
D / → = kanan
```

NPC kemudian bereaksi terhadap pergerakan Player.

---

# 3. Membuat Project Unity 6

Buka **Unity Hub**.

Pilih:

```text
New Project
```

Gunakan template:

```text
Universal 3D
```

atau:

```text
3D
```

Nama project:

```text
GameCerdas_Praktikum02
```

Klik:

```text
Create Project
```

---

# 4. Membuat Struktur Folder

Di dalam jendela Project, buat folder:

```text
Assets
│
├── Materials
├── Prefabs
├── Scenes
└── Scripts
```

Tujuannya agar asset project lebih terorganisir.

---

# 5. Menyimpan Scene

Pilih:

```text
File
→ Save As
```

Simpan di:

```text
Assets/Scenes
```

dengan nama:

```text
Praktikum02_NPCGuard
```

---

# 6. Membuat Ground

Pada Hierarchy pilih:

```text
GameObject
→ 3D Object
→ Plane
```

Rename:

```text
Ground
```

Atur Transform:

```text
Position
X = 0
Y = 0
Z = 0

Scale
X = 3
Y = 1
Z = 3
```

Arena sekarang memiliki ukuran yang cukup luas untuk pengujian.

---

# 7. Membuat Material Ground

Masuk:

```text
Assets/Materials
```

Klik kanan:

```text
Create
→ Material
```

Nama:

```text
GroundMaterial
```

Pilih warna yang mudah dibedakan.

Drag material tersebut ke object:

```text
Ground
```

---

# 8. Membuat Player

Pada Hierarchy:

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
Position
X = 0
Y = 1
Z = 0
```

Capsule akan menjadi karakter Player sederhana.

---

# 9. Memberikan Material Player

Buat material:

```text
PlayerMaterial
```

Misalnya gunakan warna biru.

Drag material tersebut ke:

```text
Player
```

Tujuannya agar Player mudah dibedakan dari NPC.

---

# 10. Membuat Tag Player

Pilih object:

```text
Player
```

Pada Inspector:

```text
Tag
→ Add Tag
```

Tambahkan:

```text
Player
```

Kembali ke object Player.

Set:

```text
Tag = Player
```

---

# 11. Membuat Layer Player dan Obstacle

Pilih:

```text
Layer
→ Add Layer
```

Tambahkan dua Layer:

```text
Player
Obstacle
```

Kemudian pilih Player.

Set:

```text
Layer = Player
```

Layer Obstacle akan digunakan untuk Wall.

---

# 12. Membuat Player Controller

Sekarang Player akan dibuat dapat bergerak menggunakan:

```text
WASD
```

dan:

```text
Arrow Key
```

Masuk ke:

```text
Assets/Scripts
```

Klik kanan:

```text
Create
→ Scripting
→ MonoBehaviour Script
```

Nama:

```text
PlayerController
```

Buka script tersebut.

Hapus seluruh isinya dan gunakan kode berikut.

```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 10f;

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(
            horizontal,
            0f,
            vertical
        ).normalized;

        // Gerakkan Player
        transform.position +=
            movement *
            moveSpeed *
            Time.deltaTime;

        // Putar Player mengikuti arah gerakan
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(movement);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
        }
    }
}
```

---

# 13. Memahami Player Controller

Bagian:

```csharp
Input.GetAxisRaw("Horizontal");
```

membaca:

```text
A / D
← / →
```

Sedangkan:

```csharp
Input.GetAxisRaw("Vertical");
```

membaca:

```text
W / S
↑ / ↓
```

Arah gerakan:

```text
                 +Z
                  ↑
                W / ↑
                  │
                  │

      A / ← ─── PLAYER ─── D / →

                  │
                  │
                S / ↓
                  ↓
                 -Z
```

---

# 14. Fungsi normalized

Perhatikan:

```csharp
Vector3 movement =
    new Vector3(
        horizontal,
        0f,
        vertical
    ).normalized;
```

`.normalized` digunakan agar kecepatan diagonal tidak lebih tinggi.

Tanpa normalisasi:

```text
W + D
```

dapat menghasilkan kecepatan lebih tinggi dibanding hanya:

```text
W
```

Dengan `.normalized`, kecepatan tetap konsisten.

---

# 15. Memasang PlayerController

Pilih:

```text
Player
```

Drag:

```text
PlayerController.cs
```

ke Inspector Player.

Atau:

```text
Add Component
→ PlayerController
```

Set:

```text
Move Speed = 5
Rotation Speed = 10
```

---

# 16. Konfigurasi Input Unity

Kode praktikum menggunakan:

```csharp
Input.GetAxisRaw()
```

Jika Unity menampilkan error bahwa project hanya menggunakan Input System baru, buka:

```text
Edit
→ Project Settings
→ Player
```

Cari:

```text
Active Input Handling
```

Pilih:

```text
Both
```

Jika Unity meminta restart Editor, lakukan restart.

---

# 17. Testing Player

Tekan:

```text
Play
```

Klik jendela Game.

Coba:

```text
W
A
S
D
```

Kemudian:

```text
↑
←
↓
→
```

Player harus dapat bergerak.

Player juga harus berputar mengikuti arah pergerakan.

Stop Play Mode setelah pengujian selesai.

---

# 18. Membuat Wall

Pada Hierarchy:

```text
GameObject
→ 3D Object
→ Cube
```

Rename:

```text
Wall
```

Contoh Transform:

```text
Position
X = 0
Y = 1.5
Z = 3

Scale
X = 6
Y = 3
Z = 0.5
```

Set Layer:

```text
Obstacle
```

Wall akan digunakan untuk menghalangi pandangan NPC.

---

# 19. Membuat Material Wall

Buat:

```text
WallMaterial
```

Misalnya gunakan warna abu-abu.

Drag ke object:

```text
Wall
```

---

# 20. Membuat NPC Guard

Pilih:

```text
GameObject
→ 3D Object
→ Capsule
```

Rename:

```text
NPC_Guard
```

Contoh posisi:

```text
X = -6
Y = 1
Z = -6
```

---

# 21. Membuat Material NPC

Buat material:

```text
NPCMaterial
```

Misalnya gunakan warna merah.

Drag ke:

```text
NPC_Guard
```

Sekarang:

```text
Player = biru
NPC = merah
```

---

# 22. Membuat Direction Marker

Capsule sulit menunjukkan arah hadap.

Kita akan membuat penanda arah.

Klik kanan:

```text
NPC_Guard
→ 3D Object
→ Cube
```

Rename:

```text
DirectionMarker
```

Atur local position:

```text
X = 0
Y = 0.5
Z = 0.65
```

Scale:

```text
X = 0.2
Y = 0.2
Z = 0.5
```

DirectionMarker menunjukkan arah:

```csharp
transform.forward
```

NPC.

---

# 23. Instalasi AI Navigation

Buka:

```text
Window
→ Package Manager
```

Cari package:

```text
AI Navigation
```

Install package tersebut.

Package ini menyediakan:

```text
NavMesh Surface
NavMesh Link
NavMesh Modifier
```

yang diperlukan untuk navigasi NPC.

---

# 24. Membuat Navigation Object

Pada Hierarchy:

```text
GameObject
→ Create Empty
```

Rename:

```text
Navigation
```

Pilih:

```text
Add Component
```

Cari:

```text
NavMesh Surface
```

Tambahkan component tersebut.

---

# 25. Bake NavMesh

Pilih object:

```text
Navigation
```

Pada komponen:

```text
NavMesh Surface
```

lakukan Bake NavMesh menggunakan kontrol baking pada komponen tersebut.

Setelah proses selesai, Ground akan menjadi area navigasi NPC.

Pastikan NPC berada di atas area NavMesh.

---

# 26. Menambahkan NavMeshAgent

Pilih:

```text
NPC_Guard
```

Klik:

```text
Add Component
```

Cari:

```text
NavMesh Agent
```

Gunakan parameter awal:

```text
Speed = 3
Angular Speed = 360
Acceleration = 8
Stopping Distance = 0.3
```

NPC sekarang dapat digerakkan menggunakan:

```csharp
agent.SetDestination(...)
```

---

# 27. Membuat Patrol Points

Buat Empty GameObject:

```text
PatrolPoints
```

Kemudian buat empat child Empty GameObject:

```text
PatrolPoints
│
├── Point1
├── Point2
├── Point3
└── Point4
```

Contoh posisi:

```text
Point1
X = -8
Y = 0
Z = -8

Point2
X = 8
Y = 0
Z = -8

Point3
X = 8
Y = 0
Z = 8

Point4
X = -8
Y = 0
Z = 8
```

Flow patrol:

```text
Point1
   ↓
Point2
   ↓
Point3
   ↓
Point4
   ↓
Point1
```

---

# 28. Arsitektur Script NPC

Pada praktikum ini digunakan dua script utama.

```text
NPCSensor.cs
```

bertanggung jawab terhadap:

```text
PERCEPTION
```

Sedangkan:

```text
NPCBrain.cs
```

menangani:

```text
MEMORY
DECISION
ACTION
```

Secara konseptual:

```text
GAME WORLD
    │
    ▼
NPCSensor
    │
    ▼
Perception
    │
    ▼
NPCBrain
    │
    ├── Memory
    ├── Decision
    └── Action
          │
          ▼
     NavMeshAgent
```

---

# 29. Membuat NPCSensor.cs

Masuk:

```text
Assets/Scripts
```

Buat script:

```text
NPCSensor.cs
```

Gunakan kode berikut.

```csharp
using UnityEngine;

public class NPCSensor : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Vision Settings")]
    [SerializeField] private float viewRadius = 8f;

    [Range(0f, 360f)]
    [SerializeField] private float viewAngle = 90f;

    [SerializeField] private LayerMask obstacleMask;

    [Header("Eye Settings")]
    [SerializeField] private float eyeHeight = 1.2f;

    public bool CanSeePlayer { get; private set; }

    public Transform Player => player;

    private void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        CanSeePlayer = false;

        if (player == null)
            return;

        Vector3 directionToPlayer =
            player.position - transform.position;

        float distanceToPlayer =
            directionToPlayer.magnitude;

        // =============================
        // STEP 1 : DISTANCE CHECK
        // =============================

        if (distanceToPlayer > viewRadius)
            return;

        Vector3 normalizedDirection =
            directionToPlayer.normalized;

        // =============================
        // STEP 2 : FIELD OF VIEW
        // =============================

        float angleToPlayer =
            Vector3.Angle(
                transform.forward,
                normalizedDirection
            );

        if (angleToPlayer > viewAngle / 2f)
            return;

        // =============================
        // STEP 3 : LINE OF SIGHT
        // =============================

        Vector3 eyePosition =
            transform.position +
            Vector3.up * eyeHeight;

        Vector3 targetPosition =
            player.position +
            Vector3.up * 0.5f;

        Vector3 rayDirection =
            targetPosition - eyePosition;

        float rayDistance =
            rayDirection.magnitude;

        if (Physics.Raycast(
            eyePosition,
            rayDirection.normalized,
            rayDistance,
            obstacleMask))
        {
            return;
        }

        // Semua pemeriksaan berhasil
        CanSeePlayer = true;
    }

    private void OnDrawGizmosSelected()
    {
        // =============================
        // VIEW RADIUS
        // =============================

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            viewRadius
        );

        // =============================
        // FIELD OF VIEW
        // =============================

        Vector3 leftBoundary =
            DirectionFromAngle(
                -viewAngle / 2f
            );

        Vector3 rightBoundary =
            DirectionFromAngle(
                viewAngle / 2f
            );

        Gizmos.DrawLine(
            transform.position,
            transform.position +
            leftBoundary * viewRadius
        );

        Gizmos.DrawLine(
            transform.position,
            transform.position +
            rightBoundary * viewRadius
        );

        // =============================
        // PLAYER VISIBLE
        // =============================

        if (player != null &&
            CanSeePlayer)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                transform.position +
                Vector3.up * eyeHeight,
                player.position +
                Vector3.up * 0.5f
            );
        }
    }

    private Vector3 DirectionFromAngle(
        float angle
    )
    {
        float finalAngle =
            transform.eulerAngles.y +
            angle;

        return new Vector3(
            Mathf.Sin(
                finalAngle *
                Mathf.Deg2Rad
            ),
            0f,
            Mathf.Cos(
                finalAngle *
                Mathf.Deg2Rad
            )
        );
    }
}
```

---

# 30. Cara Kerja NPC Sensor

NPC melakukan tiga pemeriksaan.

## Pemeriksaan 1 — Radius

```text
Apakah Player cukup dekat?
```

Kode:

```csharp
if (distanceToPlayer > viewRadius)
    return;
```

Contoh:

```text
View Radius = 8 meter
```

Player pada jarak 12 meter:

```text
Tidak terlihat
```

---

# 31. Pemeriksaan 2 — Field of View

NPC kemudian memeriksa sudut.

Contoh:

```text
View Angle = 90°
```

maka area pandangan kira-kira:

```text
             +45°
               /
              /
             /
NPC ───────→
             \
              \
               \
             -45°
```

Player di belakang NPC tidak terlihat meskipun jaraknya dekat.

---

# 32. Pemeriksaan 3 — Line of Sight

Terakhir NPC menggunakan:

```csharp
Physics.Raycast()
```

Situasi:

```text
NPC ───────────── PLAYER
```

Player terlihat.

Tetapi:

```text
NPC ─── WALL ─── PLAYER
```

Raycast mengenai Wall.

Player dianggap tidak terlihat.

---

# 33. Memasang NPCSensor

Pilih:

```text
NPC_Guard
```

Tambahkan:

```text
NPCSensor
```

Pada Inspector:

```text
Player
→ drag GameObject Player

View Radius
→ 8

View Angle
→ 90

Obstacle Mask
→ Obstacle

Eye Height
→ 1.2
```

---

# 34. Testing Sensor

Tekan:

```text
Play
```

Pilih NPC_Guard di Scene View.

Pastikan tombol:

```text
Gizmos
```

aktif.

Anda harus melihat:

```text
Lingkaran kuning
= Detection Radius

Dua garis kuning
= Field of View

Garis merah
= Player terlihat
```

---

# 35. Test Sensor 1

Gerakkan Player ke depan NPC.

Expected:

```text
Player:
dalam radius
+
dalam FOV
+
tidak terhalang
```

hasil:

```text
CanSeePlayer = TRUE
```

---

# 36. Test Sensor 2

Gerakkan Player ke belakang NPC.

Expected:

```text
CanSeePlayer = FALSE
```

meskipun Player masih dekat.

---

# 37. Test Sensor 3

Gerakkan Player ke belakang Wall.

```text
NPC ─── WALL │ PLAYER
```

Expected:

```text
CanSeePlayer = FALSE
```

---

# 38. Membuat NPCBrain.cs

Sekarang buat script:

```text
NPCBrain.cs
```

Gunakan kode berikut.

```csharp
using UnityEngine;
using UnityEngine.AI;

public class NPCBrain : MonoBehaviour
{
    public enum NPCState
    {
        Patrol,
        Chase,
        Search
    }

    [Header("References")]
    [SerializeField]
    private NPCSensor sensor;

    [SerializeField]
    private NavMeshAgent agent;

    [Header("Patrol Settings")]
    [SerializeField]
    private Transform[] patrolPoints;

    [SerializeField]
    private float waypointTolerance = 0.7f;

    [SerializeField]
    private float patrolSpeed = 2f;

    [Header("Chase Settings")]
    [SerializeField]
    private float chaseSpeed = 4f;

    [Header("Search Settings")]
    [SerializeField]
    private float searchDuration = 4f;

    [SerializeField]
    private float searchTolerance = 0.8f;

    [Header("Debug")]
    [SerializeField]
    private NPCState currentState;

    private NPCState previousState;

    private int patrolIndex = 0;

    // =============================
    // MEMORY
    // =============================

    private Vector3 lastKnownPosition;

    private bool hasLastKnownPosition;

    private float searchTimer;

    private void Start()
    {
        currentState = NPCState.Patrol;
        previousState = currentState;

        GoToCurrentPatrolPoint();
    }

    private void Update()
    {
        UpdateMemory();

        MakeDecision();

        ExecuteCurrentState();
    }

    // ======================================
    // MEMORY
    // ======================================

    private void UpdateMemory()
    {
        if (sensor.CanSeePlayer)
        {
            lastKnownPosition =
                sensor.Player.position;

            hasLastKnownPosition = true;
        }
    }

    // ======================================
    // DECISION
    // ======================================

    private void MakeDecision()
    {
        // PRIORITAS 1
        // PLAYER TERLIHAT
        if (sensor.CanSeePlayer)
        {
            ChangeState(
                NPCState.Chase
            );

            return;
        }

        // PRIORITAS 2
        // PLAYER BARU HILANG
        if (currentState ==
                NPCState.Chase &&
            hasLastKnownPosition)
        {
            searchTimer =
                searchDuration;

            ChangeState(
                NPCState.Search
            );

            return;
        }

        // PRIORITAS 3
        // SEARCH SELESAI
        if (currentState ==
                NPCState.Search &&
            searchTimer <= 0f)
        {
            hasLastKnownPosition =
                false;

            ChangeState(
                NPCState.Patrol
            );
        }
    }

    // ======================================
    // ACTION
    // ======================================

    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case NPCState.Patrol:

                Patrol();
                break;

            case NPCState.Chase:

                Chase();
                break;

            case NPCState.Search:

                Search();
                break;
        }
    }

    // ======================================
    // PATROL
    // ======================================

    private void Patrol()
    {
        agent.speed = patrolSpeed;

        if (patrolPoints == null ||
            patrolPoints.Length == 0)
        {
            return;
        }

        if (!agent.pathPending &&
            agent.remainingDistance <=
            waypointTolerance)
        {
            patrolIndex++;

            if (patrolIndex >=
                patrolPoints.Length)
            {
                patrolIndex = 0;
            }

            GoToCurrentPatrolPoint();
        }
    }

    private void GoToCurrentPatrolPoint()
    {
        if (patrolPoints == null ||
            patrolPoints.Length == 0)
        {
            return;
        }

        agent.SetDestination(
            patrolPoints[
                patrolIndex
            ].position
        );
    }

    // ======================================
    // CHASE
    // ======================================

    private void Chase()
    {
        agent.speed =
            chaseSpeed;

        if (sensor.Player == null)
            return;

        agent.SetDestination(
            sensor.Player.position
        );
    }

    // ======================================
    // SEARCH
    // ======================================

    private void Search()
    {
        agent.speed =
            patrolSpeed;

        agent.SetDestination(
            lastKnownPosition
        );

        if (!agent.pathPending &&
            agent.remainingDistance <=
            searchTolerance)
        {
            searchTimer -=
                Time.deltaTime;

            agent.ResetPath();
        }
    }

    // ======================================
    // STATE TRANSITION
    // ======================================

    private void ChangeState(
        NPCState newState
    )
    {
        if (currentState ==
            newState)
        {
            return;
        }

        previousState =
            currentState;

        currentState =
            newState;

        Debug.Log(
            gameObject.name +
            ": " +
            previousState +
            " -> " +
            currentState
        );

        if (currentState ==
            NPCState.Patrol)
        {
            GoToCurrentPatrolPoint();
        }
    }

    // ======================================
    // DEBUG GIZMOS
    // ======================================

    private void OnDrawGizmosSelected()
    {
        switch (currentState)
        {
            case NPCState.Patrol:

                Gizmos.color =
                    Color.green;
                break;

            case NPCState.Chase:

                Gizmos.color =
                    Color.red;
                break;

            case NPCState.Search:

                Gizmos.color =
                    Color.blue;
                break;
        }

        Gizmos.DrawWireSphere(
            transform.position,
            0.8f
        );

        if (hasLastKnownPosition)
        {
            Gizmos.color =
                Color.magenta;

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
```

---

# 39. Memasang NPCBrain

Pilih:

```text
NPC_Guard
```

Tambahkan:

```text
NPCBrain
```

Isi References:

```text
Sensor
→ drag NPC_Guard

Agent
→ drag NPC_Guard
```

Unity akan mengambil komponen:

```text
NPCSensor
NavMeshAgent
```

dari object tersebut.

---

# 40. Memasukkan Patrol Points

Pada:

```text
NPCBrain
→ Patrol Points
```

set:

```text
Size = 4
```

Kemudian:

```text
Element 0 → Point1
Element 1 → Point2
Element 2 → Point3
Element 3 → Point4
```

---

# 41. Parameter NPC

Gunakan nilai awal berikut.

## NPCSensor

```text
View Radius = 8
View Angle = 90
Eye Height = 1.2
```

## NPCBrain

```text
Waypoint Tolerance = 0.7

Patrol Speed = 2

Chase Speed = 4

Search Duration = 4

Search Tolerance = 0.8
```

---

# 42. Memahami State NPC

NPC memiliki tiga state utama.

```text
PATROL

CHASE

SEARCH
```

---

# 43. State PATROL

Dalam PATROL:

```text
NPC berjalan:
Point1
   ↓
Point2
   ↓
Point3
   ↓
Point4
   ↓
Point1
```

Kecepatan:

```text
Patrol Speed
```

---

# 44. State CHASE

NPC masuk CHASE ketika:

```text
sensor.CanSeePlayer == true
```

NPC kemudian menjalankan:

```csharp
agent.SetDestination(
    sensor.Player.position
);
```

Tujuan NPC selalu diperbarui mengikuti posisi Player.

---

# 45. State SEARCH

Ketika Player menghilang:

```text
CHASE
  ↓
SEARCH
```

NPC tidak mengetahui posisi Player terbaru.

Tetapi NPC masih menyimpan:

```text
lastKnownPosition
```

NPC kemudian menuju lokasi tersebut.

---

# 46. Konsep Memory

Perhatikan:

```csharp
lastKnownPosition =
    sensor.Player.position;
```

Kode hanya dijalankan jika Player terlihat.

Contoh:

```text
t0

NPC ───────── PLAYER
```

Memory NPC:

```text
lastKnownPosition = posisi Player
```

Kemudian:

```text
t1

NPC ─── WALL │ PLAYER
```

NPC tidak melihat Player lagi.

Tetapi memory masih menyimpan:

```text
posisi Player saat terakhir terlihat
```

Inilah bentuk sederhana dari:

```text
Belief State
```

sesuai konsep pada materi Pertemuan 2.

---

# 47. Konsep Decision

Decision rule praktikum:

```text
IF Player terlihat
    CHASE

ELSE IF Player baru saja hilang
    SEARCH

ELSE IF Search selesai
    PATROL
```

Flow:

```text
Player terlihat?
       │
   ┌───┴───┐
  YES      NO
   │        │
   ▼        ▼
 CHASE   Baru hilang?
            │
        ┌───┴───┐
       YES      NO
        │        │
        ▼        ▼
      SEARCH   PATROL
```

---

# 48. Konsep Action

Decision menentukan:

```text
Apa yang harus dilakukan?
```

Action menerjemahkan keputusan tersebut ke Unity.

PATROL:

```csharp
agent.SetDestination(
    patrolPoint.position
);
```

CHASE:

```csharp
agent.SetDestination(
    player.position
);
```

SEARCH:

```csharp
agent.SetDestination(
    lastKnownPosition
);
```

Sehingga:

```text
Decision
    ↓
CHASE
    ↓
Action
    ↓
NavMeshAgent.SetDestination()
```

---

# 49. Testing Lengkap

Sekarang tekan:

```text
Play
```

Klik jendela Game.

Gunakan:

```text
WASD
```

atau:

```text
Arrow Key
```

untuk menggerakkan Player.

---

# 50. Test 1 — Patrol

Tempatkan Player jauh dari NPC.

Expected:

```text
NPC State = Patrol
```

NPC berjalan melalui waypoint.

Pastikan:

```text
Point1 → Point2 → Point3 → Point4
```

berjalan dengan benar.

---

# 51. Test 2 — Player di Belakang NPC

Gerakkan Player mendekati NPC dari belakang.

```text
PLAYER → NPC →
```

Walaupun dekat:

```text
Player tidak berada dalam FOV
```

Expected:

```text
NPC tetap PATROL
```

---

# 52. Test 3 — Player Masuk FOV

Gerakkan Player ke depan NPC.

```text
NPC ─────→ PLAYER
```

Jika:

```text
Distance < View Radius

dan

Angle < View Angle / 2

dan

tidak ada obstacle
```

Expected:

```text
PATROL
   ↓
CHASE
```

Console:

```text
NPC_Guard: Patrol -> Chase
```

---

# 53. Test 4 — NPC Mengejar Player

Gunakan:

```text
WASD
```

untuk menjauh.

NPC harus mengikuti Player.

Contoh:

```text
NPC → → → PLAYER
```

Perhatikan:

```text
Patrol Speed = 2

Chase Speed = 4
```

NPC bergerak lebih cepat ketika CHASE.

---

# 54. Test 5 — Player Bersembunyi

Ketika NPC mengejar, arahkan Player ke belakang Wall.

```text
NPC → → WALL │ PLAYER
```

Raycast mengenai Wall.

Expected:

```text
CanSeePlayer = false
```

State:

```text
CHASE
   ↓
SEARCH
```

Console:

```text
NPC_Guard: Chase -> Search
```

---

# 55. Last Known Position

Saat Player hilang, NPC bergerak menuju:

```text
lastKnownPosition
```

Pada Scene View akan terlihat:

```text
Sphere magenta
```

yang menunjukkan posisi terakhir Player terlihat.

---

# 56. Test 6 — Player Tetap Bersembunyi

Biarkan Player tetap berada di belakang Wall.

NPC akan:

```text
menuju lastKnownPosition
        ↓
sampai di lokasi
        ↓
menunggu
        ↓
Search Duration habis
        ↓
PATROL
```

Expected:

```text
SEARCH
   ↓
PATROL
```

Console:

```text
NPC_Guard: Search -> Patrol
```

---

# 57. Test 7 — Player Muncul Saat Search

Ulangi:

```text
CHASE
  ↓
Player bersembunyi
  ↓
SEARCH
```

Tetapi sebelum Search Duration habis, keluarkan Player dari balik Wall.

Jika NPC melihat Player:

```text
SEARCH
   ↓
CHASE
```

Ini menunjukkan perception terbaru mempunyai prioritas tinggi.

---

# 58. Debugging dengan Gizmos

Pilih:

```text
NPC_Guard
```

Pastikan:

```text
Gizmos = ON
```

Interpretasi warna:

```text
Lingkaran kuning
= View Radius

Garis kuning
= batas FOV

Garis merah ke Player
= Player terlihat

Sphere magenta
= Last Known Position
```

Warna state NPC:

```text
Hijau
= PATROL

Merah
= CHASE

Biru
= SEARCH
```

Debug visual sangat penting karena masalah AI sering berasal dari:

```text
FOV salah
Layer salah
Raycast salah
Waypoint salah
NavMesh salah
Memory tidak tersimpan
State tidak berubah
```

---

# 59. Debugging Console

Setiap perpindahan state menghasilkan log.

Contoh:

```text
NPC_Guard: Patrol -> Chase

NPC_Guard: Chase -> Search

NPC_Guard: Search -> Patrol
```

Gunakan:

```text
Window
→ General
→ Console
```

untuk melihat log.

---

# 60. Eksperimen 1 — View Radius

Coba:

```text
View Radius = 4
```

kemudian:

```text
View Radius = 8
```

dan:

```text
View Radius = 15
```

Amati perubahan jarak deteksi.

Pertanyaan:

> Apakah radius yang sangat besar selalu menghasilkan NPC yang lebih baik?

---

# 61. Eksperimen 2 — View Angle

Coba:

```text
45°
90°
180°
360°
```

Bandingkan perilakunya.

Dengan:

```text
360°
```

NPC dapat mendeteksi Player dari hampir seluruh arah.

Diskusikan apakah perilaku tersebut terasa adil bagi pemain.

---

# 62. Eksperimen 3 — Player Speed dan Chase Speed

Gunakan:

```text
Player Move Speed = 5
```

Bandingkan:

| Player | Patrol | Chase | Efek |
|---:|---:|---:|---|
| 5 | 2 | 3 | Player mudah kabur |
| 5 | 2 | 4 | Cukup seimbang |
| 5 | 2 | 5 | Hampir sama |
| 5 | 2 | 7 | NPC sangat agresif |

Mahasiswa harus memahami bahwa Game AI tidak hanya mengenai algoritma, tetapi juga:

```text
parameter tuning
```

---

# 63. Eksperimen 4 — Search Duration

Coba:

```text
Search Duration = 1
```

kemudian:

```text
4
```

dan:

```text
10
```

Amati perbedaannya.

Search Duration terlalu kecil:

```text
NPC terlalu cepat lupa
```

Search Duration terlalu besar:

```text
NPC terlalu lama mencari
```

---

# 64. Eksperimen 5 — Obstacle

Tambahkan beberapa Cube baru.

Set semuanya:

```text
Layer = Obstacle
```

Susun menjadi lorong sederhana.

Contoh:

```text
┌───────────────┐
│               │
│   WALL        │
│        WALL   │
│               │
│ NPC    PLAYER │
└───────────────┘
```

Amati bagaimana obstacle memengaruhi:

```text
Line of Sight
```

dan:

```text
NavMesh navigation
```

---

# 65. Checklist Pengujian

Mahasiswa wajib menguji minimal kondisi berikut:

| No | Kondisi | Expected |
|---|---|---|
| 1 | Player jauh | PATROL |
| 2 | Player dekat tetapi di belakang NPC | PATROL |
| 3 | Player di depan NPC | CHASE |
| 4 | Player di balik Wall sebelum terdeteksi | PATROL |
| 5 | Player hilang ketika sedang dikejar | SEARCH |
| 6 | Player muncul kembali saat SEARCH | CHASE |
| 7 | Player tidak ditemukan | PATROL |

---

# 66. Troubleshooting — Player Tidak Bergerak

Pastikan:

```text
PlayerController
```

sudah terpasang pada:

```text
Player
```

Pastikan juga jendela:

```text
Game
```

aktif ketika menekan keyboard.

Jika terdapat error Input:

```text
Edit
→ Project Settings
→ Player
→ Active Input Handling
→ Both
```

---

# 67. Troubleshooting — NPC Tidak Bergerak

Periksa:

```text
NavMeshAgent
```

sudah terpasang.

Pastikan:

```text
NavMesh sudah di-Bake
```

dan NPC berada tepat di atas NavMesh.

---

# 68. Troubleshooting — NPC Tidak Patrol

Periksa:

```text
Patrol Points
Size = 4
```

Pastikan:

```text
Element 0 = Point1
Element 1 = Point2
Element 2 = Point3
Element 3 = Point4
```

Pastikan waypoint berada di atas NavMesh.

---

# 69. Troubleshooting — NPC Tidak Melihat Player

Periksa field:

```text
NPCSensor
→ Player
```

Harus berisi:

```text
Player
```

Kemudian periksa:

```text
View Radius
View Angle
```

Coba terlebih dahulu:

```text
View Radius = 10
View Angle = 120
```

untuk debugging.

---

# 70. Troubleshooting — NPC Melihat Melalui Wall

Pastikan:

```text
Wall
→ Layer = Obstacle
```

dan:

```text
NPCSensor
→ Obstacle Mask
→ Obstacle
```

Pastikan Wall mempunyai:

```text
Collider
```

Cube otomatis memiliki Box Collider.

---

# 71. Troubleshooting — NPC Langsung Kembali Patrol

Periksa:

```text
Search Duration
```

Pastikan bukan:

```text
0
```

Gunakan:

```text
4
```

untuk pengujian awal.

---

# 72. Troubleshooting — NPC Tidak Mengejar Player

Periksa:

```text
CanSeePlayer
```

melalui Inspector saat Play Mode.

Jika:

```text
false
```

berarti masalah berada pada Sensor.

Jika:

```text
true
```

tetapi NPC tidak bergerak, periksa:

```text
NavMeshAgent
NavMesh
NPCBrain
```

---

# 73. Troubleshooting — NPC Bergerak Aneh

Pastikan NPC tidak memiliki:

```text
Rigidbody
```

yang menggunakan physics bebas bersamaan dengan NavMeshAgent.

Untuk praktikum ini cukup gunakan:

```text
Capsule Collider
+
NavMeshAgent
```

---

# 74. Struktur Script Akhir

Folder Scripts:

```text
Assets
└── Scripts
    ├── PlayerController.cs
    ├── NPCSensor.cs
    └── NPCBrain.cs
```

---

# 75. Struktur NPC Akhir

```text
NPC_Guard
│
├── Transform
├── Mesh Renderer
├── Capsule Collider
├── NavMesh Agent
├── NPC Sensor
├── NPC Brain
│
└── DirectionMarker
```

---

# 76. Struktur Player Akhir

```text
Player
│
├── Transform
├── Mesh Renderer
├── Capsule Collider
└── Player Controller
```

---

# 77. Struktur Scene Akhir

```text
Praktikum02_NPCGuard
│
├── Main Camera
├── Directional Light
│
├── Ground
│
├── Navigation
│   └── NavMesh Surface
│
├── Player
│   └── PlayerController
│
├── NPC_Guard
│   ├── NavMeshAgent
│   ├── NPCSensor
│   ├── NPCBrain
│   └── DirectionMarker
│
├── Wall
│
└── PatrolPoints
    ├── Point1
    ├── Point2
    ├── Point3
    └── Point4
```

---

# 78. Hubungan dengan Game Agent Architecture

Setelah praktikum selesai, identifikasi komponen berikut.

## Environment

```text
Ground
Wall
Player
Waypoint
```

## Perception

```text
Distance
Field of View
Raycast
```

## Memory

```text
lastKnownPosition

hasLastKnownPosition

searchTimer
```

## Decision

```text
PATROL

CHASE

SEARCH
```

## Action

```text
NavMeshAgent.SetDestination()
```

Keseluruhan:

```text
Player bergerak
      ↓
Environment berubah
      ↓
NPC melakukan sensing
      ↓
Memory diperbarui
      ↓
Decision dipilih
      ↓
NPC melakukan Action
      ↓
Environment kembali berubah
```

Ini merupakan implementasi konkret dari arsitektur agen yang dibahas dalam materi Pertemuan 2.

---

# 79. Diagram Final NPC Guard

```text
                 GAME WORLD
                     │
                     ▼
              ┌─────────────┐
              │ NPC SENSOR  │
              │             │
              │ Distance    │
              │ FOV         │
              │ Raycast     │
              └──────┬──────┘
                     │
                     ▼
              Can See Player?
                     │
            ┌────────┴────────┐
           YES                NO
            │                  │
            ▼                  ▼
      Save Position      Recently Seen?
            │                  │
            ▼            ┌─────┴─────┐
          CHASE          YES          NO
                           │           │
                           ▼           ▼
                        SEARCH      PATROL
                           │
                           ▼
                  Player Found Again?
                       │          │
                      YES        NO
                       │          │
                       ▼          ▼
                     CHASE    Search Timeout
                                   │
                                   ▼
                                PATROL
```

---

# 80. Pertanyaan Analisis

Jawab pertanyaan berikut.

### Pertanyaan 1

Mengapa NPC tidak cukup hanya menggunakan:

```text
distance
```

untuk mendeteksi Player?

---

### Pertanyaan 2

Apa fungsi:

```text
Field of View
```

dalam perception?

---

### Pertanyaan 3

Apa fungsi:

```text
Raycast
```

dalam praktikum ini?

---

### Pertanyaan 4

Mengapa Player yang berada di belakang Wall tidak boleh dianggap terlihat?

---

### Pertanyaan 5

Apa perbedaan antara:

```text
Perception
```

dan:

```text
Memory
```

?

---

### Pertanyaan 6

Jelaskan fungsi:

```text
lastKnownPosition
```

---

### Pertanyaan 7

Mengapa NPC tidak langsung kembali PATROL setelah kehilangan Player?

---

### Pertanyaan 8

Apa yang terjadi jika:

```text
Search Duration = 0
```

?

---

### Pertanyaan 9

Apa yang terjadi jika:

```text
View Angle = 360
```

?

---

### Pertanyaan 10

Mengapa parameter seperti:

```text
viewRadius
viewAngle
patrolSpeed
chaseSpeed
searchDuration
```

lebih baik dibuat:

```csharp
[SerializeField]
```

daripada ditulis permanen di dalam kode?

---

### Pertanyaan 11

Jelaskan bagaimana alur:

```text
Perception
→ Memory
→ Decision
→ Action
```

terjadi pada NPC Guard.

---

### Pertanyaan 12

Jika NPC berfungsi tetapi mahasiswa tidak dapat menjelaskan mengapa NPC masuk state tertentu, apakah implementasinya dapat dianggap sebagai AI yang dirancang dengan baik? Jelaskan.

---

# 81. Challenge 1 — NPC Berhenti di Waypoint

Modifikasi PATROL:

```text
berjalan
   ↓
sampai waypoint
   ↓
diam 2 detik
   ↓
lanjut waypoint berikutnya
```

Tujuannya memahami tambahan behavior sederhana.

---

# 82. Challenge 2 — Search Rotation

Saat NPC sampai di:

```text
lastKnownPosition
```

buat NPC:

```text
lihat kiri
   ↓
lihat kanan
   ↓
menunggu
   ↓
kembali patrol
```

NPC akan tampak lebih seperti sedang mencari.

---

# 83. Challenge 3 — Alert Indicator

Tambahkan object:

```text
!
```

di atas kepala NPC ketika:

```text
CHASE
```

dan:

```text
?
```

ketika:

```text
SEARCH
```

---

# 84. Challenge 4 — Hearing

Tambahkan konsep:

```text
Player menghasilkan suara
        ↓
NPC mendengar
        ↓
NPC menuju sumber suara
```

Ini memperluas perception dari:

```text
Visual Sensor
```

menjadi:

```text
Visual + Auditory
```

---

# 85. Challenge 5 — Multiple Guards

Duplikat:

```text
NPC_Guard
```

menjadi:

```text
NPC_Guard_A

NPC_Guard_B

NPC_Guard_C
```

Berikan waypoint berbeda.

Amati apakah setiap agent dapat mengambil keputusan secara mandiri.

---

# 86. Checklist Praktikum

Sebelum mengumpulkan project, pastikan semua berikut berhasil.

## Player

- [ ] Player dapat bergerak dengan W.
- [ ] Player dapat bergerak dengan A.
- [ ] Player dapat bergerak dengan S.
- [ ] Player dapat bergerak dengan D.
- [ ] Player dapat bergerak dengan Arrow Key.
- [ ] Player berputar mengikuti arah gerak.
- [ ] Move Speed dapat diubah di Inspector.

## Navigation

- [ ] AI Navigation sudah terpasang.
- [ ] Ground memiliki NavMesh.
- [ ] NPC memiliki NavMeshAgent.
- [ ] NPC dapat bergerak di arena.

## Patrol

- [ ] Terdapat minimal empat waypoint.
- [ ] NPC bergerak antar-waypoint.
- [ ] Setelah waypoint terakhir NPC kembali ke waypoint pertama.

## Perception

- [ ] NPC memiliki View Radius.
- [ ] NPC memiliki Field of View.
- [ ] NPC menggunakan Raycast.
- [ ] Player di belakang NPC tidak terdeteksi.
- [ ] Wall menghalangi penglihatan NPC.

## Memory

- [ ] NPC menyimpan lastKnownPosition.
- [ ] Last Known Position terlihat di Gizmos.
- [ ] Memory diperbarui ketika Player terlihat.

## Decision

- [ ] NPC mempunyai state PATROL.
- [ ] NPC mempunyai state CHASE.
- [ ] NPC mempunyai state SEARCH.
- [ ] PATROL berubah menjadi CHASE.
- [ ] CHASE berubah menjadi SEARCH.
- [ ] SEARCH dapat berubah kembali menjadi CHASE.
- [ ] SEARCH berubah menjadi PATROL setelah timeout.

## Debugging

- [ ] Detection Radius terlihat.
- [ ] FOV terlihat.
- [ ] Garis ke Player terlihat ketika terdeteksi.
- [ ] Last Known Position terlihat.
- [ ] Transisi state muncul di Console.

---

# 87. Skenario Demo Akhir

Video demo sebaiknya memperlihatkan seluruh siklus berikut.

## Adegan 1

NPC berjalan patroli.

```text
PATROL
```

---

## Adegan 2

Player digerakkan menggunakan:

```text
WASD
```

masuk ke FOV NPC.

```text
PATROL
   ↓
CHASE
```

---

## Adegan 3

NPC mengejar Player.

```text
NPC → → → PLAYER
```

---

## Adegan 4

Player bersembunyi di balik Wall.

```text
NPC → WALL │ PLAYER
```

State:

```text
CHASE
   ↓
SEARCH
```

---

## Adegan 5

NPC menuju:

```text
lastKnownPosition
```

---

## Adegan 6

Player tetap bersembunyi.

Setelah:

```text
Search Duration
```

state menjadi:

```text
SEARCH
   ↓
PATROL
```

---

## Adegan 7

Ulangi demonstrasi, tetapi kali ini Player keluar dari Wall ketika SEARCH.

Expected:

```text
SEARCH
   ↓
CHASE
```

---

# 88. Deliverable Praktikum

Mahasiswa mengumpulkan:

### A. Unity Project

Project Unity lengkap atau repository Git.

---

### B. Screenshot Scene

Harus menunjukkan:

```text
Player
NPC
Wall
Waypoint
Navigation
```

---

### C. Screenshot Gizmos

Minimal menunjukkan:

```text
View Radius

Field of View

Line of Sight

Last Known Position
```

---

### D. Screenshot State

Minimal:

```text
PATROL

CHASE

SEARCH
```

---

### E. Video Demo

Durasi yang disarankan:

```text
1–3 menit
```

Harus menunjukkan:

```text
PATROL
   ↓
CHASE
   ↓
SEARCH
   ↓
PATROL
```

serta satu contoh:

```text
SEARCH
   ↓
CHASE
```

---

### F. Laporan Singkat

Struktur:

```text
1. Tujuan Praktikum

2. Desain Scene

3. Arsitektur NPC

4. Implementasi Player Controller

5. Implementasi Sensor

6. Implementasi Memory

7. Implementasi Decision

8. Implementasi Action

9. Hasil Pengujian

10. Eksperimen Parameter

11. Analisis

12. Kesimpulan
```

---

# 89. Rubrik Penilaian

| Komponen | Bobot |
|---|---:|
| Sensor bekerja | 25% |
| Memory & State | 20% |
| Decision Rule | 20% |
| Debug Gizmos | 15% |
| Parameter Tuning | 10% |
| Penjelasan / Analisis | 10% |
| **Total** | **100%** |

---

# 90. Kriteria Keberhasilan Praktikum

NPC tidak cukup hanya:

> bergerak dan mengejar Player.

Mahasiswa harus mampu menjelaskan:

```text
Apa yang dilihat NPC?

Bagaimana NPC mengetahui Player ada?

Bagaimana NPC mengetahui Player terhalang?

Informasi apa yang disimpan?

Mengapa NPC memilih CHASE?

Mengapa NPC memilih SEARCH?

Mengapa NPC kembali PATROL?
```

Dengan demikian fokus praktikum bukan hanya coding, tetapi memahami bagaimana sebuah **Game Agent** bekerja.

---

# 91. Ringkasan

Pada praktikum ini kita telah membangun:

```text
PLAYER
WASD / Arrow Key
       │
       ▼
   GAME WORLD
       │
       ▼
   PERCEPTION
       │
       ├── Radius
       ├── FOV
       └── Raycast
       │
       ▼
     MEMORY
       │
       └── lastKnownPosition
       │
       ▼
    DECISION
       │
       ├── PATROL
       ├── CHASE
       └── SEARCH
       │
       ▼
     ACTION
       │
       └── NavMeshAgent
       │
       ▼
   GAME WORLD
```

Perilaku akhir:

```text
NPC Patrol
    ↓
Player terlihat
    ↓
NPC Chase
    ↓
Player bersembunyi
    ↓
NPC kehilangan target
    ↓
NPC menuju posisi terakhir
    ↓
NPC Search
    ↓
Player tidak ditemukan
    ↓
NPC Patrol kembali
```

atau:

```text
SEARCH
   ↓
Player muncul kembali
   ↓
CHASE
```

Praktikum ini menjadi dasar untuk pengembangan AI berikutnya seperti:

```text
Finite State Machine
Steering Behavior
Behavior Tree
Utility AI
Hearing System
Combat AI
Group AI
```

---

# SELESAI

## Praktikum 2 — NPC Guard: Sensor + Memory + Decision

**Game Cerdas – S1 Teknik Informatika**

**Unity 6 + C#**

**Konsep utama:**

```text
PERCEPTION → MEMORY → DECISION → ACTION
```