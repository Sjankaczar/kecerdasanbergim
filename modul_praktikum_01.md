# MODUL PRAKTIKUM 1
## NPC Detector: Perception dan Parameter AI di Unity 6

**Mata Kuliah:**  Game Cerdas
**Program Studi:**  S1 Teknik Informatika
**Game Engine:**  Unity 6
**Bahasa Pemrograman:**  C#

---

# 1. Tujuan Praktikum

Pada praktikum ini mahasiswa akan membuat prototype**NPC Detector** sederhana.

NPC dapat:

- mendeteksi Player berdasarkan jarak,
- memiliki state `IDLE` dan `ALERT`,
- berubah warna sesuai state,
- memiliki parameter `Detection Radius`,
- menampilkan area deteksi menggunakan Gizmos.

Player dapat digerakkan menggunakan:

```text
W A S D
```

atau:

```text
Arrow Key
↑ ↓ ← →
```

Praktikum ini digunakan untuk memahami konsep dasar:

```text
Perception → Decision → Action
```

---

# 2. Hasil Akhir yang Akan Dibuat

Scene terdiri atas:

```text
Main Camera
Directional Light
Ground
Player
Enemy
```

Player dapat bergerak menggunakan keyboard.

Enemy memiliki radius deteksi.

```text
                 Player
                   ●
                   │
                   │ bergerak
                   ▼

          .-------------------.
       .'                       '.
      /     Detection Radius      \
     |                             |
     |            Enemy            |
     |              ■              |
     |                             |
      \                           /
       '.                       .'
          '-------------------'
```

Jika Player di luar radius:

```text
Enemy = IDLE
Warna = Biru
```

Jika Player masuk radius:

```text
Enemy = ALERT
Warna = Merah
```

---

# 3. Konsep Perception–Decision–Action

NPC dapat dianggap sebagai sebuah**intelligent agent**.

```text
Environment
     │
     ▼
Perception
     │
     ▼
Decision
     │
     ▼
Action
     │
     ▼
Environment
```

Pada praktikum ini:

```text
PERCEPTION
Menghitung jarak Player
        ↓
DECISION
Apakah Player berada
di dalam Detection Radius?
        ↓
   ┌�”€─�”€─�”´─�”€─�”€┐
   ▼         ▼
 IDLE      ALERT
   │         │
   └�”€─�”€─�”¬─�”€─�”€┘
        ▼
ACTION
Mengubah warna Enemy
```

---

# 4. Membuat Project Unity

Buka:

```text
Unity Hub
```

Klik:

```text
New Project
```

Pilih template:

```text
Universal 3D
```

atau:

```text
3D Core
```

Nama project:

```text
GameCerdas_Praktikum01_NPCDetector
```

Klik:

```text
Create Project
```

---

# 5. Menyimpan Scene

Setelah Unity terbuka, pilih:

```text
File
→ Save As
```

Buat folder:

```text
Assets/Scenes
```

Simpan scene dengan nama:

```text
NPCDetector
```

---

# 6. Membuat Struktur Folder

Pada folder `Assets`, buat:

```text
Assets
│
├�”€─ Materials
├�”€─ Scenes
└�”€─ Scripts
```

Folder digunakan agar project lebih terstruktur.

---

# 7. Membuat Ground

Pada Hierarchy:

```text
Right Click
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
```

Scale:

```text
X = 2
Y = 1
Z = 2
```

---

# 8. Membuat Player

Pada Hierarchy:

```text
Right Click
→ 3D Object
→ Capsule
```

Rename:

```text
Player
```

Atur Transform:

```text
Position
X = 0
Y = 1
Z = -6
```

Player akan digunakan untuk menguji kemampuan Enemy mendeteksi objek.

---

# 9. Membuat Player Controller

Agar pengujian NPC lebih mudah, Player akan dibuat dapat bergerak menggunakan:

```text
WASD
```

atau:

```text
Arrow Key
```

Kontrol:

| Aksi | WASD | Arrow Key |
|---|---|---|
| Maju | W | ↑ |
| Mundur | S | ↓ |
| Kiri | A | ← |
| Kanan | D | → |

---

# 10. Memastikan Input System Tersedia

Buka:

```text
Window
→ Package Manager
```

Cari:

```text
Input System
```

Jika belum terpasang:

```text
Install
```

Jika Unity meminta restart atau mengganti input system, izinkan Unity melakukan restart.

Jika diperlukan, cek:

```text
Edit
→ Project Settings
→ Player
→ Active Input Handling
```

Gunakan:

```text
Input System Package (New)
```

atau:

```text
Both
```

---

# 11. Membuat Script PlayerController

Masuk ke folder:

```text
Assets/Scripts
```

Klik kanan:

```text
Create
→ MonoBehaviour Script
```

Beri nama:

```text
PlayerController
```

Buka file:

```text
PlayerController.cs
```

Ganti isinya dengan kode berikut:

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 5f;

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (Keyboard.current == null)
            return;

        float horizontal = 0f;
        float vertical = 0f;

        // Kiri
        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            horizontal = -1f;
        }

        // Kanan
        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            horizontal = 1f;
        }

        // Maju
        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed)
        {
            vertical = 1f;
        }

        // Mundur
        if (Keyboard.current.sKey.isPressed ||
            Keyboard.current.downArrowKey.isPressed)
        {
            vertical = -1f;
        }

        Vector3 movement =
            new Vector3(horizontal, 0f, vertical);

        movement = movement.normalized;

        transform.position +=
            movement * moveSpeed * Time.deltaTime;
    }
}
```

Simpan:

```text
Ctrl + S
```

---

# 12. Penjelasan PlayerController

Parameter:

```csharp
[SerializeField]
private float moveSpeed = 5f;
```

akan muncul pada Inspector sebagai:

```text
Move Speed
```

Nilainya dapat diubah tanpa mengedit program.

Contoh:

```text
Move Speed = 3
```

Player bergerak lambat.

```text
Move Speed = 5
```

Player bergerak normal.

```text
Move Speed = 10
```

Player bergerak lebih cepat.

---

## Membaca Keyboard

Contoh:

```csharp
Keyboard.current.wKey.isPressed
```

berarti:

> Apakah tombol W sedang ditekan?

Sedangkan:

```csharp
Keyboard.current.upArrowKey.isPressed
```

berarti:

> Apakah tombol ↑ sedang ditekan?

Operator:

```csharp
||
```

berarti:

```text
OR / ATAU
```

Sehingga:

```csharp
Keyboard.current.wKey.isPressed ||
Keyboard.current.upArrowKey.isPressed
```

berarti:

> W atau Arrow Up dapat digunakan untuk bergerak maju.

---

# 13. Mengapa Menggunakan normalized?

Kode:

```csharp
movement = movement.normalized;
```

digunakan agar Player tidak bergerak lebih cepat ketika bergerak diagonal.

Contoh:

```text
W + D
```

menghasilkan:

```text
↗
```

Tanpa normalisasi, kecepatan diagonal dapat lebih tinggi daripada gerakan lurus.

---

# 14. Mengapa Menggunakan Time.deltaTime?

Kode:

```csharp
movement * moveSpeed * Time.deltaTime
```

membuat gerakan lebih konsisten terhadap frame rate.

Dengan demikian game pada:

```text
30 FPS
60 FPS
120 FPS
```

tidak menyebabkan perbedaan kecepatan Player yang terlalu besar.

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
→ Player Controller
```

Atur:

```text
Move Speed = 5
```

---

# 16. Menguji Player

Tekan:

```text
Play
```

Klik terlebih dahulu area:

```text
Game
```

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

Kontrol:

```text
               W / ↑
                 ↑

       A / ←   PLAYER   D / →

                 ↓
               S / ↓
```

Coba juga gerakan diagonal:

```text
W + D → ↗
W + A → ↖
S + D → ↘
S + A → ↙
```

Jika Player dapat bergerak dengan baik, lanjutkan.

---

# 17. Membuat Enemy

Keluar dari Play Mode.

Pada Hierarchy:

```text
Right Click
→ 3D Object
→ Capsule
```

Rename:

```text
Enemy
```

Atur:

```text
Position
X = 0
Y = 1
Z = 2
```

Posisi awal menjadi:

```text
                Enemy
                  ■

                  ↑
                  │
                  │
                  │

                Player
                  ●
```

---

# 18. Membuat Material Enemy

Buka:

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
EnemyMaterial
```

Drag material tersebut ke:

```text
Enemy
```

Material akan digunakan untuk menunjukkan perubahan state AI.

---

# 19. Membuat EnemyDetector

Buka:

```text
Assets/Scripts
```

Klik kanan:

```text
Create
→ MonoBehaviour Script
```

Nama:

```text
EnemyDetector
```

Buka:

```text
EnemyDetector.cs
```

Masukkan kode berikut:

```csharp
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Alert
    }

    [Header("Target")]
    [SerializeField]
    private Transform player;

    [Header("AI Parameters")]
    [SerializeField]
    [Min(0f)]
    private float detectionRadius = 5f;

    [Header("Visual")]
    [SerializeField]
    private Color idleColor = Color.blue;

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

        currentDistance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (currentDistance <= detectionRadius)
        {
            SetState(EnemyState.Alert);
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

        Debug.Log(
            "Enemy State → " + currentState
        );

        if (enemyRenderer == null)
            return;

        if (currentState == EnemyState.Alert)
        {
            enemyRenderer.material.color = alertColor;
        }
        else
        {
            enemyRenderer.material.color = idleColor;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRadius
        );
    }
}
```

Simpan script.

---

# 20. Memasang EnemyDetector

Pilih:

```text
Enemy
```

Drag:

```text
EnemyDetector.cs
```

ke Inspector Enemy.

Component akan menampilkan:

```text
Enemy Detector

Target
    Player

AI Parameters
    Detection Radius

Visual
    Idle Color
    Alert Color

Debug
    Current State
    Current Distance
```

---

# 21. Menghubungkan Player

Pada Hierarchy terdapat:

```text
Player
Enemy
```

Pilih:

```text
Enemy
```

Pada:

```text
Enemy Detector
→ Player
```

drag GameObject:

```text
Player
```

ke field tersebut.

Hasil:

```text
Player = Player
```

Jangan biarkan:

```text
Player = None
```

---

# 22. Mengatur Parameter Enemy

Pada Inspector Enemy:

```text
Detection Radius = 5
```

Atur:

```text
Idle Color = Blue
Alert Color = Red
```

Sekarang parameter perilaku NPC dapat diubah melalui Inspector.

---

# 23. Perception pada NPC

Perception dilakukan dengan:

```csharp
currentDistance = Vector3.Distance(
    transform.position,
    player.position
);
```

Kode menghitung jarak:

```text
Enemy
  ■
  │
  │
  │ Current Distance
  │
  ●
Player
```

Informasi yang diketahui NPC adalah:

```text
Seberapa jauh Player dari Enemy?
```

---

# 24. Decision pada NPC

Kode:

```csharp
if (currentDistance <= detectionRadius)
{
    SetState(EnemyState.Alert);
}
else
{
    SetState(EnemyState.Idle);
}
```

membentuk aturan:

```text
            Hitung jarak
                 │
                 ▼
     distance <= detectionRadius?
            /             \
          YA               TIDAK
          │                  │
          ▼                  ▼
        ALERT               IDLE
```

---

# 25. Action pada NPC

Ketika:

```text
State = IDLE
```

Enemy menggunakan:

```text
Idle Color
```

Ketika:

```text
State = ALERT
```

Enemy menggunakan:

```text
Alert Color
```

Sehingga:

```text
PERCEPTION
   ↓
Menghitung jarak

DECISION
   ↓
IDLE / ALERT

ACTION
   ↓
Mengubah warna
```

---

# 26. Menguji NPC Detector

Tekan:

```text
Play
```

Klik Game View.

Player berada di luar radius.

Enemy seharusnya:

```text
State = Idle
Color = Blue
```

Gerakkan Player menuju Enemy:

```text
W
```

atau:

```text
↑
```

Ketika Player masuk ke Detection Radius:

```text
State = Alert
Color = Red
```

Kemudian menjauh:

```text
S
```

atau:

```text
↓
```

Enemy kembali:

```text
State = Idle
Color = Blue
```

---

# 27. Mengamati Current Distance

Saat game berjalan, pilih:

```text
Enemy
```

Perhatikan Inspector.

Contoh:

```text
Detection Radius = 5

Current Distance = 8.4
Current State    = Idle
```

Ketika mendekat:

```text
Current Distance = 5.7
Current State    = Idle
```

Setelah melewati radius:

```text
Current Distance = 4.8
Current State    = Alert
```

Karena:

```text
4.8 <= 5
```

maka Player terdeteksi.

---

# 28. Menampilkan Detection Radius dengan Gizmos

Pada Scene View, pilih Enemy.

Pastikan:

```text
Gizmos
```

aktif.

Kode:

```csharp
void OnDrawGizmosSelected()
{
    Gizmos.color = Color.yellow;

    Gizmos.DrawWireSphere(
        transform.position,
        detectionRadius
    );
}
```

akan menggambar area:

```text
             Detection Radius
          .-------------------.
       .'                       '.
      /                           \
     |                             |
     |            Enemy            |
     |              ■              |
     |                             |
      \                           /
       '.                       .'
          '-------------------'
```

Gizmos hanya digunakan untuk membantu developer saat debugging.

Gizmos tidak menjadi objek game sebenarnya.

---

# 29. Menguji Detection Radius

Ubah:

```text
Detection Radius = 3
```

Jalankan game.

Player harus lebih dekat untuk dideteksi.

Kemudian coba:

```text
Detection Radius = 8
```

Player dapat dideteksi dari jarak lebih jauh.

Bandingkan:

| Radius | Perilaku |
|---:|---|
| 3 | Enemy mendeteksi dari dekat |
| 5 | Jarak sedang |
| 8 | Enemy mendeteksi lebih jauh |

Hal ini menunjukkan:

```text
AI Parameter
     ↓
AI Behavior
```

---

# 30. Mengamati Console

Buka:

```text
Window
→ General
→ Console
```

Saat Player masuk radius:

```text
Enemy State → Alert
```

Saat keluar:

```text
Enemy State → Idle
```

Log hanya ditampilkan ketika state berubah.

Ini mempermudah debugging.

---

# 31. Arsitektur Sistem Akhir

Keseluruhan sistem yang dibuat:

```text
           KEYBOARD INPUT
          WASD / Arrow Key
                 │
                 ▼
        PlayerController
                 │
                 ▼
          Player bergerak
                 │
                 ▼
          Player Position
                 │
                 ▼
        ┌�”€─�”€─�”€─�”€─�”€─�”€─�”€─�”€─�”€┐
        │   PERCEPTION    │
        │Vector3.Distance │
        └�”€─�”€─�”€─�”€─�”¬─�”€─�”€─�”€─�”€┘
                 │
                 ▼
          Current Distance
                 │
                 ▼
        ┌�”€─�”€─�”€─�”€─�”€─�”€─�”€─�”€─�”€┐
        │    DECISION     │
        │ distance <=     │
        │ detectionRadius │
        └�”€─�”€─�”€─�”€─�”¬─�”€─�”€─�”€─�”€┘
                 │
          ┌�”€─�”€─�”€─�”´─�”€─�”€─�”€┐
          ▼             ▼
        IDLE          ALERT
          │             │
          └�”€─�”€─�”€─�”¬─�”€─�”€─�”€┘
                 ▼
        ┌�”€─�”€─�”€─�”€─�”€─�”€─�”€─�”€─�”€┐
        │     ACTION      │
        │  Change Color   │
        └�”€─�”€─�”€─�”€─�”€─�”€─�”€─�”€─�”€┘
```

---

# 32. Hubungan dengan Intelligent Agent

| Konsep | Implementasi |
|---|---|
| Environment | Scene Unity |
| Player | Object yang diamati |
| Agent | Enemy |
| Perception | `Vector3.Distance()` |
| Sensor Data | `currentDistance` |
| AI Parameter | `detectionRadius` |
| Internal State | `Idle`, `Alert` |
| Decision | Membandingkan distance dan radius |
| Action | Mengubah warna |
| Debugging | Inspector, Console, Gizmos |

---

# 33. Struktur Hierarchy Akhir

Hierarchy:

```text
NPCDetector
│
├�”€─ Main Camera
├�”€─ Directional Light
├�”€─ Ground
├�”€─ Player
│   └�”€─ PlayerController
│
└�”€─ Enemy
    └�”€─ EnemyDetector
```

---

# 34. Struktur Folder Akhir

Project:

```text
Assets
│
├�”€─ Materials
│   └�”€─ EnemyMaterial
│
├�”€─ Scenes
│   └�”€─ NPCDetector.unity
│
└�”€─ Scripts
    ├�”€─ PlayerController.cs
    └�”€─ EnemyDetector.cs
```

---

# 35. Troubleshooting

## Player tidak bergerak

Pastikan Player memiliki:

```text
Player Controller
```

Pastikan:

```text
Move Speed > 0
```

Klik Game View sebelum menekan keyboard.

---

## Error InputSystem

Jika muncul error:

```text
The type or namespace name 'InputSystem'
could not be found
```

buka:

```text
Window
→ Package Manager
→ Input System
```

Install Input System.

---

## Player hanya bergerak dengan salah satu tombol

Periksa kode PlayerController.

Harus terdapat:

```csharp
wKey
aKey
sKey
dKey
```

dan:

```csharp
upArrowKey
downArrowKey
leftArrowKey
rightArrowKey
```

---

## Enemy tidak berubah menjadi ALERT

Periksa:

```text
Enemy
→ Enemy Detector
→ Player
```

Pastikan:

```text
Player = Player
```

bukan:

```text
None
```

---

## Enemy selalu ALERT

Perhatikan:

```text
Current Distance
```

Jika:

```text
Current Distance <= Detection Radius
```

maka kondisi tersebut benar.

Coba jauhkan Player.

---

## Gizmos tidak tampil

Pastikan:

- Enemy dipilih,
- Scene View aktif,
- tombol Gizmos aktif.

---

## Console menunjukkan error merah

Buka:

```text
Window
→ General
→ Console
```

Selesaikan semua error sebelum melanjutkan.

---

# 36. Eksperimen

## Eksperimen 1 — Detection Radius

Uji:

```text
3
5
8
```

Catat pengaruhnya.

---

## Eksperimen 2 — Player Speed

Uji:

```text
Move Speed = 2
Move Speed = 5
Move Speed = 10
```

Apakah perubahan kecepatan Player mengubah logika AI?

Tidak.

Yang berubah adalah kecepatan perubahan kondisi lingkungan yang diterima AI.

---

## Eksperimen 3 — Warna Enemy

Ubah:

```text
Idle Color
```

dan:

```text
Alert Color
```

Perhatikan bahwa perubahan visual tidak mengubah logika perception.

---

# 37. Pertanyaan Analisis

Jawab pertanyaan berikut.

1. Apa yang menjadi**Perception** pada praktikum ini?
2. Apa yang menjadi**Decision**?
3. Apa yang menjadi**Action**?
4. Apa fungsi `Vector3.Distance()`?
5. Apa fungsi `Detection Radius`?
6. Mengapa `Detection Radius` sebaiknya dapat diubah melalui Inspector?
7. Apa manfaat `enum EnemyState`?
8. Apa fungsi `currentDistance`?
9. Apa fungsi Gizmos?
10. Apa fungsi `Time.deltaTime` pada PlayerController?
11. Mengapa movement menggunakan `.normalized`?
12. Apakah Detection Radius yang lebih besar selalu berarti NPC lebih pintar?
13. Apakah NPC benar-benar "melihat" Player seperti manusia? Jelaskan.

---

# 38. Tantangan Tambahan — State SUSPICIOUS

Jika prototype dasar sudah selesai, tambahkan satu state:

```text
Suspicious
```

Sehingga:

```text
Idle
Suspicious
Alert
```

Aturan:

```text
Distance > 8
→ IDLE
```

```text
Distance <= 8
→ SUSPICIOUS
```

```text
Distance <= 4
→ ALERT
```

Visual:

```text
IDLE        = Blue
SUSPICIOUS  = Yellow
ALERT       = Red
```

Ilustrasi:

```text
               Radius 8
         .-------------------.
       .'                     '.
      /      SUSPICIOUS         \
     |                           |
     |         Radius 4          |
     |       .----------.        |
     |      /            \       |
     |     |    ALERT     |      |
     |     |      ■       |      |
     |      \            /       |
     |       '----------'        |
      \                         /
       '.                     .'
         '-------------------'

Di luar Radius 8:

IDLE
```

Tantangan ini bersifat tambahan dan tidak menjadi requirement minimum.

---

# 39. Checklist Praktikum

Pastikan:

- [ ] Project berhasil dibuat.
- [ ] Scene `NPCDetector` dibuat.
- [ ] Ground dibuat.
- [ ] Player dibuat.
- [ ] Enemy dibuat.
- [ ] Input System aktif.
- [ ] `PlayerController.cs` dibuat.
- [ ] Player dapat bergerak dengan WASD.
- [ ] Player dapat bergerak dengan Arrow Key.
- [ ] Gerak diagonal bekerja.
- [ ] `EnemyDetector.cs` dibuat.
- [ ] Player sudah dimasukkan ke EnemyDetector.
- [ ] Detection Radius dapat diubah.
- [ ] Current Distance muncul.
- [ ] Enemy memiliki state IDLE.
- [ ] Enemy memiliki state ALERT.
- [ ] Enemy berubah warna sesuai state.
- [ ] Gizmos menampilkan Detection Radius.
- [ ] Console menampilkan perubahan state.
- [ ] Tidak terdapat error merah pada Console.

---

# 40. Output Praktikum

Mahasiswa mengumpulkan:

## Project Unity

Nama project:

```text
NRP_Nama_Praktikum01_NPCDetector
```

---

## Screenshot 1

Menunjukkan:

```text
Player berada di luar radius
Enemy = IDLE
```

---

## Screenshot 2

Menunjukkan:

```text
Player berada di dalam radius
Enemy = ALERT
```

---

## Screenshot 3

Menunjukkan Inspector Enemy:

```text
Detection Radius
Current State
Current Distance
```

---

## Screenshot 4

Menunjukkan:

```text
Detection Radius Gizmos
```

---

## Video/GIF

Durasi sekitar:

```text
10–30 detik
```

Menampilkan:

```text
Player bergerak
      ↓
Enemy IDLE
      ↓
Player masuk radius
      ↓
Enemy ALERT
      ↓
Player keluar radius
      ↓
Enemy IDLE
```

---

# 41. Laporan Singkat

Buat laporan sekitar**1–2 halaman**, terdiri atas:

1. Tujuan praktikum.
2. Konsep Perception–Decision–Action.
3. Implementasi Player Controller.
4. Implementasi NPC Detector.
5. Hasil eksperimen Detection Radius.
6. Screenshot hasil.
7. Kendala yang ditemukan.
8. Kesimpulan.

---

# 42. Kriteria Penilaian

| Komponen | Bobot |
|---|---:|
| Setup Scene dan GameObject | 10% |
| Player Controller WASD/Arrow | 10% |
| Perception / Distance Detection | 20% |
| Decision IDLE/ALERT | 15% |
| Visual Action / perubahan warna | 10% |
| Parameter Inspector | 10% |
| Gizmos dan Debugging | 10% |
| Kerapian kode | 5% |
| Analisis dan laporan | 10% |
|**TOTAL** |**100%** |

---

# 43. Ringkasan

Pada Praktikum 1 mahasiswa telah membuat sistem:

```text
Keyboard
WASD / Arrow
      ↓
Player Controller
      ↓
Player bergerak
      ↓
Posisi Player berubah
      ↓
Enemy melakukan Perception
      ↓
Menghitung Distance
      ↓
Decision
      ↓
IDLE / ALERT
      ↓
Action
      ↓
Perubahan warna
```

Komponen utama AI yang dipelajari adalah:

```text
PERCEPTION
Vector3.Distance()

        ↓

DECISION
distance <= detectionRadius

        ↓

STATE
IDLE / ALERT

        ↓

ACTION
Change Color
```

Debugging dilakukan menggunakan:

```text
Current Distance
Current State
Console Log
Gizmos
```

---

# 44. Kesimpulan

Praktikum ini menunjukkan bahwa NPC sederhana sudah dapat dimodelkan sebagai**intelligent agent**.

NPC menerima informasi dari lingkungan:

```text
Player Position
```

kemudian melakukan perception:

```text
Vector3.Distance()
```

selanjutnya menentukan keputusan berdasarkan:

```text
Detection Radius
```

dan menghasilkan state:

```text
IDLE
```

atau:

```text
ALERT
```

State tersebut kemudian diterjemahkan menjadi action berupa perubahan warna Enemy.

Player Controller dengan:

```text
WASD
```

atau:

```text
Arrow Key
```

memungkinkan mahasiswa menguji sistem AI secara langsung dan melihat bagaimana perubahan kondisi lingkungan menyebabkan perubahan state NPC.

Prototype ini menjadi fondasi untuk praktikum selanjutnya ketika NPC mulai memiliki kemampuan yang lebih kompleks seperti:

```text
Sensor
   ↓
Memory
   ↓
Decision
   ↓
Patrol
   ↓
Chase
   ↓
Last Seen Position
   ↓
Return to Patrol
```**Konsep utama yang harus dipahami:**  
>**Game AI tidak harus dimulai dari algoritma yang kompleks. Sistem sederhana yang mampu mengamati lingkungan, mengambil keputusan berdasarkan parameter, dan memberikan respons yang jelas sudah merupakan fondasi penting dari intelligent game agent.**