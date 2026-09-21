# MODUL PRAKTIKUM 3  
# Autonomous Steering Agent — Movement AI & Steering Behaviors

**Mata Kuliah:** Game Cerdas  
**Program Studi:** S1 Teknik Informatika  
**Semester:** 7  
**Game Engine:** Unity 6  
**Bahasa Pemrograman:** C#  
**Materi:** Movement AI & Steering Behaviors

---

# 1. Tujuan Praktikum

Pada praktikum ini mahasiswa akan membuat sebuah **NPC Autonomous Steering Agent** yang mampu bergerak secara mandiri di dalam lingkungan game.

NPC akan memiliki kemampuan:

1. Bergerak menuju target.
2. Melambat ketika mendekati target.
3. Berhenti pada jarak tertentu dari target.
4. Menghadap ke arah gerak.
5. Bergerak secara acak menggunakan **Wander** ketika tidak memiliki target.
6. Mendeteksi obstacle menggunakan sensor fisika.
7. Menghindari obstacle menggunakan **Obstacle Avoidance**.
8. Menggabungkan beberapa steering behavior.
9. Melakukan tuning parameter AI melalui Inspector.
10. Melakukan debugging menggunakan Gizmos.

Behavior utama yang digunakan:

```text
                    TARGET ADA?
                       │
             ┌─────────┴─────────┐
             │                   │
            YA                 TIDAK
             │                   │
          ARRIVE               WANDER
             │                   │
             └─────────┬─────────┘
                       │
              OBSTACLE AVOIDANCE
                       │
                       ▼
                DESIRED VELOCITY
                       │
                       ▼
                 ACCELERATION
                       │
                       ▼
                    VELOCITY
                       │
                       ▼
                    MOVEMENT
                       │
                       ▼
                   ROTATION
```

---

# 2. Hubungan dengan Praktikum Sebelumnya

Pada praktikum sebelumnya fokus AI berada pada:

```text
Perception
    ↓
Memory
    ↓
Decision
```

Pada Praktikum 3 fokus berpindah ke:

```text
Decision
    ↓
Movement / Action
```

Contohnya:

```text
Decision:
"Kejar Player"

       ↓

Movement AI:
"Bagaimana NPC bergerak menuju Player?"
```

Movement AI tidak menentukan **apa keputusan NPC**, tetapi menentukan **bagaimana keputusan tersebut diterjemahkan menjadi gerakan**.

---

# 3. Konsep yang Digunakan

Praktikum ini menggunakan beberapa konsep utama.

## 3.1 Position

Posisi GameObject di world space.

```csharp
transform.position
```

Contoh:

```csharp
Vector3 npcPosition = transform.position;
```

---

## 3.2 Direction

Direction adalah arah dari suatu posisi menuju posisi lainnya.

```text
Direction = Target Position - Agent Position
```

Dalam Unity:

```csharp
Vector3 direction =
    target.position - transform.position;
```

---

## 3.3 Magnitude

Magnitude menunjukkan panjang sebuah vector.

```csharp
float distance = direction.magnitude;
```

Dalam konteks steering:

```text
Magnitude direction
        =
jarak agent ke target
```

---

## 3.4 Normalized Vector

Jika:

```csharp
Vector3 direction =
    target.position - transform.position;
```

besar vector tersebut masih dipengaruhi jarak.

Agar hanya mendapatkan arah:

```csharp
direction.Normalize();
```

atau:

```csharp
direction = direction.normalized;
```

Vector hasil normalisasi memiliki panjang:

```text
1
```

Sehingga dapat digunakan sebagai:

```csharp
velocity = direction * speed;
```

---

# 4. Model Movement yang Digunakan

Materi membedakan movement menjadi:

```text
Kinematic Movement
Dynamic Movement
```

Untuk praktikum ini digunakan pendekatan **semi-dynamic steering**.

Agent memiliki velocity internal:

```csharp
private Vector3 velocity;
```

Kemudian velocity bergerak menuju desired velocity secara bertahap:

```csharp
velocity = Vector3.MoveTowards(
    velocity,
    desiredVelocity,
    maxAcceleration * Time.deltaTime
);
```

Setelah itu:

```csharp
transform.position +=
    velocity * Time.deltaTime;
```

Dengan demikian:

```text
Desired Velocity
       ↓
Acceleration
       ↓
Velocity
       ↓
Position
```

Pendekatan ini masih cukup sederhana untuk pembelajaran tetapi menghasilkan gerakan yang lebih halus daripada mengubah posisi dengan kecepatan maksimum secara langsung.

---

# 5. Hasil Akhir Praktikum

Scene akhir kurang lebih memiliki struktur:

```text
Praktikum03
│
├── Main Camera
├── Directional Light
├── Ground
│
├── Player
│
├── NPC
│   ├── SteeringAgent
│   └── SteeringSensor
│
└── Obstacles
    ├── Obstacle01
    ├── Obstacle02
    ├── Obstacle03
    └── Obstacle04
```

NPC akan:

```text
Target tersedia
        ↓
Menuju Player
        ↓
Melambat ketika mendekat
        ↓
Berhenti dekat Player
```

Jika target tidak digunakan:

```text
Tidak ada Target
        ↓
Wander
        ↓
Berjalan secara autonomous
```

Pada kedua kondisi tersebut:

```text
Obstacle terdeteksi
        ↓
Obstacle Avoidance
        ↓
NPC membelok menghindari obstacle
```

---

# 6. Membuat Project

Buka Unity Hub.

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

Nama project yang disarankan:

```text
GameCerdas_Praktikum03_Steering
```

Kemudian klik:

```text
Create Project
```

---

# 7. Membuat Struktur Folder

Pada jendela **Project**, buka:

```text
Assets
```

Buat folder:

```text
Assets/
├── Scenes/
├── Scripts/
├── Materials/
└── Prefabs/
```

Tujuannya agar asset project lebih terorganisasi.

---

# 8. Membuat Scene Praktikum

Simpan scene menggunakan:

```text
File
→ Save As
```

Simpan di:

```text
Assets/Scenes/
```

dengan nama:

```text
Praktikum03_Steering
```

---

# 9. Membuat Ground

Pilih:

```text
GameObject
→ 3D Object
→ Plane
```

Rename menjadi:

```text
Ground
```

Atur Transform:

```text
Position
X = 0
Y = 0
Z = 0
```

Scale:

```text
X = 3
Y = 1
Z = 3
```

Ground akan menjadi area pergerakan NPC dan Player.

---

# 10. Membuat Player

Pilih:

```text
GameObject
→ 3D Object
→ Capsule
```

Rename:

```text
Player
```

Atur posisi awal misalnya:

```text
X = 6
Y = 1
Z = 5
```

Capsule secara default telah memiliki:

```text
Capsule Collider
```

---

# 11. Membuat Layer Player

Pilih GameObject:

```text
Player
```

Pada Inspector:

```text
Layer
→ Add Layer...
```

Tambahkan:

```text
Player
```

Kembali ke GameObject Player.

Set:

```text
Layer = Player
```

---

# 12. Membuat Player Controller

Agar Player dapat digerakkan menggunakan:

```text
WASD
```

atau:

```text
Arrow Keys
```

kita membuat controller sederhana.

Tambahkan komponen:

```text
Character Controller
```

ke Player melalui:

```text
Inspector
→ Add Component
→ Character Controller
```

---

# 13. Script SimplePlayerController.cs

Masuk ke:

```text
Assets/Scripts
```

Buat script:

```text
SimplePlayerController.cs
```

Isi dengan:

```csharp
using UnityEngine;

public class SimplePlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float turnSpeed = 10f;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction =
            new Vector3(horizontal, 0f, vertical);

        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        controller.Move(
            direction * moveSpeed * Time.deltaTime
        );

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    turnSpeed * Time.deltaTime
                );
        }
    }
}
```

Pasang script tersebut ke:

```text
Player
```

---

# 14. Penjelasan SimplePlayerController

Bagian:

```csharp
Input.GetAxisRaw("Horizontal")
```

membaca input horizontal.

Secara default:

```text
A / Left Arrow  → -1
D / Right Arrow → +1
```

Sedangkan:

```csharp
Input.GetAxisRaw("Vertical")
```

membaca:

```text
S / Down Arrow → -1
W / Up Arrow   → +1
```

Kemudian dibuat direction:

```csharp
Vector3 direction =
    new Vector3(horizontal, 0f, vertical);
```

Nilai Y dibuat:

```text
0
```

karena Player hanya bergerak pada bidang XZ.

---

# 15. Catatan Input System Unity 6

Jika muncul error yang berhubungan dengan:

```text
Input.GetAxisRaw
```

periksa:

```text
Edit
→ Project Settings
→ Player
```

Cari:

```text
Active Input Handling
```

Gunakan:

```text
Both
```

atau:

```text
Input Manager (Old)
```

untuk praktikum ini.

Hal ini dilakukan agar script pembelajaran tetap sederhana.

---

# 16. Membuat NPC

Buat Capsule baru:

```text
GameObject
→ 3D Object
→ Capsule
```

Rename:

```text
NPC
```

Posisi awal misalnya:

```text
X = -6
Y = 1
Z = -5
```

NPC ini akan menjadi:

```text
Autonomous Steering Agent
```

---

# 17. Memberikan Material Berbeda

Agar mudah membedakan Player dan NPC, buat dua material.

Contoh:

```text
Materials/
├── PlayerMaterial
└── NPCMaterial
```

Berikan warna yang berbeda sesuai kebutuhan.

Hal ini tidak memengaruhi algoritma AI tetapi membantu visualisasi.

---

# 18. Membuat Obstacle

Buat beberapa Cube.

```text
GameObject
→ 3D Object
→ Cube
```

Rename:

```text
Obstacle01
```

Contoh Transform:

```text
Position:
X = 0
Y = 1
Z = 0

Scale:
X = 3
Y = 2
Z = 1
```

Duplicate beberapa kali menggunakan:

```text
Ctrl + D
```

Buat konfigurasi misalnya:

```text
Obstacle01
Obstacle02
Obstacle03
Obstacle04
```

Susun obstacle sehingga NPC harus membelok untuk menuju Player.

---

# 19. Membuat Layer Obstacle

Buka:

```text
Layer
→ Add Layer
```

Tambahkan:

```text
Obstacle
```

Kemudian set setiap obstacle menjadi:

```text
Layer = Obstacle
```

Jika muncul:

```text
Change children too?
```

pilih sesuai struktur objek.

---

# 20. Mengapa Menggunakan Layer?

NPC menggunakan sensor:

```text
Physics.SphereCast()
```

Tanpa LayerMask, sensor berpotensi mendeteksi:

```text
Ground
Player
NPC
Obstacle
objek lain
```

Padahal yang ingin dideteksi hanya:

```text
Obstacle
```

Karena itu digunakan:

```csharp
LayerMask obstacleMask;
```

Alur:

```text
SphereCast
    ↓
LayerMask
    ↓
Hanya mendeteksi Obstacle
```

---

# 21. Membuat SteeringSensor.cs

Buat:

```text
Assets/Scripts/SteeringSensor.cs
```

Isi:

```csharp
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

    private bool obstacleDetected;
    private RaycastHit lastHit;

    public bool ObstacleDetected => obstacleDetected;

    public RaycastHit LastHit => lastHit;

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

            Vector3 avoidDirection =
                Vector3.ProjectOnPlane(
                    lastHit.normal,
                    Vector3.up
                );

            avoidDirection.y = 0f;

            if (avoidDirection.sqrMagnitude > 0.001f)
            {
                avoidDirection.Normalize();
            }

            avoidDirection +=
                movementDirection * forwardBias;

            return avoidDirection.normalized;
        }

        return Vector3.zero;
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
```

Pasang script ke:

```text
NPC
```

---

# 22. Memahami SphereCast

Pada Raycast:

```text
NPC ● ─────────────→
```

sensor hanya berupa garis.

Pada SphereCast:

```text
NPC ● (========)→
```

sensor memiliki radius.

NPC mempunyai ukuran fisik sehingga **SphereCast lebih representatif** daripada Raycast tunggal.

Sintaks utama:

```csharp
Physics.SphereCast(
    origin,
    radius,
    direction,
    out hit,
    distance,
    layerMask
);
```

---

# 23. RaycastHit

Jika obstacle ditemukan, Unity mengisi:

```csharp
RaycastHit lastHit;
```

Informasi penting di dalam `RaycastHit`:

```text
hit.point
hit.normal
hit.distance
hit.collider
```

Dalam praktikum ini yang penting adalah:

```csharp
hit.normal
```

---

# 24. Apa Itu Surface Normal?

Misalkan NPC menuju tembok:

```text
NPC ─────→ █ WALL
           ← normal
```

`hit.normal` adalah vector yang mengarah keluar dari permukaan obstacle.

Vector tersebut dapat dimanfaatkan sebagai dasar arah menghindar.

---

# 25. Vector3.ProjectOnPlane

Kode:

```csharp
Vector3.ProjectOnPlane(
    lastHit.normal,
    Vector3.up
);
```

digunakan untuk membuang komponen vertikal dari surface normal.

Dengan demikian NPC tetap bergerak pada:

```text
bidang XZ
```

dan tidak mencoba bergerak ke atas atau ke bawah.

---

# 26. Membuat SteeringAgent.cs

Sekarang kita membuat komponen utama AI.

Buat:

```text
Assets/Scripts/SteeringAgent.cs
```

Isi dengan kode berikut.

```csharp
using UnityEngine;

public class SteeringAgent : MonoBehaviour
{
    [Header("Target")]

    [SerializeField]
    private Transform target;

    [SerializeField]
    private bool useTarget = true;

    [Header("Movement")]

    [SerializeField]
    private float maxSpeed = 4f;

    [SerializeField]
    private float maxAcceleration = 8f;

    [SerializeField]
    private float turnSpeed = 8f;

    [Header("Arrive")]

    [SerializeField]
    private float slowRadius = 4f;

    [SerializeField]
    private float stopRadius = 1.5f;

    [Header("Wander")]

    [SerializeField]
    private float wanderSpeed = 2.5f;

    [SerializeField]
    private float wanderChangeInterval = 1.5f;

    [SerializeField]
    private float wanderAngleChange = 45f;

    [Header("Obstacle Avoidance")]

    [SerializeField]
    private SteeringSensor sensor;

    [SerializeField]
    private float avoidanceWeight = 2.5f;

    private Vector3 velocity;

    private Vector3 wanderDirection;

    private float wanderTimer;

    public Vector3 Velocity => velocity;

    private void Start()
    {
        wanderDirection = transform.forward;
        wanderTimer = wanderChangeInterval;
    }

    private void Update()
    {
        Vector3 desiredVelocity;

        if (useTarget && target != null)
        {
            desiredVelocity = CalculateArrive();
        }
        else
        {
            desiredVelocity = CalculateWander();
        }

        desiredVelocity =
            ApplyObstacleAvoidance(desiredVelocity);

        velocity =
            Vector3.MoveTowards(
                velocity,
                desiredVelocity,
                maxAcceleration * Time.deltaTime
            );

        velocity =
            Vector3.ClampMagnitude(
                velocity,
                maxSpeed
            );

        ApplyMovement();

        UpdateRotation();
    }

    private Vector3 CalculateArrive()
    {
        Vector3 toTarget =
            target.position - transform.position;

        toTarget.y = 0f;

        float distance = toTarget.magnitude;

        if (distance <= stopRadius)
        {
            return Vector3.zero;
        }

        float desiredSpeed = maxSpeed;

        if (distance < slowRadius)
        {
            float range =
                Mathf.Max(
                    slowRadius - stopRadius,
                    0.001f
                );

            float normalizedDistance =
                (distance - stopRadius) / range;

            desiredSpeed =
                maxSpeed *
                Mathf.Clamp01(normalizedDistance);
        }

        return toTarget.normalized * desiredSpeed;
    }

    private Vector3 CalculateWander()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            float randomAngle =
                Random.Range(
                    -wanderAngleChange,
                    wanderAngleChange
                );

            wanderDirection =
                Quaternion.Euler(
                    0f,
                    randomAngle,
                    0f
                ) * transform.forward;

            wanderDirection.y = 0f;
            wanderDirection.Normalize();

            wanderTimer = wanderChangeInterval;
        }

        return wanderDirection * wanderSpeed;
    }

    private Vector3 ApplyObstacleAvoidance(
        Vector3 desiredVelocity)
    {
        if (sensor == null)
        {
            return desiredVelocity;
        }

        Vector3 checkDirection =
            desiredVelocity.sqrMagnitude > 0.001f
                ? desiredVelocity.normalized
                : transform.forward;

        Vector3 avoidanceDirection =
            sensor.GetAvoidanceDirection(
                checkDirection
            );

        if (avoidanceDirection.sqrMagnitude > 0.001f)
        {
            Vector3 combinedDirection =
                checkDirection +
                avoidanceDirection *
                avoidanceWeight;

            combinedDirection.y = 0f;

            if (combinedDirection.sqrMagnitude > 0.001f)
            {
                combinedDirection.Normalize();
            }

            float desiredSpeed =
                Mathf.Max(
                    desiredVelocity.magnitude,
                    wanderSpeed
                );

            return combinedDirection * desiredSpeed;
        }

        return desiredVelocity;
    }

    private void ApplyMovement()
    {
        transform.position +=
            velocity * Time.deltaTime;
    }

    private void UpdateRotation()
    {
        Vector3 horizontalVelocity = velocity;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(
                horizontalVelocity.normalized
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            stopRadius
        );

        Gizmos.DrawWireSphere(
            transform.position,
            slowRadius
        );

        if (target != null)
        {
            Gizmos.DrawLine(
                transform.position,
                target.position
            );
        }
    }
}
```

---

# 27. Memasang SteeringAgent

Pilih:

```text
NPC
```

Tambahkan:

```text
SteeringAgent
```

Sekarang Inspector NPC memiliki parameter kurang lebih:

```text
Target
├── Target
└── Use Target

Movement
├── Max Speed
├── Max Acceleration
└── Turn Speed

Arrive
├── Slow Radius
└── Stop Radius

Wander
├── Wander Speed
├── Wander Change Interval
└── Wander Angle Change

Obstacle Avoidance
├── Sensor
└── Avoidance Weight
```

---

# 28. Menghubungkan Target

Drag:

```text
Player
```

dari Hierarchy ke field:

```text
Target
```

pada komponen SteeringAgent.

Sehingga:

```text
NPC SteeringAgent
        │
        └── Target → Player
```

Pastikan:

```text
Use Target = true
```

---

# 29. Menghubungkan SteeringSensor

Pada field:

```text
Sensor
```

drag komponen:

```text
NPC
```

atau drag langsung komponen SteeringSensor dari Inspector.

Tujuannya agar:

```text
SteeringAgent
       ↓
SteeringSensor
       ↓
SphereCast
       ↓
Obstacle
```

---

# 30. Mengatur Obstacle Mask

Pada komponen:

```text
SteeringSensor
```

set:

```text
Obstacle Mask = Obstacle
```

Jangan gunakan:

```text
Everything
```

karena sensor berpotensi mendeteksi objek yang tidak diperlukan.

---

# 31. Parameter Awal yang Disarankan

## SteeringAgent

```text
Use Target             = true

Max Speed              = 4
Max Acceleration       = 8
Turn Speed             = 8

Slow Radius            = 4
Stop Radius            = 1.5

Wander Speed           = 2.5
Wander Change Interval = 1.5
Wander Angle Change    = 45

Avoidance Weight       = 2.5
```

## SteeringSensor

```text
Sensor Distance        = 3
Sensor Radius          = 0.5
Sensor Height          = 0.5
Forward Bias           = 0.5

Obstacle Mask          = Obstacle
```

Parameter tersebut hanya titik awal.

Mahasiswa dianjurkan melakukan tuning sendiri.

---

# 32. Uji Tahap 1 — Arrive Tanpa Obstacle

Untuk memahami behavior secara bertahap, sementara letakkan Player dan NPC pada area tanpa obstacle.

Klik:

```text
Play
```

Gerakkan Player menggunakan:

```text
W A S D
```

atau:

```text
Arrow Keys
```

Yang seharusnya terjadi:

```text
NPC
 ↓
bergerak menuju Player
 ↓
semakin dekat
 ↓
melambat
 ↓
berhenti
```

---

# 33. Mengapa Menggunakan Arrive, Bukan Seek Murni?

Seek sederhana:

```csharp
desiredVelocity =
    direction * maxSpeed;
```

Agent selalu menggunakan kecepatan maksimum.

Saat mendekati target:

```text
NPC ─────────→ Target
```

agent berpotensi:

```text
melewati target
→ berbalik
→ melewati lagi
→ berbalik lagi
```

Akibatnya muncul:

```text
overshoot
jitter
bolak-balik
```

Arrive memperbaiki masalah tersebut.

---

# 34. Cara Kerja Arrive

NPC dibagi menjadi tiga kondisi.

```text
                   TARGET
                     ●

                STOP RADIUS
               ─────────────

                SLOW RADIUS
          ─────────────────────

             FULL SPEED AREA
```

## Jauh dari target

```text
distance >= slowRadius
```

Kecepatan:

```text
maxSpeed
```

## Masuk slow radius

```text
stopRadius < distance < slowRadius
```

Kecepatan berkurang secara bertahap.

## Masuk stop radius

```text
distance <= stopRadius
```

Desired velocity:

```csharp
Vector3.zero
```

---

# 35. Perbaikan Formula Arrive

Versi sederhana pada materi dapat menggunakan:

```csharp
desiredSpeed =
    maxSpeed * (distance / slowRadius);
```

Pada praktikum ini digunakan formula sedikit diperbaiki:

```csharp
float normalizedDistance =
    (distance - stopRadius) /
    (slowRadius - stopRadius);
```

Kemudian:

```csharp
desiredSpeed =
    maxSpeed *
    Mathf.Clamp01(normalizedDistance);
```

Alasannya:

ketika agent mencapai:

```text
stopRadius
```

kecepatan target benar-benar mendekati:

```text
0
```

bukan masih memiliki sebagian kecepatan maksimum.

---

# 36. Uji Parameter Arrive

Saat Play Mode, coba:

```text
Slow Radius = 8
```

Perhatikan NPC mulai melambat lebih jauh.

Kemudian coba:

```text
Slow Radius = 2
```

NPC akan mempertahankan kecepatan tinggi lebih lama.

Eksperimen juga dengan:

```text
Stop Radius = 0.5
Stop Radius = 2
Stop Radius = 4
```

---

# 37. Max Speed

Parameter:

```text
Max Speed
```

menentukan batas maksimum kecepatan agent.

Contoh:

```text
2 → lambat
4 → sedang
8 → cepat
```

Dalam model Unity sederhana ini dapat dianggap sebagai:

```text
Unity units / second
```

---

# 38. Max Acceleration

Parameter:

```text
Max Acceleration
```

mengatur seberapa cepat velocity berubah.

Kode:

```csharp
Vector3.MoveTowards(
    velocity,
    desiredVelocity,
    maxAcceleration * Time.deltaTime
);
```

Jika:

```text
Max Acceleration tinggi
```

NPC cepat mencapai kecepatan yang diinginkan.

Jika:

```text
Max Acceleration rendah
```

NPC membutuhkan waktu lebih lama untuk mempercepat atau memperlambat.

Hal ini membuat movement mempunyai kesan:

```text
massa
momentum
inersia sederhana
```

---

# 39. Turn Speed

Parameter:

```text
Turn Speed
```

menentukan seberapa cepat NPC menghadap ke arah velocity.

Rotasi menggunakan:

```csharp
Quaternion.Slerp()
```

Jika terlalu rendah:

```text
NPC berbelok lambat
```

Jika terlalu tinggi:

```text
NPC berputar hampir seketika
```

---

# 40. Uji Tahap 2 — Obstacle Avoidance

Sekarang letakkan obstacle di antara:

```text
NPC
```

dan:

```text
Player
```

Contoh:

```text
NPC ●

      █████
      █████
      █████

                  ● Player
```

Klik:

```text
Play
```

NPC seharusnya mendeteksi obstacle dan mencoba membelok.

---

# 41. Alur Obstacle Avoidance

Secara konseptual:

```text
Desired Movement
       ↓
SphereCast
       ↓
Obstacle?
       │
   ┌───┴────┐
   │        │
  NO       YES
   │        │
   │      hit.normal
   │        │
   │   Avoid Direction
   │        │
   └────┬───┘
        ↓
Combined Direction
        ↓
Movement
```

---

# 42. Weighted Blending

Pada script:

```csharp
Vector3 combinedDirection =
    checkDirection +
    avoidanceDirection *
    avoidanceWeight;
```

Secara konseptual:

```text
Final Steering
=
Movement Steering
+
Obstacle Avoidance × Weight
```

Jika:

```text
Avoidance Weight = 0
```

NPC hampir mengabaikan obstacle.

Jika:

```text
Avoidance Weight = 1
```

keduanya mempunyai pengaruh serupa.

Jika:

```text
Avoidance Weight = 3
```

avoidance mempunyai pengaruh lebih kuat.

---

# 43. Mengapa Avoidance Weight Biasanya Lebih Tinggi?

Tujuan utama:

```text
Seek / Arrive
```

adalah mencapai target.

Tetapi:

```text
Obstacle Avoidance
```

berhubungan dengan keselamatan movement lokal.

Jika bobotnya terlalu kecil:

```text
Arrive →→→→→ Player
       ↓
   obstacle
       ↓
avoidance terlalu lemah
       ↓
NPC menabrak
```

Karena itu obstacle avoidance umumnya memperoleh prioritas atau bobot lebih tinggi.

---

# 44. Uji Sensor Distance

Coba:

```text
Sensor Distance = 1
```

NPC baru mengetahui obstacle ketika sudah dekat.

Akibatnya:

```text
reaksi terlambat
```

Kemudian coba:

```text
Sensor Distance = 5
```

NPC mendeteksi obstacle lebih awal.

Tetapi terlalu panjang juga dapat menyebabkan:

```text
NPC terlalu cepat bereaksi
```

atau:

```text
memilih menghindar walaupun obstacle masih jauh
```

---

# 45. Uji Sensor Radius

Coba:

```text
Sensor Radius = 0.1
```

SphereCast hampir seperti Raycast.

Kemudian:

```text
Sensor Radius = 1
```

sensor menjadi lebih lebar.

Jika radius terlalu kecil:

```text
NPC dapat menyerempet obstacle
```

Jika terlalu besar:

```text
NPC merasa obstacle lebih besar daripada sebenarnya
```

---

# 46. Uji Tahap 3 — Wander

Pada Play Mode atau sebelum Play, ubah:

```text
Use Target = false
```

Sekarang NPC tidak menggunakan Player sebagai target.

NPC seharusnya masuk behavior:

```text
Wander
```

dan bergerak secara autonomous.

---

# 47. Cara Kerja Wander

Implementasi praktikum sengaja dibuat relatif sederhana.

Agent memiliki:

```text
wanderDirection
```

Setelah waktu:

```text
wanderChangeInterval
```

terpenuhi, arah berubah sedikit secara random.

Contoh:

```text
        ↗
      ↗
NPC ● → → →
        ↘
```

Bukan memilih arah global sepenuhnya random setiap frame.

Hal ini penting.

Jika direction dibuat random setiap frame:

```text
← ↑ ↘ → ↓ ↖ ↑ →
```

movement akan terlihat:

```text
bergetar
tidak natural
```

Dengan perubahan berkala:

```text
→ → → ↗ ↗ ↗ ↑ ↑
```

gerakan terlihat lebih halus.

---

# 48. Wander Angle Change

Parameter:

```text
Wander Angle Change
```

mengatur seberapa besar perubahan arah.

Contoh:

```text
15°
```

gerakan cenderung lurus.

```text
45°
```

gerakan cukup bervariasi.

```text
120°
```

NPC dapat berubah arah secara sangat tajam.

---

# 49. Wander Change Interval

Contoh:

```text
0.2
```

arah berubah sangat sering.

```text
1.5
```

lebih stabil.

```text
5
```

NPC berjalan relatif lama ke arah yang sama.

Tujuannya adalah melihat hubungan antara:

```text
parameter
        ↓
karakter movement
```

---

# 50. Wander + Obstacle Avoidance

Walaupun berada dalam Wander:

```text
Wander
   ↓
Desired Velocity
   ↓
Obstacle Sensor
   ↓
Obstacle?
   ↓
Avoidance
```

Dengan demikian NPC tetap mencoba menghindari obstacle.

Ini menunjukkan bahwa beberapa behavior dapat digunakan bersama.

---

# 51. Menambahkan SteeringDebug.cs

Untuk visual debugging tambahan, buat:

```text
Assets/Scripts/SteeringDebug.cs
```

Isi:

```csharp
using UnityEngine;

public class SteeringDebug : MonoBehaviour
{
    [SerializeField]
    private SteeringAgent agent;

    [SerializeField]
    private float velocityScale = 1f;

    private void Reset()
    {
        agent = GetComponent<SteeringAgent>();
    }

    private void OnDrawGizmosSelected()
    {
        if (agent == null)
        {
            return;
        }

        Vector3 start =
            transform.position + Vector3.up;

        Vector3 end =
            start +
            agent.Velocity * velocityScale;

        Gizmos.DrawLine(start, end);

        Gizmos.DrawWireSphere(
            end,
            0.15f
        );
    }
}
```

Pasang script pada:

```text
NPC
```

---

# 52. Mengapa Gizmos Penting?

Game AI memiliki banyak nilai internal yang tidak terlihat.

Contoh:

```text
velocity
sensor radius
slow radius
stop radius
detection direction
target
```

Tanpa visual debugging, kita hanya melihat:

```text
NPC bergerak aneh
```

tetapi tidak mengetahui penyebabnya.

Dengan Gizmos, kita dapat melihat:

```text
internal state
      ↓
visual representation
      ↓
lebih mudah debugging
```

---

# 53. Melihat Gizmos

Klik GameObject:

```text
NPC
```

pada Scene View.

Pastikan:

```text
Gizmos
```

aktif.

Saat NPC dipilih akan terlihat visualisasi:

```text
Slow Radius
Stop Radius
Sensor
Velocity
Target Line
```

---

# 54. Eksperimen Parameter

Setelah semua behavior bekerja, lakukan eksperimen.

## Eksperimen A

```text
Max Speed = 8
Max Acceleration = 2
```

Pertanyaan:

- Apakah NPC lambat mencapai kecepatan maksimum?
- Apakah NPC lebih mudah overshoot?

---

## Eksperimen B

```text
Max Speed = 4
Max Acceleration = 20
```

Pertanyaan:

- Apakah respons NPC menjadi lebih cepat?
- Apakah gerak terasa lebih kaku?

---

## Eksperimen C

```text
Slow Radius = 8
Stop Radius = 1
```

Amati karakter Arrive.

---

## Eksperimen D

```text
Sensor Distance = 1
```

Kemudian:

```text
Sensor Distance = 5
```

Bandingkan kemampuan obstacle avoidance.

---

## Eksperimen E

```text
Avoidance Weight = 0.5
```

Kemudian:

```text
Avoidance Weight = 4
```

Amati perubahan jalur NPC.

---

# 55. Menambahkan Beberapa NPC

Setelah satu NPC bekerja, duplicate:

```text
NPC
```

menggunakan:

```text
Ctrl + D
```

Buat misalnya:

```text
NPC
NPC (1)
NPC (2)
NPC (3)
```

Semua dapat menggunakan Player yang sama sebagai Target.

Kemungkinan akan terlihat:

```text
NPC
  \
NPC → Player
  /
NPC
```

Tetapi NPC dapat saling bertumpuk.

Ini merupakan motivasi untuk behavior:

```text
Separation
```

yang dipelajari secara konsep pada materi.

---

# 56. Bonus — Separation Sederhana

Bagian ini bersifat **pengembangan tambahan**, bukan requirement utama Praktikum 3.

Tambahkan pada `SteeringAgent.cs`:

```csharp
[Header("Separation")]

[SerializeField]
private LayerMask agentMask;

[SerializeField]
private float separationRadius = 1.5f;

[SerializeField]
private float separationWeight = 1.5f;
```

Tambahkan fungsi:

```csharp
private Vector3 CalculateSeparation()
{
    Collider[] neighbors =
        Physics.OverlapSphere(
            transform.position,
            separationRadius,
            agentMask
        );

    Vector3 separation =
        Vector3.zero;

    int count = 0;

    foreach (Collider neighbor in neighbors)
    {
        if (neighbor.transform == transform)
        {
            continue;
        }

        Vector3 away =
            transform.position -
            neighbor.transform.position;

        away.y = 0f;

        float sqrDistance =
            away.sqrMagnitude;

        if (sqrDistance > 0.001f)
        {
            separation +=
                away.normalized /
                Mathf.Max(sqrDistance, 0.01f);

            count++;
        }
    }

    if (count > 0)
    {
        separation /= count;
    }

    return separation;
}
```

Kemudian behavior tersebut dapat digabung dengan desired velocity.

Secara konseptual:

```text
Arrive
+
Obstacle Avoidance
+
Separation
```

---

# 57. Konsep Emergent Behavior

Jika beberapa agent diberikan aturan sederhana seperti:

```text
jangan terlalu dekat
```

maka tanpa instruksi:

```text
"buat formasi tertentu"
```

dapat muncul pola kelompok.

Inilah yang disebut:

```text
Emergent Behavior
```

Konsep ini menjadi dasar algoritma seperti:

```text
Boids
├── Separation
├── Alignment
└── Cohesion
```

Flocking lengkap tidak menjadi requirement utama praktikum ini.

---

# 58. Perbedaan Steering dan Pathfinding

Praktikum ini tidak menggunakan:

```text
NavMeshAgent
```

karena tujuan utamanya adalah memahami algoritma movement.

Steering menjawab:

```text
Bagaimana saya bergerak sekarang?
```

Sedangkan pathfinding menjawab:

```text
Jalur mana yang harus saya gunakan
untuk mencapai tujuan?
```

Contoh:

```text
               ███████████
               █         █
NPC ●          █         █        ● Target
               █         █
               ███████████
```

Obstacle avoidance lokal belum tentu mampu mengetahui bahwa NPC harus berjalan jauh mengelilingi gedung.

Untuk masalah seperti itu diperlukan:

```text
Pathfinding
```

yang akan dipelajari pada pertemuan berikutnya.

---

# 59. Keterbatasan Obstacle Avoidance Praktikum

SphereCast sederhana adalah:

```text
Local Obstacle Avoidance
```

bukan:

```text
Global Navigation
```

Artinya NPC hanya mengetahui:

```text
"ada obstacle di depan"
```

NPC tidak mengetahui:

```text
"jalan terbaik menuju target berada di sebelah kanan gedung"
```

Akibatnya NPC masih dapat mengalami:

```text
terjebak di sudut
berputar dekat tembok
gagal melewati obstacle berbentuk U
```

Hal tersebut bukan selalu bug implementasi.

Itu adalah keterbatasan algoritma lokal.

---

# 60. Kesalahan Umum 1 — NPC Tidak Bergerak

Periksa:

```text
SteeringAgent aktif?
```

Kemudian:

```text
Target sudah diisi?
```

Pastikan:

```text
Use Target = true
```

Jika menggunakan Wander:

```text
Use Target = false
```

---

# 61. Kesalahan Umum 2 — NPC Tidak Mendeteksi Obstacle

Periksa:

```text
Obstacle mempunyai Collider?
```

Pastikan layer:

```text
Obstacle
```

dan:

```text
SteeringSensor
→ Obstacle Mask
→ Obstacle
```

---

# 62. Kesalahan Umum 3 — NPC Masih Menabrak

Kemungkinan:

```text
Sensor Distance terlalu kecil
```

atau:

```text
Avoidance Weight terlalu rendah
```

Coba:

```text
Sensor Distance = 4
Avoidance Weight = 3
```

Jika NPC sangat cepat, sensor juga perlu lebih panjang.

---

# 63. Kesalahan Umum 4 — NPC Bergetar Dekat Player

Periksa:

```text
Stop Radius
```

Jika terlalu kecil, agent terus melakukan koreksi posisi.

Coba:

```text
Stop Radius = 1.5
```

dan:

```text
Slow Radius = 4
```

---

# 64. Kesalahan Umum 5 — NPC Bergerak Sangat Lambat

Periksa:

```text
Max Speed
Max Acceleration
```

Misalnya:

```text
Max Speed = 4
Max Acceleration = 8
```

---

# 65. Kesalahan Umum 6 — NPC Tidak Berputar

Pastikan:

```text
Turn Speed > 0
```

dan NPC mempunyai velocity.

Rotasi hanya dilakukan jika:

```csharp
horizontalVelocity.sqrMagnitude > 0.001f
```

Hal tersebut mencegah:

```csharp
Quaternion.LookRotation(Vector3.zero)
```

---

# 66. Kesalahan Umum 7 — NPC Menengadah atau Miring

Pastikan semua direction movement ground dibuat:

```csharp
direction.y = 0f;
```

Praktikum ini mengasumsikan movement terjadi pada bidang:

```text
XZ
```

---

# 67. Kesalahan Umum 8 — Movement Berbeda pada FPS Berbeda

Pastikan movement menggunakan:

```csharp
Time.deltaTime
```

Contoh benar:

```csharp
transform.position +=
    velocity * Time.deltaTime;
```

Bukan:

```csharp
transform.position += velocity;
```

---

# 68. Update() dan FixedUpdate()

Praktikum ini memindahkan NPC menggunakan:

```csharp
transform.position
```

sehingga digunakan:

```csharp
Update()
```

Jika pada pengembangan berikutnya digunakan:

```text
Rigidbody
AddForce
physics movement
```

perhitungan movement fisika sebaiknya dilakukan melalui:

```csharp
FixedUpdate()
```

---

# 69. Jangan Mencampur Transform dengan Rigidbody Dinamis

Jika NPC memiliki Rigidbody dinamis dan juga menggunakan:

```csharp
transform.position += ...
```

interaksi fisika dapat menjadi tidak konsisten.

Untuk praktikum dasar ini:

```text
NPC Steering
→ Transform movement
```

Jika ingin menggunakan physics-based movement:

```text
NPC
→ Rigidbody
→ MovePosition / AddForce
→ FixedUpdate
```

Gunakan satu pendekatan secara konsisten.

---

# 70. Checklist Pengujian

Sebelum praktikum dianggap selesai, pastikan:

- [ ] Scene `Praktikum03_Steering` sudah dibuat.
- [ ] Ground tersedia.
- [ ] Player dapat digerakkan.
- [ ] Player menggunakan WASD atau Arrow Keys.
- [ ] NPC mempunyai `SteeringAgent`.
- [ ] NPC mempunyai `SteeringSensor`.
- [ ] Target NPC menunjuk ke Player.
- [ ] NPC dapat bergerak menuju Player.
- [ ] NPC melambat ketika mendekati Player.
- [ ] NPC berhenti di dalam Stop Radius.
- [ ] NPC menghadap ke arah movement.
- [ ] Obstacle memiliki Collider.
- [ ] Obstacle menggunakan Layer `Obstacle`.
- [ ] Obstacle Mask sudah diatur.
- [ ] NPC mendeteksi obstacle.
- [ ] NPC mencoba menghindari obstacle.
- [ ] `Use Target = false` menyebabkan Wander.
- [ ] Wander tetap menggunakan obstacle avoidance.
- [ ] Gizmos dapat digunakan untuk debugging.
- [ ] Parameter AI dapat diubah melalui Inspector.

---

# 71. Eksperimen Wajib Mahasiswa

Lakukan minimal tiga eksperimen.

## Eksperimen 1 — Max Speed

Bandingkan:

```text
Max Speed = 2
Max Speed = 4
Max Speed = 8
```

Catat perubahan movement.

---

## Eksperimen 2 — Slow Radius

Bandingkan:

```text
Slow Radius = 2
Slow Radius = 5
Slow Radius = 10
```

Amati kapan NPC mulai melambat.

---

## Eksperimen 3 — Obstacle Avoidance

Bandingkan:

```text
Avoidance Weight = 0.5
Avoidance Weight = 2
Avoidance Weight = 5
```

Amati kemampuan NPC menghindari obstacle.

---

# 72. Pertanyaan Analisis

Jawab pertanyaan berikut.

1. Apa perbedaan **Seek** dan **Arrive**?

2. Mengapa Seek murni dapat menyebabkan agent melewati target?

3. Apa fungsi:

```csharp
direction.normalized
```

4. Mengapa movement dikalikan dengan:

```csharp
Time.deltaTime
```

5. Apa perbedaan:

```text
Raycast
```

dan:

```text
SphereCast
```

6. Mengapa obstacle menggunakan Layer tersendiri?

7. Apa fungsi:

```text
Avoidance Weight
```

8. Mengapa NPC masih dapat terjebak pada obstacle tertentu walaupun memiliki obstacle avoidance?

9. Apa perbedaan:

```text
Obstacle Avoidance
```

dan:

```text
Pathfinding
```

10. Apa akibat `Max Acceleration` terlalu kecil?

11. Apa akibat `Slow Radius` terlalu besar?

12. Mengapa visual debugging penting dalam pengembangan Game AI?

---

# 73. Tugas Pengembangan

Setelah menyelesaikan praktikum dasar, pilih salah satu pengembangan berikut.

### Level 1

Tambahkan perubahan warna NPC:

```text
Arrive → merah
Wander → biru
Avoiding → kuning
```

### Level 2

Tambahkan:

```text
Flee
```

ketika Player terlalu dekat.

### Level 3

Tambahkan:

```text
Separation
```

agar beberapa NPC tidak bertumpuk.

### Level 4

Tambahkan behavior:

```text
Pursue
```

menggunakan predicted player position.

---

# 74. Tantangan — Animal NPC

Kembangkan NPC menjadi animal agent.

Behavior:

```text
Player jauh
    ↓
Wander

Player dekat
    ↓
Flee

Semua kondisi
    ↓
Obstacle Avoidance
```

Struktur decision:

```text
distanceToPlayer < panicRadius?
           │
       ┌───┴───┐
       │       │
      YES      NO
       │       │
      Flee   Wander
       │       │
       └───┬───┘
           ↓
    Obstacle Avoidance
           ↓
        Movement
```

---

# 75. Catatan Teknis dan Perbaikan Modul

## Catatan 1 — Praktikum sebaiknya menggunakan Arrive sebagai behavior utama

Walaupun Seek merupakan behavior dasar yang sangat penting, penggunaan:

```text
Seek saja
```

kurang ideal sebagai produk akhir praktikum karena NPC mudah overshoot.

Urutan pembelajaran yang disarankan tetap:

```text
Seek
 ↓
lihat kelemahannya
 ↓
Arrive
```

Sedangkan implementasi final menggunakan:

```text
Arrive
```

---

## Catatan 2 — Gunakan SphereCast untuk NPC

Materi memperkenalkan:

```text
Raycast
SphereCast
```

Untuk obstacle avoidance karakter, SphereCast lebih sesuai karena agent memiliki lebar.

Raycast tetap bagus untuk menjelaskan konsep sensor.

---

## Catatan 3 — Hindari obstacle dengan steering, bukan teleport

Jangan menggunakan solusi:

```csharp
transform.position = posisiAman;
```

karena tujuan praktikum adalah memahami **steering**.

Agent seharusnya:

```text
mendeteksi obstacle
        ↓
mengubah desired direction
        ↓
membelok
```

---

## Catatan 4 — Movement AI tidak menggantikan pathfinding

Mahasiswa perlu memahami bahwa:

```text
SphereCast + Avoidance
```

bukan pengganti:

```text
A*
NavMesh
NavMeshAgent
```

Praktikum ini sengaja menggunakan masalah movement lokal sebelum masuk ke navigation pada pertemuan berikutnya.

---

## Catatan 5 — Hindari membuat terlalu banyak behavior sekaligus

Untuk Praktikum 3 wajib, cukup gunakan:

```text
Arrive
Wander
Obstacle Avoidance
```

Sedangkan:

```text
Pursue
Evade
Separation
Alignment
Cohesion
```

lebih baik digunakan sebagai eksperimen atau tugas pengembangan.

Tujuannya agar mahasiswa terlebih dahulu benar-benar memahami pipeline:

```text
Steering
→ Desired Velocity
→ Velocity
→ Movement
```

---

## Catatan 6 — Gunakan parameter Inspector

Jangan hard-code seluruh parameter.

Contoh yang kurang baik:

```csharp
float speed = 4f;
```

lebih baik:

```csharp
[SerializeField]
private float maxSpeed = 4f;
```

Karena Game AI sangat bergantung pada:

```text
Parameter Tuning
```

---

# 76. Hubungan dengan Materi Berikutnya

Pada Praktikum 3:

```text
Target
   ↓
Steering
   ↓
Local Movement
```

Pada pertemuan berikutnya akan dipelajari:

```text
Target
   ↓
Pathfinding
   ↓
Path
   ↓
Waypoint
   ↓
Steering
   ↓
Movement
```

Sehingga:

```text
Pathfinding
```

menentukan:

> Ke mana agent harus lewat?

Sedangkan:

```text
Steering
```

menentukan:

> Bagaimana agent bergerak mengikuti tujuan tersebut?

---

# 77. Ringkasan

Pada praktikum ini telah dibuat:

```text
Autonomous Steering Agent
│
├── Player Controller
│
├── Arrive
│   ├── Max Speed
│   ├── Slow Radius
│   └── Stop Radius
│
├── Dynamic Velocity
│   └── Max Acceleration
│
├── Smooth Rotation
│   └── Turn Speed
│
├── Wander
│
├── Obstacle Sensor
│   ├── SphereCast
│   ├── LayerMask
│   └── RaycastHit
│
├── Obstacle Avoidance
│
└── Gizmos Debugging
```

Konsep terpenting:

```text
Target / Environment
        ↓
Steering Behavior
        ↓
Desired Velocity
        ↓
Acceleration
        ↓
Velocity
        ↓
Position
        ↓
Rotation
```

Dengan kombinasi aturan sederhana tersebut, NPC mulai menunjukkan **autonomous movement** yang terlihat lebih natural dibanding movement langsung menuju target.

---

# 78. Kesimpulan

Praktikum ini menunjukkan bahwa movement NPC yang terlihat cerdas tidak selalu memerlukan algoritma yang sangat kompleks.

Behavior sederhana seperti:

```text
Arrive
+
Wander
+
Obstacle Avoidance
```

sudah dapat menghasilkan agent yang:

```text
bergerak mandiri
mengejar target
melambat secara natural
menghindari obstacle
menjelajah lingkungan
```

Konsep tersebut menjadi fondasi untuk materi berikutnya:

```text
Pathfinding
Navigation
NavMesh
Flocking
Behavior Tree
dan Game AI yang lebih kompleks
```