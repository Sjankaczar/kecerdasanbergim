# TUGAS UTS — GAME CERDAS
## Intelligent Game Mini Project

**Program Studi:**  S1 Teknik Informatika
**Game Engine:**  Unity 6
**Bahasa Pemrograman:**  C#
**Bentuk Tugas:**  Project Kelompok
**Jumlah Anggota:**  Maksimal 3 mahasiswa per kelompok  

---

# 1. Tema UTS

## Building a Playable Intelligent Game

UTS mata kuliah**Game Cerdas** dilaksanakan dalam bentuk**mini project kelompok**.

Mahasiswa diminta membuat sebuah mini game menggunakan Unity yang mengintegrasikan materi Game AI dari**Pertemuan 1 sampai Pertemuan 7**.

Project harus memenuhi tiga prinsip utama:

```text
PLAYABLE
+
INTELLIGENT
+
POLISHED
```

Artinya:
- game dapat dimainkan dengan jelas,
- AI memberikan pengaruh nyata terhadap gameplay,
- environment dan asset cukup baik,
- terdapat objective,
- terdapat kondisi menang/kalah,
- AI dapat diamati dan diuji.

Project tidak cukup hanya berupa demo script atau scene Unity berisi beberapa GameObject.

Target akhirnya adalah sebuah:

>**Playable Intelligent Game Vertical Slice**  
---

# 2. Tujuan UTS

Melalui project ini, mahasiswa diharapkan mampu mengintegrasikan konsep-konsep yang telah dipelajari pada Pertemuan 1–7.

```text
Pertemuan 1
Introduction to Game AI
        ↓
Pertemuan 2
Agent Architecture
Perception + Memory
        ↓
Pertemuan 3
Movement & Steering
        ↓
Pertemuan 4
Pathfinding & Navigation
        ↓
Pertemuan 5
Finite State Machine
        ↓
Pertemuan 6
Behavior Tree / Utility AI
        ↓
Pertemuan 7
Tactical AI & Integration
```

Target utama project:

```text
Perception
    ↓
Memory
    ↓
Decision
    ↓
Movement / Navigation
    ↓
Tactical Behavior
    ↓
Gameplay
```

---

# 3. Ketentuan Kelompok

Project dikerjakan secara berkelompok.

Jumlah anggota:

```text
Maksimal 3 mahasiswa
```

Setiap anggota harus berkontribusi pada aspek teknis project.

Contoh pembagian pekerjaan:

## Anggota 1 — AI Perception & Decision
- sensor/perception,
- memory,
- FSM,
- Behavior Tree,
- Utility AI,
- target selection,
- AI debugging.

## Anggota 2 — Movement & Navigation
- steering behavior,
- autonomous movement,
- NavMesh,
- waypoint,
- obstacle avoidance,
- cover,
- tactical positioning.

## Anggota 3 — Gameplay & Integration
- Player Controller,
- combat system,
- health,
- Game Manager,
- UI,
- audio,
- visual effects,
- level/environment,
- integrasi seluruh sistem.

Pembagian tersebut hanya contoh.

Kelompok dapat menggunakan pembagian lain sesuai kebutuhan.

Namun:

>**Semua anggota wajib memahami keseluruhan arsitektur AI dan gameplay project.**  
Pada saat demo, setiap anggota dapat diminta menjelaskan bagian AI tertentu.

---

# 4. Requirement Minimum Game

Setiap project wajib memiliki:

```text
1 Playable Level / Arena
1 Player Controller
Minimal 3 NPC / Enemy
Minimal 3 AI Behaviors
Perception System
Memory sederhana
Movement / Steering
Navigation / Pathfinding
Decision Making
Minimal 1 Tactical Behavior
Win Condition
Lose Condition
UI sederhana
Audio / Visual Feedback
Asset non-default Unity
AI Debug Mode
```

Game harus dapat dimainkan dari:

```text
START
   ↓
GAMEPLAY
   ↓
OBJECTIVE
   ↓
WIN / LOSE
```

---

# 5. Integrasi Materi Pertemuan 1–7

## 5.1 Perception

NPC harus memiliki kemampuan mendeteksi kondisi environment.

Minimal menggunakan salah satu atau kombinasi:

```text
Distance
Field of View
Raycast
Line of Sight
Hearing
Trigger
```

## 5.2 Memory

NPC harus memiliki bentuk memory sederhana.

Contoh:

```text
Last Known Position
Last Seen Time
Last Target
Alert Memory
Sound Position
```

## 5.3 Movement / Steering

Project harus menggunakan autonomous movement.

Contoh:

```text
Seek
Arrive
Pursue
Evade
Wander
Obstacle Avoidance
Separation
```

Minimal satu steering behavior harus terlihat dalam gameplay.

## 5.4 Navigation / Pathfinding

NPC harus dapat bergerak menuju target menggunakan:

```text
Unity NavMesh
```

atau:

```text
Waypoint / A*
```

## 5.5 Decision Making

Project wajib menggunakan minimal salah satu:

```text
Finite State Machine
Behavior Tree
Utility AI
```

## 5.6 Tactical Behavior

Minimal satu tactical behavior harus diterapkan.

Contoh:

```text
Cover
Flanking
Target Selection
Maintain Distance
Retreat
Shared Alert
Group Coordination
Role Assignment
```

Tactical behavior harus memiliki dampak nyata terhadap gameplay.

---

# 6. Requirement Asset dan Environment

Project UTS harus menggunakan environment dan asset yang cukup baik sehingga terasa sebagai sebuah mini game.

Project final**tidak diperbolehkan hanya menggunakan**:

```text
Cube
Capsule
Sphere
Plane
Material default Unity
```

Primitive Unity tetap boleh digunakan pada tahap prototype.

Namun pada hasil akhir harus menggunakan asset yang lebih layak.

## Asset Minimum yang Disarankan

Gunakan setidaknya:
- environment,
- player character,
- enemy/NPC character,
- props,
- animation,
- UI,
- sound effect,
- visual effect sederhana.

Contoh environment:

```text
Sci-Fi Facility
Dungeon
Military Outpost
Abandoned City
Robot Arena
```

## Sumber Asset

Asset dapat diperoleh dari:
- Unity Asset Store,
- Kenney,
- itch.io,
- Mixamo,
- Poly Haven,
- asset open-source,
- asset buatan sendiri.

Mahasiswa wajib memastikan bahwa asset yang digunakan memiliki izin/lisensi yang sesuai.

## Konsistensi Visual

Hindari mencampurkan asset dengan style yang sangat berbeda.

Pilih satu gaya visual yang konsisten, misalnya:

```text
Stylized Low Poly
Sci-Fi Stylized
Fantasy Stylized
Military Stylized
```

---

# 7. Target Playable

Target waktu bermain:

```text
sekitar 5–10 menit
```

Rekomendasi:

```text
1 level kecil tetapi selesai
```

lebih baik daripada:

```text
banyak level tetapi tidak selesai
```

Konsep yang digunakan adalah:

## Vertical Slice

Vertical Slice berarti:

> Bagian kecil dari sebuah game tetapi memiliki gameplay loop, AI, visual, UI, objective, dan kondisi akhir yang lengkap.

---

# 8. Enam Topik Game yang Bisa Dipilih

Mahasiswa memilih **satu dari enam topik** berikut.

Penjelasan pada bagian ini hanya berupa ringkasan. Spesifikasi gameplay, AI, minimum requirement, HUD, AI Debug Mode, dan skenario pengujian masing-masing topik dijelaskan pada **dokumen topik terpisah**.

Kelompok diperbolehkan mengembangkan variasi gameplay selama tetap memenuhi requirement utama UTS Game Cerdas.

---

## TOPIK 1 — Stealth Infiltration: Intelligent Security Facility

Player menyusup ke fasilitas keamanan untuk mengambil objective lalu mencapai extraction point tanpa tertangkap.

Fokus AI:

```text
Perception
+
Suspicion / Detection
+
Memory
+
Patrol / Investigate / Chase / Search
+
Shared Alert
+
Search Coordination
```

Contoh environment:

```text
Research Facility
Laboratory
Military Base
Sci-Fi Facility
```

---

## TOPIK 2 — Tactical Outpost Defense: Intelligent Assault AI

Player mempertahankan outpost atau reactor dari beberapa wave enemy dengan role dan tactical behavior berbeda.

Fokus AI:

```text
Enemy Roles
+
Target Selection
+
Cover
+
Flanking
+
Utility AI / FSM
+
Group Coordination
```

Contoh environment:

```text
Military Outpost
Desert Camp
Sci-Fi Base
Industrial Facility
```

---

## TOPIK 3 — Dungeon Hunter: Intelligent Enemy Dungeon

Player menjelajahi dungeon, mencari key atau artifact, melawan beberapa tipe enemy, lalu menghadapi Guardian.

Fokus AI:

```text
Melee AI
+
Ranged AI
+
Maintain Distance
+
Flee / Retreat
+
Navigation
+
Guardian Behavior
```

Contoh environment:

```text
Fantasy Dungeon
Medieval Crypt
Castle Dungeon
Cave
Ruins
```

---

## TOPIK 4 — Zombie Extraction: Survive and Escape

Player mencari resource atau mengaktifkan objective, menghadapi zombie, lalu bertahan sampai dapat mencapai extraction zone.

Fokus AI:

```text
Wander
+
Seek / Pursue
+
Vision
+
Hearing
+
Memory
+
Separation / Group Behavior
```

Contoh environment:

```text
Abandoned City
Hospital
Destroyed Village
Industrial Zone
Laboratory
```

---

## TOPIK 5 — Robot Arena: Intelligent Combat Bots

Player bertarung melawan beberapa robot dengan role dan strategi berbeda dalam arena futuristik.

Fokus AI:

```text
FSM
+
Utility AI
+
Maintain Distance
+
Cover
+
Retreat
+
Tactical Decision
```

Contoh environment:

```text
Sci-Fi Arena
Robot Training Facility
Cyber Arena
Research Lab
```

---

## TOPIK 6 — MathGate Tactics: Risk & Battle

Player memilih salah satu dari beberapa **Math Gate** yang mengubah stat karakter menggunakan operasi matematika sekaligus memberikan **trade-off**, kemudian menghadapi enemy dengan role dan AI berbeda.

Sebelum memilih gate, player dapat melihat estimasi komposisi enemy, threat level, dan potential reward sehingga harus memperkirakan risiko serta menentukan build yang paling sesuai.

Contoh:

```text
ATK ×2
HP −30%
```

atau:

```text
DEF ×2
ATK −20%
```

Fokus gameplay dan AI:

```text
Math Gate
+
Player Build
+
Buff & Trade-off
+
Enemy Preview
+
Threat Estimation
+
Risk–Reward Decision
+
Role-Based Enemy AI
+
Tactical Battle
```

Contoh environment:

```text
Fantasy Arena
Sci-Fi Arena
Dungeon Arena
Stylized Battle Arena
```

---

# 9. AI Debug Mode — Wajib

Setiap project harus memiliki:

```text
AI DEBUG MODE
```

Contoh kontrol:

```text
F1
→ Toggle AI Debug
```

Debug Mode bertujuan agar proses keputusan AI dapat diamati.

## Debug Informasi

Minimal tampilkan beberapa data berikut:

```text
Current State
Current Target
Can See Player
Distance to Player
Current Destination
Current Path
Last Known Position
```

Jika menggunakan Utility AI:

```text
Attack Score
Cover Score
Flee Score
Selected Action
```

## Contoh Debug UI

```text
Enemy_01

State       : CHASE
Target      : Player
Can See     : TRUE
Distance    : 6.8
Destination : Player
Path        : ACTIVE
```

## Debug Visual

Gunakan Gizmos atau Debug Draw untuk menampilkan:

```text
Detection Radius
Field of View
Raycast
Path
Target
Waypoint
Last Known Position
Cover Point
Flank Position
```

Debug Mode merupakan bagian penting karena mahasiswa harus dapat menjelaskan:

>**Mengapa NPC mengambil keputusan tertentu?**  
---

# 10. Gameplay Minimum

Game wajib memiliki gameplay loop.

Contoh:

```text
START
  ↓
Explore / Fight / Defend
  ↓
AI Challenge
  ↓
Complete Objective
  ↓
WIN
```

dan kondisi gagal:

```text
Player mati
atau
Objective gagal
  ↓
LOSE
```

Contoh objective:
- mengambil artifact,
- mengambil data,
- survive beberapa wave,
- mencapai extraction,
- mempertahankan reactor,
- mengalahkan guardian,
- membuka final area.

---

# 11. Requirement Playable dan Polishing

Project harus dapat dimainkan tanpa perlu mengubah Inspector selama demo.

Pastikan:
- Player Controller bekerja,
- Camera nyaman,
- objective jelas,
- enemy dapat ditemukan,
- collision benar,
- tidak ada object jatuh/menembus lantai,
- tidak terdapat error merah pada Console,
- UI dapat dibaca,
- sound effect tidak mengganggu,
- lighting cukup baik,
- scene memiliki environment yang layak.

---

# 12. Video Demo Penjelasan dan Uji Coba

Setiap kelompok wajib membuat**video demo** yang menjelaskan dan menguji project.

Durasi yang disarankan:

```text
5–10 menit
```

Video bukan hanya gameplay montage.

Video harus memperlihatkan:

```text
Penjelasan
+
Demonstrasi
+
Pengujian AI
```

## Struktur Video Demo

### Bagian 1 — Perkenalan
Tampilkan:
- nama project,
- nama anggota,
- konsep game,
- topik yang dipilih.

### Bagian 2 — Gameplay dan Objective
Jelaskan:
- tujuan game,
- kontrol Player,
- win condition,
- lose condition,
- gameplay loop.

### Bagian 3 — Arsitektur AI
Jelaskan secara singkat:

```text
Perception
Memory
Decision
Movement
Navigation
Tactical Behavior
```

### Bagian 4 — Uji Perception
Demonstrasikan:
- Player di luar radius,
- Player di dalam radius,
- Player di luar FOV,
- Player di balik obstacle,
- Line of Sight bekerja.

### Bagian 5 — Uji Memory
Contoh:

```text
Player terlihat
      ↓
NPC Chase
      ↓
Player bersembunyi
      ↓
NPC menuju Last Known Position
```

### Bagian 6 — Uji Decision Making
Contoh:

```text
PATROL
   ↓
CHASE
   ↓
ATTACK
   ↓
SEARCH
   ↓
PATROL
```

Jika menggunakan Utility AI, tampilkan score.

### Bagian 7 — Uji Navigation / Movement
Demonstrasikan:
- NPC menuju target,
- NPC tidak menembus Wall,
- NPC menggunakan NavMesh/path,
- steering behavior terlihat.

### Bagian 8 — Uji Tactical Behavior
Tunjukkan minimal satu tactical behavior:
- cover,
- flank,
- maintain distance,
- shared alert,
- separation,
- target selection.

### Bagian 9 — AI Debug Mode
Aktifkan AI Debug Mode dan jelaskan:
- state,
- target,
- FOV,
- path,
- Last Known Position,
- tactical position,
- utility score jika digunakan.

### Bagian 10 — Kesimpulan
Jelaskan:
- AI yang berhasil dibuat,
- bagian tersulit,
- hasil pengujian,
- keterbatasan yang masih ada,
- kontribusi masing-masing anggota.

---

# 13. Pengujian Minimum Project

Sebelum membuat video, kelompok wajib melakukan pengujian.

```text
[ ] Player Controller berjalan
[ ] Perception bekerja
[ ] FOV bekerja
[ ] Obstacle menghalangi Line of Sight
[ ] Memory bekerja
[ ] State / Decision berubah sesuai kondisi
[ ] Navigation bekerja
[ ] NPC tidak menembus obstacle
[ ] Tactical Behavior bekerja
[ ] Win Condition bekerja
[ ] Lose Condition bekerja
[ ] AI Debug Mode bekerja
[ ] Console tidak memiliki error merah
```

---

# 14. Contoh Skenario Demo AI

Contoh Stealth Infiltration:

```text
NPC sedang PATROL
        ↓
Player masuk FOV
        ↓
NPC CHASE
        ↓
Player bersembunyi di balik Wall
        ↓
NPC kehilangan Line of Sight
        ↓
NPC menuju Last Known Position
        ↓
NPC SEARCH
        ↓
Player tidak ditemukan
        ↓
NPC kembali PATROL
```

Contoh Tactical Outpost:

```text
Player terlihat
        ↓
Enemy mengevaluasi posisi
        ↓
Enemy memilih Cover
        ↓
Enemy bergerak ke Cover
        ↓
Enemy Attack
        ↓
Enemy lain melakukan Flank
```

---

# 15. Kualitas Project yang Diharapkan

Project UTS yang baik memiliki:

```text
AI yang jelas
Gameplay yang jelas
Objective yang jelas
Environment yang layak
Visual yang konsisten
Debug yang jelas
Project stabil
```

Fokus utama:

>**AI harus memberikan dampak nyata terhadap gameplay.**  
---

# 16. Kriteria Keberhasilan

Project dianggap berhasil jika mahasiswa dapat menunjukkan dan menjelaskan:

### Perception
```text
Apa yang dapat dideteksi NPC?
```

### Memory
```text
Informasi apa yang disimpan NPC?
```

### Decision
```text
Mengapa NPC memilih behavior tertentu?
```

### Movement
```text
Bagaimana NPC bergerak?
```

### Navigation
```text
Bagaimana NPC mencapai target?
```

### Tactical AI
```text
Apa behavior taktis yang digunakan?
```

### Gameplay
```text
Bagaimana AI membuat game lebih menarik?
```

---

# 17. Ringkasan Tugas UTS

```text
UTS GAME CERDAS
|
+-- Kelompok Maksimal 3 Mahasiswa
+-- Unity 6 + C#
+-- Pilih 1 dari 6 Topik
+-- Playable Mini Game
+-- Environment & Asset yang Layak
+-- Perception
+-- Memory
+-- Steering / Movement
+-- Navigation
+-- Decision Making
+-- Tactical Behavior
+-- Win / Lose Condition
+-- AI Debug Mode
+-- Video Demo Penjelasan + Uji Coba
```

---

# 18. Enam Topik Pilihan

```text
1. Stealth Infiltration
   Intelligent Security Facility

2. Tactical Outpost Defense
   Intelligent Assault AI

3. Dungeon Hunter
   Intelligent Enemy Dungeon

4. Zombie Extraction
   Survive and Escape

5. Robot Arena
   Intelligent Combat Bots

6. MathGate Tactics
   Risk & Battle
```

Setiap kelompok memilih **satu topik**.

Penjelasan dan requirement detail masing-masing topik tersedia pada dokumen topik yang terpisah.

---

# 19. Pesan Utama

Project UTS tidak dinilai hanya dari seberapa banyak script yang dibuat.

Yang paling penting adalah:

```text
Apakah Game Playable?
        +
Apakah AI Terlihat?
        +
Apakah AI Bisa Dijelaskan?
        +
Apakah AI Memberikan Dampak pada Gameplay?
```

Target akhir:

>**Buatlah satu mini game kecil yang selesai, menarik, memiliki environment yang baik, dan menunjukkan integrasi Game AI Pertemuan 1–7 secara jelas.**
