using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Tool sekali-klik untuk merakit objek Player first-person dari Robot yang sudah ada di scene.
/// Hasilnya: Player (CharacterController + PlayerFirstPersonController) dengan anak
/// "Robot" (model) dan "Main Camera" (kamera first-person).
/// </summary>
public static class PlayerRigSetup
{
    private const string MenuPath = "Tools/Stealth Infiltration/Setup Player Rig";
    private const string InputActionsPath = "Assets/InputSystem_Actions.inputactions";
    private const string PlayerName = "Player";
    private const string RobotName = "Robot";
    private const string CameraName = "Main Camera";
    private const string LevelName = "lacation_vl1_TTF";

    [MenuItem(MenuPath, true)]
    private static bool ValidateSetup()
    {
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    [MenuItem(MenuPath)]
    public static void Setup()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || !scene.isLoaded)
        {
            EditorUtility.DisplayDialog("Setup Player Rig", "Tidak ada scene aktif yang valid.", "OK");
            return;
        }

        GameObject robot = FindRoot(scene, RobotName);
        if (robot == null)
        {
            EditorUtility.DisplayDialog("Setup Player Rig",
                "Objek 'Robot' tidak ditemukan sebagai root scene.\n\n" +
                "Drag dulu Assets/Prefabs/futuristic-flying-animated-robot-low-poly/source/Robot.fbx ke scene, " +
                "lalu beri nama 'Robot'.", "OK");
            return;
        }

        if (FindRoot(scene, PlayerName) != null)
        {
            EditorUtility.DisplayDialog("Setup Player Rig",
                "Objek 'Player' sudah ada di scene.\n\nHapus objek Player terlebih dulu bila ingin merakit ulang.", "OK");
            return;
        }

        GameObject cameraObject = FindRoot(scene, CameraName);
        if (cameraObject == null && Camera.main != null)
            cameraObject = Camera.main.gameObject;

        if (cameraObject == null)
        {
            EditorUtility.DisplayDialog("Setup Player Rig",
                "Kamera 'Main Camera' tidak ditemukan di scene.", "OK");
            return;
        }

        Camera camera = cameraObject.GetComponent<Camera>();
        if (camera == null)
        {
            EditorUtility.DisplayDialog("Setup Player Rig",
                "GameObject '" + cameraObject.name + "' tidak memiliki komponen Camera.", "OK");
            return;
        }

        Bounds bounds = CalculateBounds(robot);
        if (bounds.size.sqrMagnitude <= 0f)
        {
            EditorUtility.DisplayDialog("Setup Player Rig",
                "Renderer pada Robot tidak ditemukan atau bounds-nya nol,\nsehingga ukuran CharacterController tidak bisa dihitung otomatis.", "OK");
            return;
        }

        BuildRig(scene, robot, cameraObject, camera, bounds);
    }

    private static GameObject FindRoot(Scene scene, string objectName)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == objectName)
                return root;
        }

        return null;
    }

    private static Bounds CalculateBounds(GameObject root)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
            return new Bounds();

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        return bounds;
    }

    private static void BuildRig(Scene scene, GameObject robot, GameObject cameraObject, Camera camera, Bounds bounds)
    {
        // Kapsul dibuat pas dengan bounding box Robot.
        float height = Mathf.Max(bounds.size.y, 0.5f);
        float radius = Mathf.Clamp(Mathf.Max(bounds.size.x, bounds.size.z) * 0.5f, 0.15f, height * 0.5f);

        GameObject player = new GameObject(PlayerName);
        Undo.RegisterCreatedObjectUndo(player, "Setup Player Rig");

        // Titik asal Player = pusat dasar kapsul, tepat di bawah Robot.
        player.transform.position = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
        player.transform.rotation = Quaternion.identity;
        player.transform.localScale = Vector3.one;

        CharacterController characterController = Undo.AddComponent<CharacterController>(player);
        characterController.height = height;
        characterController.radius = radius;
        characterController.center = new Vector3(0f, height * 0.5f, 0f);
        characterController.slopeLimit = 45f;
        characterController.stepOffset = Mathf.Clamp(height * 0.25f, 0.1f, 0.5f);
        characterController.skinWidth = Mathf.Max(radius * 0.08f, 0.005f);
        characterController.minMoveDistance = 0.001f;

        ReparentKeepingWorldTransform(robot, player);
        ReparentKeepingWorldTransform(cameraObject, player);

        // Kamera menjadi first-person: pivot pitch di kepala, yaw mengikuti Player.
        float eyeHeight = height * 0.9f;
        cameraObject.transform.localRotation = Quaternion.identity;
        cameraObject.transform.localPosition = new Vector3(0f, eyeHeight, 0f);

        InputActionAsset actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);
        PlayerFirstPersonController firstPerson = Undo.AddComponent<PlayerFirstPersonController>(player);
        firstPerson.Configure(actions, cameraObject.transform, camera);

        WarnAboutMissingColliders(scene);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Selection.activeGameObject = player;

        Debug.Log(
            "[PlayerRigSetup] Player dibuat.\n" +
            "  Capsule : radius " + radius.ToString("0.###") + ", height " + height.ToString("0.###") +
            ", eye height " + eyeHeight.ToString("0.###") + "\n" +
            "  Kamera  : '" + cameraObject.name + "' sebagai anak Player\n" +
            "  Input   : " + (actions != null ? actions.name : "InputSystem.actions") + " (Player/Move, Look, Jump, Sprint)\n" +
            "  Posisi  : " + player.transform.position, player);
    }

    private static void ReparentKeepingWorldTransform(GameObject child, GameObject newParent)
    {
        Vector3 worldPosition = child.transform.position;
        Quaternion worldRotation = child.transform.rotation;

        Undo.SetTransformParent(child.transform, newParent.transform, "Setup Player Rig");

        child.transform.position = worldPosition;
        child.transform.rotation = worldRotation;
    }

    private static void WarnAboutMissingColliders(Scene scene)
    {
        GameObject level = FindRoot(scene, LevelName);
        if (level == null)
            return;

        if (level.GetComponentsInChildren<Renderer>(true).Length == 0)
            return;

        if (level.GetComponentsInChildren<Collider>(true).Length > 0)
            return;

        Debug.LogWarning(
            "[PlayerRigSetup] Level '" + level.name + "' tidak memiliki collider sama sekali,\n" +
            "jadi pemain akan langsung jatuh menembus lantai/dinding.\n\n" +
            "Cara memperbaikinya:\n" +
            "1. Pilih Assets/Prefabs/location-underground-laboratory/source/lacation_vl1_TTF.fbx\n" +
            "2. Buka tab Model (ModelImporter)\n" +
            "3. Centang 'Add Colliders' pada bagian Meshes\n" +
            "4. Klik Apply\n\n" +
            "Alternatif: beri MeshCollider manual pada objek lantai/dinding yang bisa ditembus pemain.");
    }
}
