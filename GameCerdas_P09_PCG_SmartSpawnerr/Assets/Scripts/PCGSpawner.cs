using System.Collections.Generic;
using UnityEngine;

public class PCGSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEntry
    {
        public string name;
        public GameObject prefab;

        [Min(0)]
        public int weight = 1;
    }

    [Header("Seed")]
    [SerializeField]
    private bool useRandomSeed = false;

    [SerializeField]
    private int seed = 12345;


    [Header("References")]
    [SerializeField]
    private Transform player;

    [SerializeField]
    private Transform generatedRoot;


    [Header("Spawn Area")]
    [SerializeField]
    private Vector2 spawnAreaSize =
        new Vector2(24f, 24f);

    [SerializeField]
    private float spawnY = 0.5f;


    [Header("Spawn Counts")]
    [SerializeField]
    private int enemyCount = 12;

    [SerializeField]
    private int itemCount = 8;


    [Header("Validation")]
    [SerializeField]
    private float minDistanceFromPlayer = 5f;

    [SerializeField]
    private float checkRadius = 0.75f;

    [SerializeField]
    private float minimumSpacing = 1.5f;

    [SerializeField]
    private LayerMask obstacleMask;

    [SerializeField]
    private int maxAttemptsPerObject = 50;


    [Header("Enemy Weighted Table")]
    [SerializeField]
    private SpawnEntry[] enemyEntries;


    [Header("Item Weighted Table")]
    [SerializeField]
    private SpawnEntry[] itemEntries;


    [Header("Runtime Debug")]
    [SerializeField]
    private int currentSeed;

    [SerializeField]
    private int successfulEnemySpawns;

    [SerializeField]
    private int successfulItemSpawns;

    [SerializeField]
    private int rejectedPositions;


    private readonly List<Vector3> usedPositions =
        new List<Vector3>();


    private void Start()
    {
        Generate();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Generate();
        }
    }


    public void Generate()
    {
        ClearGeneratedObjects();

        usedPositions.Clear();

        successfulEnemySpawns = 0;
        successfulItemSpawns = 0;
        rejectedPositions = 0;


        if (useRandomSeed)
        {
            seed = System.Environment.TickCount;
        }

        currentSeed = seed;

        Random.InitState(currentSeed);


        SpawnGroup(
            enemyEntries,
            enemyCount,
            true
        );

        SpawnGroup(
            itemEntries,
            itemCount,
            false
        );


        Debug.Log(
            $"PCG Generation Complete | " +
            $"Seed: {currentSeed} | " +
            $"Enemies: {successfulEnemySpawns}/{enemyCount} | " +
            $"Items: {successfulItemSpawns}/{itemCount} | " +
            $"Rejected Positions: {rejectedPositions}"
        );
    }


    private void SpawnGroup(
        SpawnEntry[] entries,
        int count,
        bool enemyGroup)
    {
        if (entries == null || entries.Length == 0)
        {
            Debug.LogWarning(
                "Spawn table kosong."
            );

            return;
        }


        for (int i = 0; i < count; i++)
        {
            bool spawned = false;


            for (
                int attempt = 0;
                attempt < maxAttemptsPerObject;
                attempt++)
            {
                Vector3 candidate =
                    GenerateRandomPosition();


                if (!IsPositionValid(candidate))
                {
                    rejectedPositions++;
                    continue;
                }


                GameObject selectedPrefab =
                    SelectWeightedPrefab(entries);


                if (selectedPrefab == null)
                {
                    Debug.LogWarning(
                        "Tidak ada prefab valid pada weighted table."
                    );

                    return;
                }


                GameObject spawnedObject =
                    Instantiate(
                        selectedPrefab,
                        candidate,
                        Quaternion.identity,
                        generatedRoot
                    );


                spawnedObject.name =
                    $"{selectedPrefab.name}_Generated_{i}";


                usedPositions.Add(candidate);


                if (enemyGroup)
                {
                    successfulEnemySpawns++;
                }
                else
                {
                    successfulItemSpawns++;
                }


                spawned = true;
                break;
            }


            if (!spawned)
            {
                Debug.LogWarning(
                    $"Gagal spawn object ke-{i}. " +
                    $"Tidak menemukan posisi valid setelah " +
                    $"{maxAttemptsPerObject} percobaan."
                );
            }
        }
    }


    private Vector3 GenerateRandomPosition()
    {
        float halfWidth =
            spawnAreaSize.x * 0.5f;

        float halfDepth =
            spawnAreaSize.y * 0.5f;


        float randomX =
            Random.Range(-halfWidth, halfWidth);

        float randomZ =
            Random.Range(-halfDepth, halfDepth);


        return transform.position +
            new Vector3(
                randomX,
                spawnY,
                randomZ
            );
    }


    private bool IsPositionValid(
        Vector3 candidate)
    {
        // Rule 1:
        // Jangan spawn terlalu dekat dengan player.
        if (player != null)
        {
            Vector3 playerFlat =
                new Vector3(
                    player.position.x,
                    candidate.y,
                    player.position.z
                );


            float distanceToPlayer =
                Vector3.Distance(
                    candidate,
                    playerFlat
                );


            if (distanceToPlayer <
                minDistanceFromPlayer)
            {
                return false;
            }
        }


        // Rule 2:
        // Jangan spawn di dalam obstacle.
        bool blocked =
            Physics.CheckSphere(
                candidate,
                checkRadius,
                obstacleMask,
                QueryTriggerInteraction.Ignore
            );


        if (blocked)
        {
            return false;
        }


        // Rule 3:
        // Jangan terlalu dekat dengan
        // procedural object sebelumnya.
        for (
            int i = 0;
            i < usedPositions.Count;
            i++)
        {
            float distance =
                Vector3.Distance(
                    candidate,
                    usedPositions[i]
                );


            if (distance < minimumSpacing)
            {
                return false;
            }
        }


        return true;
    }


    private GameObject SelectWeightedPrefab(
        SpawnEntry[] entries)
    {
        int totalWeight = 0;


        foreach (SpawnEntry entry in entries)
        {
            if (
                entry.prefab != null &&
                entry.weight > 0)
            {
                totalWeight += entry.weight;
            }
        }


        if (totalWeight <= 0)
        {
            return null;
        }


        int randomValue =
            Random.Range(0, totalWeight);


        int cumulativeWeight = 0;


        foreach (SpawnEntry entry in entries)
        {
            if (
                entry.prefab == null ||
                entry.weight <= 0)
            {
                continue;
            }


            cumulativeWeight +=
                entry.weight;


            if (
                randomValue <
                cumulativeWeight)
            {
                return entry.prefab;
            }
        }


        return null;
    }


    private void ClearGeneratedObjects()
    {
        if (generatedRoot == null)
        {
            return;
        }


        for (
            int i =
                generatedRoot.childCount - 1;
            i >= 0;
            i--)
        {
            GameObject child =
                generatedRoot
                    .GetChild(i)
                    .gameObject;

            child.SetActive(false);

            Destroy(child);
        }
    }


    private void OnGUI()
    {
        GUI.skin.box.fontSize = 30;
        GUI.skin.label.fontSize = 36;

        GUI.Box(
            new Rect(10, 10, 500, 320),
            "PCG Debug"
        );

        GUI.Label(
            new Rect(30, 65, 450, 40),
            $"Seed: {currentSeed}"
        );

        GUI.Label(
            new Rect(30, 110, 450, 40),
            $"Enemy: {successfulEnemySpawns}/{enemyCount}"
        );

        GUI.Label(
            new Rect(30, 155, 450, 40),
            $"Item: {successfulItemSpawns}/{itemCount}"
        );

        GUI.Label(
            new Rect(30, 200, 450, 40),
            $"Rejected: {rejectedPositions}"
        );

        GUI.Label(
            new Rect(30, 245, 450, 40),
            "Press R to regenerate"
        );
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;


        Vector3 center =
            transform.position +
            Vector3.up * spawnY;


        Vector3 size =
            new Vector3(
                spawnAreaSize.x,
                0.1f,
                spawnAreaSize.y
            );


        Gizmos.DrawWireCube(
            center,
            size
        );


        if (player != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(
                player.position,
                minDistanceFromPlayer
            );
        }
    }
}