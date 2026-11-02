
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorInteraction : MonoBehaviour
{
    [Header("Named Door Lookup")]
    [Tooltip("Cari dua objek pintu dengan nama yang ditentukan saat scene mulai.")]
    public bool findDoorsByName;
    public string frontDoorName = "bld_sml_wall_door_04";
    public string backDoorName = "bld_sml_wall_door_04 1";
    public string[] additionalObjectNamesToRemove = new string[0];

    public Transform player;
    public StealableExperiment potion;

    public GameObject frontDoor;
    public GameObject backDoor;
    public GameObject[] additionalObjectsToRemove = new GameObject[0];

    [Header("Wall Objects")]
    public GameObject[] blockingWalls;

    public float interactionDistance = 3f;

    private bool doorsRemoved = false;
    private bool missingTargetsReported;
    private bool wasShowingDoorPrompt;
    private readonly string[] requiredRemovalNames =
    {
        "bld_sml_wall_door_04",
        "bld_sml_wall_door_04 1",
        "Box008"
    };
    private ProtocolZeroGameplayHudToolkit gameplayHud;

    public void Configure(Transform playerTransform, StealableExperiment experiment, ProtocolZeroGameplayHudToolkit hud)
    {
        player = playerTransform;
        potion = experiment;
        gameplayHud = hud;
        findDoorsByName = true;
        frontDoorName = "bld_sml_wall_door_04";
        backDoorName = "bld_sml_wall_door_04 1";
        additionalObjectNamesToRemove = new[] { "Box008" };
        ResolveNamedObjects();
    }

    void Awake()
    {
        if (findDoorsByName) ResolveNamedObjects();
    }

    void Update()
    {
        if (doorsRemoved)
            return;

        if (player == null || potion == null)
        {
            if (player == null) player = FindAnyObjectByType<PlayerFirstPersonController>()?.transform;
            if (potion == null) potion = FindAnyObjectByType<StealableExperiment>();
        }

        if (frontDoor == null || backDoor == null)
            ResolveNamedObjects();

        if (player == null || potion == null)
            return;

        if (frontDoor == null && backDoor == null)
            return;

        bool playing = GameManager.Instance == null || GameManager.Instance.CurrentGameState == GameState.Playing;
        float distance = float.MaxValue;
        if (frontDoor != null) distance = Mathf.Min(distance, Vector3.Distance(player.position, frontDoor.transform.position));
        if (backDoor != null) distance = Mathf.Min(distance, Vector3.Distance(player.position, backDoor.transform.position));
        bool inRange = playing && potion.IsStolen && distance <= interactionDistance;
        if (inRange)
        {
            gameplayHud?.SetInteractionPrompt("E", "OPEN EXIT", true);
            wasShowingDoorPrompt = true;
        }
        else if (wasShowingDoorPrompt)
        {
            gameplayHud?.SetInteractionVisible(false);
            wasShowingDoorPrompt = false;
        }

        if (inRange &&
            potion.IsStolen &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (frontDoor != null) frontDoor.SetActive(false);
            if (backDoor != null) backDoor.SetActive(false);
            if (additionalObjectsToRemove != null)
                foreach (GameObject target in additionalObjectsToRemove)
                    if (target != null) target.SetActive(false);
            if (blockingWalls != null)
                foreach (GameObject wall in blockingWalls)
                    if (wall != null)
                        foreach (Collider collider in wall.GetComponentsInChildren<Collider>(true))
                            collider.enabled = false;
            RemoveRequiredObjectsByName();
            doorsRemoved = true;
            gameplayHud?.SetInteractionVisible(false);
            wasShowingDoorPrompt = false;
            Debug.Log("Pintu hilang dan collider tembok dinonaktifkan!");
        }
    }

    private void ResolveNamedObjects()
    {
        if (frontDoor == null && !string.IsNullOrEmpty(frontDoorName)) frontDoor = FindLoadedSceneObject(frontDoorName);
        if (backDoor == null && !string.IsNullOrEmpty(backDoorName)) backDoor = FindLoadedSceneObject(backDoorName);

        if (additionalObjectNamesToRemove == null) return;
        if (additionalObjectsToRemove == null || additionalObjectsToRemove.Length != additionalObjectNamesToRemove.Length)
            additionalObjectsToRemove = new GameObject[additionalObjectNamesToRemove.Length];
        for (int i = 0; i < additionalObjectNamesToRemove.Length; i++)
            if (additionalObjectsToRemove[i] == null && !string.IsNullOrEmpty(additionalObjectNamesToRemove[i]))
                additionalObjectsToRemove[i] = FindLoadedSceneObject(additionalObjectNamesToRemove[i]);

        if (!missingTargetsReported && (frontDoor == null || backDoor == null))
        {
            missingTargetsReported = true;
            Debug.LogError($"DoorInteraction tidak menemukan target pintu '{frontDoorName}' / '{backDoorName}'. Periksa nama objek di RafliScene dan pastikan scene sudah disimpan.", this);
        }
    }

    private static GameObject FindLoadedSceneObject(string objectName)
    {
        Transform[] transforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform candidate in transforms)
        {
            if (candidate != null && candidate.gameObject.scene.IsValid() &&
                candidate.gameObject.scene.isLoaded && candidate.name == objectName)
                return candidate.gameObject;
        }
        return null;
    }

    private void RemoveRequiredObjectsByName()
    {
        Transform[] transforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform candidate in transforms)
        {
            if (candidate == null || !candidate.gameObject.scene.IsValid() || !candidate.gameObject.scene.isLoaded)
                continue;
            foreach (string objectName in requiredRemovalNames)
            {
                if (candidate.name == objectName)
                {
                    candidate.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }

    private void OnDisable()
    {
        if (wasShowingDoorPrompt) gameplayHud?.SetInteractionVisible(false);
        wasShowingDoorPrompt = false;
    }
}
