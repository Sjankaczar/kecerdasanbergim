# MODUL PRAKTIKUM 4 — PATHFINDING & NAVIGATION
## Game Cerdas — S1 Teknik Informatika
### Unity 6 + C#

---

## 1. Identitas Praktikum

**Nama praktikum:**  
**Praktikum 4 — A* Grid Pathfinder & Unity NavMesh Navigation**

**Materi terkait:** Pertemuan 4 — Pathfinding & Navigation

**Topik utama:**
- Graph, node, edge, neighbor, cost
- Grid sebagai graph
- A* Search
- `g(n)`, `h(n)`, dan `f(n)`
- Reconstruct path
- Path following
- Unity AI Navigation
- `NavMeshSurface`
- `NavMeshAgent`
- `NavMeshObstacle`
- `NavMeshLink`
- Repathing dan debugging

---

# 2. Tujuan Praktikum

Setelah menyelesaikan praktikum ini, mahasiswa diharapkan mampu:

1. Merepresentasikan area game sebagai grid/graph.
2. Menentukan node walkable dan obstacle.
3. Menentukan neighbor dari sebuah node.
4. Mengimplementasikan A* secara manual.
5. Menjelaskan fungsi `gCost`, `hCost`, dan `fCost`.
6. Menyimpan parent node dan melakukan reconstruct path.
7. Memvisualisasikan hasil pathfinding di Unity.
8. Membuat agent mengikuti path hasil A*.
9. Menggunakan package AI Navigation pada Unity 6.
10. Membuat dan melakukan bake `NavMeshSurface`.
11. Menggerakkan NPC menggunakan `NavMeshAgent`.
12. Menggunakan `NavMeshObstacle` untuk obstacle dinamis.
13. Memahami perbedaan A* manual dan Unity NavMesh.
14. Melakukan debugging ketika path atau NavMesh tidak bekerja.

---

# 3. Hasil Akhir Praktikum

Praktikum dibagi menjadi dua bagian.

## Bagian A — A* Manual pada Grid

Hasil yang diharapkan:

```text
Start
  ↓
S . . . . .
. # # . . .
. . . . # .
. # . . # .
. . . . . G
              ↑
             Goal
```

Unity akan:

1. membangun grid,
2. mendeteksi obstacle,
3. menentukan start dan goal,
4. menjalankan A*,
5. menampilkan node path,
6. menggerakkan agent mengikuti path.

## Bagian B — Unity NavMesh

Scene sederhana:

```text
NPC ●
   \
    \       ███████
     \      █ Wall █
      \     ███████
       \____________ Player ●
```

NPC harus mencari jalur memutari obstacle menggunakan NavMesh.

---

# 4. Rekomendasi Bentuk Praktikum

## Pilihan yang direkomendasikan: A* Manual + NavMesh dalam satu praktikum

Urutan terbaik:

```text
A* Manual
    ↓
Pahami algoritma
    ↓
Visualisasi path
    ↓
Path following
    ↓
Unity NavMesh
    ↓
Bandingkan manual vs engine
```

Alasan:

- A* memberikan pemahaman algoritmik.
- NavMesh memberikan pengalaman implementasi game 3D yang praktis.
- Mahasiswa dapat melihat hubungan antara teori dan tool.
- Praktikum tidak berhenti pada penggunaan `SetDestination()` tanpa memahami pathfinding.
- Materi Pertemuan 4 dapat tercakup dengan seimbang.

---

# 5. Persiapan

## Software

- Unity 6
- Unity Hub
- Visual Studio Code / Visual Studio / Rider
- Package AI Navigation untuk Bagian B

## Nama Project yang Disarankan

```text
GameCerdas_Praktikum04_PathfindingNavigation
```

## Nama Scene

```text
P04_AStarGrid
P04_NavMesh
```

## Struktur Folder

Buat folder berikut di `Assets`:

```text
Assets/
├── Scenes/
├── Scripts/
│   ├── AStar/
│   └── NavMesh/
├── Materials/
├── Prefabs/
└── Models/
```

---

# BAGIAN A — IMPLEMENTASI A* MANUAL

# 6. Konsep Implementasi

Grid akan dianggap sebagai graph.

```text
Cell = Node

Hubungan antar-cell = Edge

Obstacle = Node tidak walkable
```

Untuk praktikum dasar, gunakan neighbor **4 arah**:

```text
        Atas
          ↑
Kiri ← Current → Kanan
          ↓
        Bawah
```

Keuntungan:
- lebih mudah dipahami,
- tidak ada masalah diagonal corner cutting,
- Manhattan Distance dapat digunakan sebagai heuristic.

Setiap perpindahan antarnode diberi cost:

```text
10
```

Penggunaan nilai 10 memudahkan jika praktikum nanti dikembangkan menjadi diagonal dengan cost 14.

---

# 7. Membuat Scene A*

1. Buat scene baru.
2. Simpan sebagai:

```text
Assets/Scenes/P04_AStarGrid.unity
```

3. Hapus objek yang tidak diperlukan.
4. Pertahankan `Main Camera`.
5. Buat Empty GameObject:

```text
AStarSystem
```

6. Buat Empty GameObject:

```text
Agent
```

7. Buat Empty GameObject:

```text
StartMarker
```

8. Buat Empty GameObject:

```text
GoalMarker
```

9. Buat beberapa Cube yang akan menjadi obstacle.

---

# 8. Membuat Layer Obstacle

Buat layer:

```text
Obstacle
```

Langkah:

1. Pilih salah satu Cube obstacle.
2. Pada Inspector pilih `Layer`.
3. Pilih `Add Layer...`.
4. Tambahkan:

```text
Obstacle
```

5. Kembali ke setiap Cube obstacle.
6. Set layer menjadi `Obstacle`.

Ini penting karena grid akan menggunakan physics query untuk menentukan apakah sebuah cell tertutup obstacle.

---

# 9. Menempatkan Obstacle

Gunakan ukuran grid:

```text
Width  = 10
Height = 10
Cell Size = 1
```

Contoh posisi obstacle:

```text
(3, 0.5, 2)
(3, 0.5, 3)
(3, 0.5, 4)
(6, 0.5, 5)
(6, 0.5, 6)
(7, 0.5, 6)
```

Set scale setiap Cube:

```text
X = 0.9
Y = 1
Z = 0.9
```

Pastikan Cube memiliki `BoxCollider`.

---

# 10. Script GridNode.cs

Buat:

```text
Assets/Scripts/AStar/GridNode.cs
```

Isi:

```csharp
using UnityEngine;

public class GridNode
{
    public int x;
    public int y;

    public Vector3 worldPosition;

    public bool walkable;

    public int gCost;
    public int hCost;

    public GridNode parent;

    public GameObject visual;

    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public GridNode(
        int x,
        int y,
        Vector3 worldPosition,
        bool walkable)
    {
        this.x = x;
        this.y = y;
        this.worldPosition = worldPosition;
        this.walkable = walkable;

        gCost = int.MaxValue;
        hCost = 0;
        parent = null;
    }
}
```

## Penjelasan

`x` dan `y`
: indeks node pada grid.

`worldPosition`
: posisi node di dunia Unity.

`walkable`
: apakah node dapat dilewati.

`gCost`
: cost dari start menuju node tersebut.

`hCost`
: estimasi cost dari node menuju goal.

`FCost`
: nilai:

```text
f(n) = g(n) + h(n)
```

`parent`
: node sebelumnya pada jalur terbaik.

`parent` digunakan untuk reconstruct path.

---

# 11. Script GridManager.cs

Buat:

```text
Assets/Scripts/AStar/GridManager.cs
```

Isi:

```csharp
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    [Header("Obstacle Detection")]
    public LayerMask obstacleMask;

    [Header("Visualization")]
    public bool showGrid = true;
    public float visualHeight = 0.05f;

    public GridNode[,] grid;

    private Transform visualParent;

    private void Awake()
    {
        CreateGrid();
    }

    public void CreateGrid()
    {
        grid = new GridNode[width, height];

        GameObject holder = new GameObject("GridVisuals");
        holder.transform.SetParent(transform);
        visualParent = holder.transform;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPosition = GetWorldPosition(x, y);

                bool blocked = Physics.CheckBox(
                    worldPosition + Vector3.up * 0.5f,
                    new Vector3(
                        cellSize * 0.4f,
                        0.45f,
                        cellSize * 0.4f),
                    Quaternion.identity,
                    obstacleMask
                );

                bool walkable = !blocked;

                GridNode node = new GridNode(
                    x,
                    y,
                    worldPosition,
                    walkable
                );

                grid[x, y] = node;

                if (showGrid)
                {
                    CreateVisual(node);
                }
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return transform.position +
               new Vector3(
                   x * cellSize,
                   0f,
                   y * cellSize
               );
    }

    private void CreateVisual(GridNode node)
    {
        GameObject tile =
            GameObject.CreatePrimitive(PrimitiveType.Cube);

        tile.name = $"Node_{node.x}_{node.y}";

        tile.transform.SetParent(visualParent);

        tile.transform.position =
            node.worldPosition +
            Vector3.down * (visualHeight * 0.5f);

        tile.transform.localScale =
            new Vector3(
                cellSize * 0.9f,
                visualHeight,
                cellSize * 0.9f
            );

        Collider col = tile.GetComponent<Collider>();
        if (col != null)
        {
            Destroy(col);
        }

        node.visual = tile;

        SetNodeColor(
            node,
            node.walkable ? Color.white : Color.black
        );
    }

    public GridNode NodeFromWorldPosition(Vector3 worldPosition)
    {
        Vector3 local =
            worldPosition - transform.position;

        int x = Mathf.RoundToInt(local.x / cellSize);
        int y = Mathf.RoundToInt(local.z / cellSize);

        x = Mathf.Clamp(x, 0, width - 1);
        y = Mathf.Clamp(y, 0, height - 1);

        return grid[x, y];
    }

    public List<GridNode> GetNeighbors(GridNode node)
    {
        List<GridNode> neighbors =
            new List<GridNode>();

        TryAddNeighbor(node.x + 1, node.y, neighbors);
        TryAddNeighbor(node.x - 1, node.y, neighbors);
        TryAddNeighbor(node.x, node.y + 1, neighbors);
        TryAddNeighbor(node.x, node.y - 1, neighbors);

        return neighbors;
    }

    private void TryAddNeighbor(
        int x,
        int y,
        List<GridNode> neighbors)
    {
        if (x < 0 || x >= width ||
            y < 0 || y >= height)
        {
            return;
        }

        neighbors.Add(grid[x, y]);
    }

    public void ResetSearchData()
    {
        foreach (GridNode node in grid)
        {
            node.gCost = int.MaxValue;
            node.hCost = 0;
            node.parent = null;

            if (showGrid)
            {
                SetNodeColor(
                    node,
                    node.walkable ?
                    Color.white :
                    Color.black
                );
            }
        }
    }

    public void SetNodeColor(
        GridNode node,
        Color color)
    {
        if (node.visual == null)
        {
            return;
        }

        Renderer renderer =
            node.visual.GetComponent<Renderer>();

        renderer.material.color = color;
    }
}
```

---

# 12. Memahami GridManager

## `CreateGrid()`

Membuat node:

```text
grid[x, y]
```

Setiap node dikonversi menjadi posisi dunia.

## `Physics.CheckBox()`

Digunakan untuk memeriksa apakah cell ditempati objek pada layer `Obstacle`.

Jika ada obstacle:

```text
walkable = false
```

Jika kosong:

```text
walkable = true
```

## `GetNeighbors()`

Pada praktikum ini neighbor hanya:

```text
(x+1, y)
(x-1, y)
(x, y+1)
(x, y-1)
```

---

# 13. Memasang GridManager

Pilih:

```text
AStarSystem
```

Tambahkan komponen:

```text
GridManager
```

Set:

```text
Width     = 10
Height    = 10
Cell Size = 1
```

Pada:

```text
Obstacle Mask
```

pilih:

```text
Obstacle
```

---

# 14. Mengatur Start dan Goal

Posisi yang disarankan:

```text
StartMarker = (0, 0.2, 0)
GoalMarker  = (9, 0.2, 9)
```

Agar mudah dilihat:

- gunakan Sphere untuk Start,
- gunakan Sphere untuk Goal,
- atau gunakan Empty GameObject dengan icon Scene.

Agent dapat diletakkan pada:

```text
(0, 0.5, 0)
```

---

# 15. Script AStarPathfinder.cs

Buat:

```text
Assets/Scripts/AStar/AStarPathfinder.cs
```

Isi:

```csharp
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : MonoBehaviour
{
    public GridManager gridManager;

    public Transform startMarker;
    public Transform goalMarker;

    [Header("Debug")]
    public bool showOpenClosed = true;

    public List<GridNode> currentPath =
        new List<GridNode>();

    private void Start()
    {
        FindPath();
    }

    [ContextMenu("Find Path")]
    public void FindPath()
    {
        if (gridManager == null ||
            startMarker == null ||
            goalMarker == null)
        {
            Debug.LogWarning(
                "GridManager, StartMarker, atau GoalMarker belum diisi."
            );
            return;
        }

        gridManager.ResetSearchData();

        GridNode startNode =
            gridManager.NodeFromWorldPosition(
                startMarker.position
            );

        GridNode goalNode =
            gridManager.NodeFromWorldPosition(
                goalMarker.position
            );

        if (!startNode.walkable)
        {
            Debug.LogWarning(
                "Start berada pada obstacle."
            );
            return;
        }

        if (!goalNode.walkable)
        {
            Debug.LogWarning(
                "Goal berada pada obstacle."
            );
            return;
        }

        List<GridNode> openSet =
            new List<GridNode>();

        HashSet<GridNode> closedSet =
            new HashSet<GridNode>();

        startNode.gCost = 0;
        startNode.hCost =
            GetHeuristic(startNode, goalNode);

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            GridNode currentNode =
                GetLowestFCostNode(openSet);

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (showOpenClosed)
            {
                gridManager.SetNodeColor(
                    currentNode,
                    new Color(1f, 0.6f, 0.2f)
                );
            }

            if (currentNode == goalNode)
            {
                currentPath =
                    ReconstructPath(
                        startNode,
                        goalNode
                    );

                VisualizeFinalPath(
                    startNode,
                    goalNode
                );

                Debug.Log(
                    $"Path ditemukan. Node path: {currentPath.Count}"
                );

                return;
            }

            foreach (
                GridNode neighbor
                in gridManager.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable)
                {
                    continue;
                }

                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                int tentativeGCost =
                    currentNode.gCost + 10;

                if (tentativeGCost <
                    neighbor.gCost)
                {
                    neighbor.parent =
                        currentNode;

                    neighbor.gCost =
                        tentativeGCost;

                    neighbor.hCost =
                        GetHeuristic(
                            neighbor,
                            goalNode
                        );

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }

                    if (showOpenClosed)
                    {
                        gridManager.SetNodeColor(
                            neighbor,
                            Color.yellow
                        );
                    }
                }
            }
        }

        currentPath.Clear();

        Debug.LogWarning(
            "Path tidak ditemukan."
        );
    }

    private GridNode GetLowestFCostNode(
        List<GridNode> openSet)
    {
        GridNode best = openSet[0];

        for (int i = 1;
             i < openSet.Count;
             i++)
        {
            GridNode candidate =
                openSet[i];

            if (candidate.FCost < best.FCost)
            {
                best = candidate;
            }
            else if (
                candidate.FCost == best.FCost &&
                candidate.hCost < best.hCost)
            {
                best = candidate;
            }
        }

        return best;
    }

    private int GetHeuristic(
        GridNode a,
        GridNode b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        return (dx + dy) * 10;
    }

    private List<GridNode> ReconstructPath(
        GridNode startNode,
        GridNode goalNode)
    {
        List<GridNode> path =
            new List<GridNode>();

        GridNode current =
            goalNode;

        while (current != null &&
               current != startNode)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Add(startNode);
        path.Reverse();

        return path;
    }

    private void VisualizeFinalPath(
        GridNode startNode,
        GridNode goalNode)
    {
        foreach (GridNode node in currentPath)
        {
            gridManager.SetNodeColor(
                node,
                Color.cyan
            );
        }

        gridManager.SetNodeColor(
            startNode,
            Color.green
        );

        gridManager.SetNodeColor(
            goalNode,
            Color.red
        );
    }
}
```

---

# 16. Memasang AStarPathfinder

Pada `AStarSystem`:

1. Add Component.
2. Pilih:

```text
AStarPathfinder
```

Assign:

```text
Grid Manager → AStarSystem
Start Marker → StartMarker
Goal Marker  → GoalMarker
```

---

# 17. Cara Kerja A*

A* menggunakan:

```text
f(n) = g(n) + h(n)
```

Pada script:

```csharp
public int FCost
{
    get { return gCost + hCost; }
}
```

## `gCost`

Biaya nyata dari start.

Pada praktikum:

```text
per langkah = 10
```

Sehingga:

```text
1 langkah = 10
2 langkah = 20
3 langkah = 30
```

## `hCost`

Estimasi cost menuju goal.

Karena movement hanya 4 arah, gunakan Manhattan Distance:

```text
h = |dx| + |dy|
```

Dalam script:

```csharp
return (dx + dy) * 10;
```

---

# 18. Open Set dan Closed Set

## Open Set

Berisi node yang sudah ditemukan tetapi belum selesai diperiksa.

```csharp
List<GridNode> openSet
```

## Closed Set

Node yang sudah selesai diproses.

```csharp
HashSet<GridNode> closedSet
```

Pada visualisasi:

```text
Putih   = walkable belum diperiksa
Hitam   = obstacle
Kuning  = open set
Oranye  = closed set
Cyan    = final path
Hijau   = start
Merah   = goal
```

---

# 19. Mengapa Open Set Tidak Menggunakan PriorityQueue?

Untuk praktikum awal, open set menggunakan:

```csharp
List<GridNode>
```

Kemudian node dengan `fCost` terkecil dicari secara manual.

Alasannya:

- algoritma lebih transparan,
- mahasiswa dapat melihat proses pemilihan node,
- tidak tergantung implementasi priority queue tertentu,
- lebih mudah untuk debugging.

Catatan:

Untuk project besar, pendekatan ini tidak efisien karena pencarian minimum dilakukan berulang.

Optimasi berikutnya dapat menggunakan:

- binary heap,
- priority queue,
- custom min-heap.

---

# 20. Uji A* Pertama

Tekan:

```text
Play
```

Periksa:

1. Node grid muncul.
2. Obstacle berwarna hitam.
3. Start berwarna hijau.
4. Goal berwarna merah.
5. Path berwarna cyan.
6. Path memutari obstacle.

Jika path menembus obstacle, periksa:

```text
Layer obstacle
Obstacle Mask
Collider
Posisi obstacle
```

---

# 21. Script AgentPathFollower.cs

Sekarang NPC akan mengikuti path hasil A*.

Buat:

```text
Assets/Scripts/AStar/AgentPathFollower.cs
```

Isi:

```csharp
using System.Collections.Generic;
using UnityEngine;

public class AgentPathFollower : MonoBehaviour
{
    public AStarPathfinder pathfinder;

    public float moveSpeed = 2f;
    public float rotationSpeed = 8f;
    public float waypointThreshold = 0.1f;

    private List<GridNode> path;
    private int currentIndex;

    private void Start()
    {
        if (pathfinder == null)
        {
            return;
        }

        path = pathfinder.currentPath;
        currentIndex = 0;
    }

    private void Update()
    {
        if (path == null ||
            path.Count == 0 ||
            currentIndex >= path.Count)
        {
            return;
        }

        Vector3 target =
            path[currentIndex].worldPosition;

        target.y = transform.position.y;

        Vector3 direction =
            target - transform.position;

        if (direction.magnitude <=
            waypointThreshold)
        {
            currentIndex++;
            return;
        }

        Vector3 moveDirection =
            direction.normalized;

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );

        if (moveDirection.sqrMagnitude >
            0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(
                    moveDirection,
                    Vector3.up
                );

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed *
                    Time.deltaTime
                );
        }
    }
}
```

---

# 22. Membuat Visual Agent

Agar mudah:

1. Pilih `Agent`.
2. Tambahkan child Capsule.
3. Nama:

```text
Body
```

4. Set Capsule local position:

```text
(0, 0.5, 0)
```

5. Posisi `Agent`:

```text
(0, 0, 0)
```

Tambahkan:

```text
AgentPathFollower
```

Assign:

```text
Pathfinder → AStarSystem
```

---

# 23. Menguji Path Following

Tekan Play.

Agent seharusnya:

```text
Start
  ↓
mengikuti node cyan
  ↓
memutari obstacle
  ↓
Goal
```

Perhatikan perbedaan:

```text
A*             = menghasilkan rute
PathFollower   = melakukan movement
```

Ini menunjukkan perbedaan antara:

```text
Pathfinding
dan
Navigation / Movement
```

---

# 24. Eksperimen A*

Lakukan beberapa eksperimen.

## Eksperimen 1 — Pindahkan Goal

Ubah posisi Goal:

```text
(9, 0.2, 2)
```

Jalankan kembali.

Pertanyaan:

- Apakah path berubah?
- Mengapa?

## Eksperimen 2 — Tambahkan Dinding

Tambahkan obstacle membentuk:

```text
# # # # #
```

Periksa apakah A* memutari dinding.

## Eksperimen 3 — Goal Terisolasi

Kelilingi Goal dengan obstacle.

Hasil yang diharapkan:

```text
Path tidak ditemukan.
```

Console:

```text
Path tidak ditemukan.
```

---

# 25. Pengembangan Opsional — Klik untuk Mengubah Goal

Buat:

```text
ClickGoalSetter.cs
```

```csharp
using UnityEngine;

public class ClickGoalSetter : MonoBehaviour
{
    public Camera mainCamera;
    public Transform goalMarker;
    public AStarPathfinder pathfinder;

    public LayerMask groundMask;

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        Ray ray =
            mainCamera.ScreenPointToRay(
                Input.mousePosition
            );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            100f,
            groundMask))
        {
            goalMarker.position =
                hit.point + Vector3.up * 0.2f;

            pathfinder.FindPath();
        }
    }
}
```

Catatan:

Untuk eksperimen ini diperlukan permukaan dengan Collider pada layer ground.

---

# 26. Keterbatasan Implementasi A* Praktikum

Implementasi ini sengaja dibuat sederhana.

Belum menangani:

- diagonal movement,
- terrain cost berbeda,
- moving obstacle,
- path smoothing,
- optimized heap,
- hierarchical pathfinding,
- multi-agent path planning.

Tujuan utamanya adalah membuat mekanisme A* terlihat dan mudah dipahami.

---

# BAGIAN B — UNITY NAVMESH

# 27. Tujuan Bagian NavMesh

Mahasiswa akan membuat NPC 3D yang:

1. bergerak menuju target,
2. memutari dinding,
3. berhenti dekat target,
4. menghitung ulang path ketika target bergerak,
5. menghadapi obstacle dinamis.

---

# 28. Membuat Scene NavMesh

Buat scene baru:

```text
Assets/Scenes/P04_NavMesh.unity
```

Buat:

```text
Ground
Wall_01
Wall_02
Wall_03
NPC
PlayerTarget
Navigation
Main Camera
Directional Light
```

---

# 29. Ground

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

Scale:

```text
X = 5
Y = 1
Z = 5
```

Plane default 10 x 10, sehingga area menjadi cukup luas untuk percobaan.

---

# 30. Membuat Dinding

Buat Cube.

Rename:

```text
Wall_01
```

Contoh:

```text
Position = (0, 1, 0)
Scale    = (2, 2, 8)
```

Tambahkan dua dinding lain untuk membuat rute tidak lurus.

Contoh:

```text
Wall_02
Position = (-5, 1, 3)
Scale    = (5, 2, 1)

Wall_03
Position = (5, 1, -3)
Scale    = (5, 2, 1)
```

Pastikan masih ada jalur yang bisa dilalui.

---

# 31. Install AI Navigation

Buka:

```text
Window
→ Package Manager
```

Cari:

```text
AI Navigation
```

Install package yang kompatibel dengan Unity 6 yang digunakan.

Setelah terpasang, komponen navigation seperti berikut tersedia:

```text
NavMesh Surface
NavMesh Agent
NavMesh Obstacle
NavMesh Link
NavMesh Modifier
```

---

# 32. Membuat NavMeshSurface

Pilih:

```text
Navigation
```

Tambahkan:

```text
NavMesh Surface
```

Untuk praktikum dasar, gunakan pengaturan yang sederhana.

Contoh:

```text
Agent Type      = Humanoid
Collect Objects = All Game Objects
Use Geometry    = Physics Colliders
```

Kemudian klik:

```text
Bake
```

Scene View akan memperlihatkan area navigasi.

Area walkable harus mengelilingi dinding.

---

# 33. Jika NavMesh Tidak Terlihat

Aktifkan visualisasi AI Navigation pada Scene View.

Periksa:

```text
Show NavMesh
```

Jika masih tidak muncul, periksa:

1. `NavMeshSurface` sudah di-Bake.
2. Ground aktif.
3. Ground memiliki geometry/collider sesuai opsi `Use Geometry`.
4. Agent Type benar.
5. Surface aktif.
6. Area tidak terlalu sempit untuk radius agent.

---

# 34. Membuat NPC

Buat:

```text
GameObject
→ 3D Object
→ Capsule
```

Rename:

```text
NPC
```

Contoh posisi:

```text
(-8, 1, -8)
```

Tambahkan:

```text
NavMesh Agent
```

---

# 35. Parameter NavMeshAgent

Gunakan nilai awal:

```text
Speed             = 3.5
Angular Speed     = 180
Acceleration      = 8
Stopping Distance = 1.5
Radius            = 0.5
Height            = 2
Auto Braking      = aktif
```

Nilai dapat disesuaikan dengan skala scene.

## Speed

Kecepatan maksimum agent.

## Angular Speed

Kecepatan rotasi.

## Acceleration

Seberapa cepat agent mencapai kecepatan geraknya.

## Stopping Distance

Jarak agent berhenti dari destination.

## Radius

Lebar ruang yang dibutuhkan agent.

## Height

Tinggi agent.

---

# 36. Membuat PlayerTarget

Buat Sphere.

Rename:

```text
PlayerTarget
```

Contoh posisi:

```text
(8, 0.5, 8)
```

Target tidak harus memiliki `NavMeshAgent`.

---

# 37. Script NavMeshChaser.cs

Buat:

```text
Assets/Scripts/NavMesh/NavMeshChaser.cs
```

Isi:

```csharp
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshChaser : MonoBehaviour
{
    public Transform target;

    [Header("Repathing")]
    public float repathInterval = 0.25f;
    public float targetMoveThreshold = 0.5f;

    private NavMeshAgent agent;

    private float nextRepathTime;

    private Vector3 lastTargetPosition;

    private void Awake()
    {
        agent =
            GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (target != null)
        {
            lastTargetPosition =
                target.position;

            UpdateDestination();
        }
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        if (Time.time < nextRepathTime)
        {
            return;
        }

        float targetMoved =
            Vector3.Distance(
                lastTargetPosition,
                target.position
            );

        if (targetMoved >=
            targetMoveThreshold)
        {
            UpdateDestination();
        }

        nextRepathTime =
            Time.time + repathInterval;
    }

    private void UpdateDestination()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning(
                "NPC tidak berada di atas NavMesh."
            );
            return;
        }

        agent.SetDestination(
            target.position
        );

        lastTargetPosition =
            target.position;
    }
}
```

---

# 38. Memasang NavMeshChaser

Pilih:

```text
NPC
```

Tambahkan:

```text
NavMeshChaser
```

Assign:

```text
Target → PlayerTarget
```

Tekan Play.

NPC harus:

```text
NPC
 ↓
menghitung path
 ↓
memutari dinding
 ↓
mendekati target
 ↓
berhenti pada Stopping Distance
```

---

# 39. Mengapa Tidak Memanggil SetDestination Setiap Frame?

Implementasi paling sederhana memang dapat melakukan:

```csharp
agent.SetDestination(target.position);
```

di setiap `Update()`.

Namun praktikum menggunakan mekanisme:

```text
repath interval
+
target move threshold
```

agar mahasiswa memahami bahwa target bergerak tidak selalu berarti path perlu diminta ulang tanpa kontrol pada setiap frame.

Ini juga mempersiapkan konsep optimasi ketika jumlah NPC bertambah.

---

# 40. Script PlayerTargetMovement.cs

Agar target dapat digerakkan dengan keyboard:

```text
Assets/Scripts/NavMesh/PlayerTargetMovement.cs
```

Isi:

```csharp
using UnityEngine;

public class PlayerTargetMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private void Update()
    {
        float horizontal =
            Input.GetAxisRaw("Horizontal");

        float vertical =
            Input.GetAxisRaw("Vertical");

        Vector3 direction =
            new Vector3(
                horizontal,
                0f,
                vertical
            ).normalized;

        transform.position +=
            direction *
            moveSpeed *
            Time.deltaTime;
    }
}
```

Tambahkan pada:

```text
PlayerTarget
```

Kontrol:

```text
W / Up Arrow    = maju
S / Down Arrow  = mundur
A / Left Arrow  = kiri
D / Right Arrow = kanan
```

Catatan:

Script ini menggunakan Input Manager klasik. Jika project dikonfigurasi hanya untuk New Input System, atur `Active Input Handling` agar sesuai atau gunakan controller berbasis Input System.

---

# 41. Menguji Target Bergerak

Tekan Play.

Gerakkan `PlayerTarget`.

Amati:

```text
Target bergerak
      ↓
jarak perubahan diperiksa
      ↓
SetDestination dipanggil lagi
      ↓
path berubah
      ↓
NPC mengikuti path baru
```

Ini adalah contoh:

```text
Repathing
```

---

# 42. Melihat Path NavMeshAgent

Buat:

```text
NavMeshPathDebugger.cs
```

Isi:

```csharp
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshPathDebugger : MonoBehaviour
{
    private NavMeshAgent agent;

    public Color lineColor = Color.cyan;

    private void Awake()
    {
        agent =
            GetComponent<NavMeshAgent>();
    }

    private void OnDrawGizmos()
    {
        if (agent == null)
        {
            agent =
                GetComponent<NavMeshAgent>();
        }

        if (agent == null ||
            !agent.hasPath)
        {
            return;
        }

        Vector3[] corners =
            agent.path.corners;

        Gizmos.color = lineColor;

        for (int i = 0;
             i < corners.Length - 1;
             i++)
        {
            Gizmos.DrawLine(
                corners[i],
                corners[i + 1]
            );

            Gizmos.DrawSphere(
                corners[i],
                0.12f
            );
        }

        if (corners.Length > 0)
        {
            Gizmos.DrawSphere(
                corners[corners.Length - 1],
                0.12f
            );
        }
    }
}
```

Tambahkan pada NPC.

Sekarang mahasiswa dapat melihat:

```text
corner 0
   ↓
corner 1
   ↓
corner 2
   ↓
destination
```

---

# 43. Memeriksa Status Path

`NavMeshAgent` menyediakan informasi penting:

```text
agent.hasPath
agent.pathPending
agent.remainingDistance
agent.pathStatus
agent.velocity
agent.isOnNavMesh
```

Contoh debugging:

```csharp
Debug.Log(
    $"Pending: {agent.pathPending}, " +
    $"Has Path: {agent.hasPath}, " +
    $"Remaining: {agent.remainingDistance}, " +
    $"Status: {agent.pathStatus}"
);
```

---

# 44. NavMeshObstacle

Tambahkan Cube baru:

```text
DynamicObstacle
```

Tambahkan:

```text
NavMesh Obstacle
```

Set Shape:

```text
Box
```

Aktifkan:

```text
Carve
```

Dengan carving, obstacle dapat membuat area sementara yang tidak dapat dilalui sehingga pathfinding dapat merencanakan rute di sekitarnya ketika kondisi carving berlaku.

---

# 45. Eksperimen NavMeshObstacle

Letakkan obstacle di jalur NPC.

Bandingkan:

## Carve OFF

NPC terutama menggunakan local obstacle avoidance.

## Carve ON

Pathfinding dapat memperhitungkan lubang/area yang di-carve pada NavMesh.

Pertanyaan:

- Apakah jalur agent berubah?
- Kapan obstacle avoidance cukup?
- Kapan path harus benar-benar berubah?

---

# 46. NavMeshLink

Untuk Unity 6 dengan AI Navigation modern, gunakan:

```text
NavMesh Link
```

NavMesh Link digunakan untuk menghubungkan area navigasi yang tidak memiliki permukaan walkable kontinu.

Contoh:

```text
Platform A            Platform B
──────────            ──────────
     ●  ============> ●
          NavMeshLink
```

Contoh penggunaan:

- melompat parit,
- turun dari ledge,
- melewati gap,
- koneksi khusus antarpermukaan,
- aksi pintu tertentu.

---

# 47. Eksperimen NavMeshLink Opsional

Buat dua platform terpisah.

Bake NavMesh.

Tanpa Link:

```text
Platform A     gap     Platform B

NPC tidak memiliki koneksi.
```

Tambahkan:

```text
NavMesh Link
```

Atur endpoint link agar berada pada dua area NavMesh.

Pastikan link aktif dan area link termasuk dalam Area Mask agent.

---

# 48. Area Cost

NavMesh dapat menggunakan area dengan biaya berbeda.

Contoh konsep:

```text
Road     cost rendah
Mud      cost lebih tinggi
Danger   cost tinggi
```

Secara konseptual ini sama dengan weighted graph:

```text
edge / area memiliki cost
```

Agent dapat memilih rute yang lebih panjang secara geometris jika total cost lebih murah.

---

# 49. Hubungan A* Manual dan NavMesh

## A* Manual

Developer melihat langsung:

```text
Node
Neighbor
gCost
hCost
fCost
Open Set
Closed Set
Parent
Reconstruct Path
```

## Unity NavMesh

Developer bekerja pada abstraksi yang lebih tinggi:

```text
Bake walkable area
        ↓
Set destination
        ↓
Path dihitung
        ↓
Agent mengikuti corners
        ↓
Local avoidance
```

---

# 50. Perbandingan Praktis

| Aspek | A* Manual | Unity NavMesh |
|---|---|---|
| Representasi | Grid | Polygon NavMesh |
| Algoritma terlihat | Ya | Detail internal diabstraksikan |
| Cocok belajar | Sangat baik | Baik untuk implementasi |
| Setup | Lebih banyak kode | Lebih cepat |
| Kontrol algoritma | Tinggi | Lebih otomatis |
| 3D environment | Perlu banyak pengembangan | Sangat cocok |
| Debug algoritma | Dapat dibuat sangat detail | Debug surface/path tersedia |
| Production prototyping | Lebih lambat | Cepat |

---

# 51. Integrasi dengan Praktikum Sebelumnya

Urutan kemampuan NPC:

```text
Praktikum 1
Perception
    ↓
NPC mendeteksi player

Praktikum 2
Memory + Decision
    ↓
NPC memilih Patrol / Chase / Investigate

Praktikum 3
Movement / Steering
    ↓
NPC bergerak secara lokal

Praktikum 4
Pathfinding + Navigation
    ↓
NPC dapat memilih jalan memutari obstacle
```

Integrasi akhirnya:

```text
Detect Player
      ↓
Remember Target
      ↓
Decide Chase
      ↓
Find / Update Path
      ↓
Follow Path
      ↓
Arrive Near Target
```

---

# 52. Tugas Wajib Mahasiswa

Mahasiswa diminta menyerahkan:

## Bagian A

1. Screenshot grid awal.
2. Screenshot obstacle.
3. Screenshot open/closed/final path.
4. Screenshot agent mengikuti path.
5. Source code:
   - `GridNode.cs`
   - `GridManager.cs`
   - `AStarPathfinder.cs`
   - `AgentPathFollower.cs`

## Bagian B

1. Screenshot hasil Bake NavMesh.
2. Screenshot NPC dan target.
3. Screenshot path yang memutari obstacle.
4. Screenshot eksperimen `NavMeshObstacle`.
5. Source code:
   - `NavMeshChaser.cs`
   - `PlayerTargetMovement.cs`
   - `NavMeshPathDebugger.cs`

---

# 53. Tugas Analisis

Jawab singkat:

1. Mengapa Seek saja tidak cukup untuk scene yang memiliki dinding besar?
2. Apa yang direpresentasikan oleh node pada grid?
3. Apa fungsi `gCost`?
4. Apa fungsi `hCost`?
5. Mengapa `fCost = gCost + hCost`?
6. Mengapa Manhattan Distance cocok untuk grid 4 arah?
7. Apa fungsi `parent` pada `GridNode`?
8. Mengapa hasil reconstruct path perlu dibalik?
9. Apa perbedaan open set dan closed set?
10. Apa perbedaan pathfinding dan path following?
11. Apa fungsi `NavMeshSurface`?
12. Apa fungsi `NavMeshAgent`?
13. Apa fungsi `Stopping Distance`?
14. Mengapa agent radius dapat menyebabkan lorong menjadi tidak dapat dilalui?
15. Apa perbedaan `NavMeshObstacle` dan collider biasa dalam konteks navigation?
16. Apa fungsi carving?
17. Kapan `NavMeshLink` diperlukan?
18. Mengapa path tidak sebaiknya dihitung ulang tanpa kontrol untuk semua NPC setiap frame?

---

# 54. Tantangan Tambahan

## Challenge 1 — Diagonal A*

Tambahkan neighbor diagonal.

Cost:

```text
lurus   = 10
diagonal = 14
```

Kemudian gunakan heuristic yang sesuai.

## Challenge 2 — Terrain Cost

Buat:

```text
Normal = cost 10
Mud    = cost 30
Road   = cost 5
```

Amati apakah A* memilih jalur yang geometris lebih panjang tetapi lebih murah.

## Challenge 3 — Click Destination

Klik pada ground untuk mengubah tujuan agent.

## Challenge 4 — Dynamic Obstacle

Gerakkan obstacle dan amati repathing.

## Challenge 5 — Multiple Agents

Tambahkan beberapa `NavMeshAgent`.

Amati local avoidance.

---

# 55. Kesalahan Umum — A*

## Grid tidak muncul

Periksa:

```text
GridManager terpasang
Width > 0
Height > 0
Show Grid aktif
```

## Obstacle tidak terdeteksi

Periksa:

```text
Obstacle menggunakan layer Obstacle
ObstacleMask berisi Obstacle
Obstacle memiliki Collider
Obstacle berada pada cell yang benar
```

## Path menembus obstacle

Kemungkinan:

```text
layer salah
mask salah
collider tidak aktif
CheckBox tidak mengenai obstacle
```

## Path tidak ditemukan

Periksa:

```text
Start tidak terblokir
Goal tidak terblokir
Masih ada koneksi antar-node walkable
```

## Agent tidak bergerak

Periksa:

```text
AStarPathfinder menemukan path
AgentPathFollower memiliki reference pathfinder
Agent mulai dekat Start
Move Speed > 0
```

---

# 56. Kesalahan Umum — NavMesh

## NPC diam

Periksa:

```text
NavMesh sudah di-Bake
NPC berada di atas NavMesh
NavMeshAgent aktif
Target sudah di-assign
Speed > 0
```

## Pesan "NPC tidak berada di atas NavMesh"

Pindahkan NPC sedikit ke area biru/walkable atau perbaiki Bake.

## Agent tidak bisa melewati pintu

Periksa:

```text
Agent Radius
ukuran pintu
voxel/bake resolution
geometry
```

## Agent menabrak / tidak memutari obstacle besar

Periksa:

```text
Obstacle masuk ke Bake
atau
NavMeshObstacle + Carve sesuai kebutuhan
```

## Link tidak digunakan

Periksa:

```text
endpoint menyentuh / terhubung ke NavMesh
link aktif
Area Mask agent mengizinkan area link
```

---

# 57. Checklist Keberhasilan

- [ ] Project Unity 6 dapat dibuka tanpa compile error.
- [ ] Scene `P04_AStarGrid` tersedia.
- [ ] Grid berhasil dibuat.
- [ ] Obstacle terdeteksi sebagai non-walkable.
- [ ] Start dan Goal dapat ditentukan.
- [ ] A* menghasilkan path.
- [ ] Final path divisualisasikan.
- [ ] Agent dapat mengikuti path.
- [ ] Scene `P04_NavMesh` tersedia.
- [ ] AI Navigation terpasang.
- [ ] `NavMeshSurface` berhasil di-Bake.
- [ ] NPC memiliki `NavMeshAgent`.
- [ ] NPC menuju target.
- [ ] NPC memutari obstacle.
- [ ] Repathing bekerja saat target bergerak.
- [ ] Path corners dapat divisualisasikan.
- [ ] Eksperimen `NavMeshObstacle` dilakukan.
- [ ] Mahasiswa dapat menjelaskan perbedaan A* dan NavMesh.

---

# 58. Rekomendasi Penilaian

| Komponen | Bobot |
|---|---:|
| Grid dan obstacle | 10% |
| Implementasi A* | 25% |
| Visualisasi path | 10% |
| Path following | 10% |
| NavMesh setup dan Bake | 10% |
| NavMeshAgent navigation | 15% |
| Dynamic obstacle / eksperimen | 10% |
| Analisis konsep | 10% |
| **Total** | **100%** |

---

# 59. Rekomendasi Pembelajaran

Praktikum sebaiknya jangan dimulai langsung dari `NavMeshAgent.SetDestination()`.

Urutan demonstrasi yang disarankan:

```text
1. Tunjukkan Seek menabrak / tertahan dinding.
2. Gambarkan grid sebagai graph.
3. Jelaskan node dan neighbor.
4. Tunjukkan g, h, dan f.
5. Jalankan visualisasi A*.
6. Tunjukkan reconstruct path.
7. Gerakkan agent mengikuti path.
8. Baru buka scene NavMesh.
9. Bake NavMesh.
10. Jalankan NavMeshAgent.
11. Bandingkan kedua pendekatan.
12. Tambahkan obstacle dinamis dan repathing.
```

Dengan urutan tersebut, mahasiswa melihat bahwa:

```text
SetDestination()
```

bukanlah konsep pathfinding itu sendiri, melainkan API tingkat tinggi untuk meminta sistem navigasi Unity membawa agent menuju tujuan.

---

# 60. Pilihan Praktikum Terbaik

## Rekomendasi Utama

Gunakan:

```text
Bagian A — A* Grid 4 arah
+
Bagian B — Unity NavMesh
```

dan jadikan **NavMeshObstacle serta NavMeshLink sebagai eksperimen lanjutan**, bukan syarat inti pertama.

### Mengapa?

A* Grid 4 arah memberikan keseimbangan terbaik antara:

- algoritma yang cukup lengkap,
- kode yang masih dapat dipahami mahasiswa,
- debugging yang mudah,
- visualisasi yang jelas,
- hubungan langsung dengan graph dan heuristic.

NavMesh kemudian memberikan konteks nyata bagaimana teknik navigation digunakan pada game 3D Unity.

### Yang tidak saya rekomendasikan sebagai praktikum utama

**Hanya NavMeshAgent**

Karena mahasiswa berisiko hanya melakukan:

```csharp
agent.SetDestination(target.position);
```

tanpa benar-benar memahami pathfinding.

**A* 8 arah + terrain cost + smoothing sekaligus**

Terlalu banyak konsep untuk praktikum inti pertama A* dan dapat mengalihkan perhatian dari konsep utama.

---

# 61. Target Minimum dan Target Ideal

## Target Minimum

Mahasiswa harus berhasil:

```text
A* menemukan path
+
agent mengikuti path
+
NavMeshAgent memutari obstacle
```

## Target Ideal

Mahasiswa juga berhasil:

```text
target bergerak
+
repathing
+
dynamic obstacle
+
path debugging
```

## Target Bonus

```text
diagonal A*
terrain cost
NavMeshLink
multi-agent
```

---

# 62. Kesimpulan

Praktikum 4 menghubungkan tiga lapisan penting AI agent:

```text
Pathfinding
    ↓
menentukan jalur

Navigation
    ↓
mengikuti jalur

Movement
    ↓
menghasilkan gerakan aktual
```

A* manual memberikan pemahaman tentang:

```text
graph
node
neighbor
cost
heuristic
open set
closed set
parent
path reconstruction
```

Unity NavMesh memberikan implementasi praktis:

```text
NavMeshSurface
NavMeshAgent
NavMeshObstacle
NavMeshLink
SetDestination
repathing
local avoidance
```

Dengan menyelesaikan kedua bagian, mahasiswa tidak hanya dapat menggunakan navigation tool Unity, tetapi juga memahami konsep algoritmik yang menjadi dasar perencanaan jalur NPC.
