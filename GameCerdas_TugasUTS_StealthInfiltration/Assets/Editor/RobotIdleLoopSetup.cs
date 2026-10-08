using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Tool sekali-klik untuk objek atomic-robot:
///  1) mengaktifkan Loop Time pada clip locomotion (Idle/Walking/Running/Creep)
///  2) membuat Animator Controller dengan state "Idle" sebagai default
///  3) memasang controller itu ke objek robot di scene
/// sehingga saat Play, robot langsung memutar animasi Idle terus-menerus.
/// </summary>
public static class RobotIdleLoopSetup
{
    private const string MenuPath = "Tools/Stealth Infiltration/Setup Robot Idle Loop";
    private const string FbxPath = "Assets/Prefabs/atomic-robot/source/atomic-robot.fbx";
    private const string ControllerPath = "Assets/Prefabs/atomic-robot/AtomicRobotIdle.controller";
    private const string ClipName = "Idle";
    private const string TargetName = "atomic-robot";

    /// <summary>Clip yang secara alami berulang, Loop Time-nya dipaksa aktif.</summary>
    private static readonly string[] LoopClips = { "Idle", "Walking", "Running", "Creep" };

    [MenuItem(MenuPath, true)]
    private static bool ValidateSetup()
    {
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    [MenuItem(MenuPath)]
    public static void Setup()
    {
        // 1) Loop Time dulu, karena SaveAndReimport membuat referensi clip jadi basi.
        EnableLoopTime();

        // 2) Ambil clip Idle dari FBX.
        AnimationClip clip = FindClip(ClipName);
        if (clip == null)
        {
            EditorUtility.DisplayDialog("Setup Robot Idle Loop",
                "Clip '" + ClipName + "' tidak ditemukan di:\n" + FbxPath +
                "\n\nCek tab Animation pada aset FBX-nya.", "OK");
            return;
        }

        // 3) Buat/pakai controller dengan state Idle sebagai default.
        AnimatorController controller = EnsureController(clip);
        if (controller == null)
            return;

        // 4) Cari objek robot di scene lalu pasang controllernya.
        GameObject target = FindTarget();
        if (target == null)
        {
            EditorUtility.DisplayDialog("Setup Robot Idle Loop",
                "Objek '" + TargetName + "' tidak ditemukan di scene.\n\n" +
                "Drag " + FbxPath + " ke scene dulu, atau pilih objeknya di Hierarchy " +
                "lalu jalankan tool ini lagi.", "OK");
            return;
        }

        AssignController(target, controller);

        Debug.Log("[RobotIdleLoopSetup] Selesai.\n" +
                  "  Controller : " + ControllerPath + "\n" +
                  "  Objek      : " + target.name + "\n" +
                  "  State      : " + ClipName + " (loopTime=" + clip.isLooping + ")", target);
    }

    /// <summary>
    /// Mengaktifkan Loop Time pada clip locomotion. Harus dipanggil SEBELUM memuat
    /// clip, karena SaveAndReimport mengganti objek clip di memori.
    /// </summary>
    private static void EnableLoopTime()
    {
        ModelImporter importer = AssetImporter.GetAtPath(FbxPath) as ModelImporter;
        if (importer == null)
            return;

        ModelImporterClipAnimation[] clips = importer.clipAnimations;
        if (clips == null || clips.Length == 0)
            clips = importer.defaultClipAnimations;
        if (clips == null || clips.Length == 0)
            return;

        bool changed = false;
        for (int i = 0; i < clips.Length; i++)
        {
            ModelImporterClipAnimation anim = clips[i];
            if (anim == null || !LoopClips.Contains(anim.name))
                continue;
            if (anim.loopTime)
                continue;

            anim.loopTime = true;
            changed = true;
        }

        if (!changed)
            return;

        importer.clipAnimations = clips;
        importer.SaveAndReimport();
    }

    private static AnimationClip FindClip(string clipName)
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(FbxPath);
        if (assets == null)
            return null;

        // Nama persis (mis. "Idle").
        foreach (Object asset in assets)
        {
            AnimationClip clip = asset as AnimationClip;
            if (clip != null && clip.name == clipName)
                return clip;
        }

        // Cadangan: nama take Blender "Armature|Idle".
        foreach (Object asset in assets)
        {
            AnimationClip clip = asset as AnimationClip;
            if (clip != null && clip.name.EndsWith("|" + clipName))
                return clip;
        }

        return null;
    }

    private static AnimatorController EnsureController(AnimationClip clip)
    {
        AnimatorController controller =
            AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);

        if (controller == null)
        {
            string folder = Path.GetDirectoryName(ControllerPath).Replace('\\', '/');
            if (!AssetDatabase.IsValidFolder(folder))
            {
                EditorUtility.DisplayDialog("Setup Robot Idle Loop",
                    "Folder tidak ditemukan:\n" + folder, "OK");
                return null;
            }

            try
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Setup Robot Idle Loop",
                    "Gagal membuat controller:\n" + e.Message, "OK");
                return null;
            }

            if (controller == null)
            {
                EditorUtility.DisplayDialog("Setup Robot Idle Loop",
                    "Gagal membuat controller di:\n" + ControllerPath, "OK");
                return null;
            }
        }

        if (controller.layers == null || controller.layers.Length == 0)
            controller.AddLayer("Base Layer");

        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
        if (stateMachine == null)
        {
            EditorUtility.DisplayDialog("Setup Robot Idle Loop",
                "Controller tidak punya state machine.", "OK");
            return null;
        }

        // Cari state Idle; kalau tidak ada, pakai state kosong bekas pembuatan; kalau
        // tetap tidak ada, buat baru.
        AnimatorState state = null;
        AnimatorState emptyState = null;
        foreach (ChildAnimatorState child in stateMachine.states)
        {
            if (child.state == null)
                continue;
            if (child.state.name == ClipName)
            {
                state = child.state;
                break;
            }
            if (emptyState == null && child.state.motion == null)
                emptyState = child.state;
        }

        if (state == null)
            state = emptyState;
        if (state == null)
            state = stateMachine.AddState(ClipName);

        state.name = ClipName;
        state.motion = clip;
        stateMachine.defaultState = state;

        EditorUtility.SetDirty(state);
        EditorUtility.SetDirty(stateMachine);
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();

        return controller;
    }

    private static GameObject FindTarget()
    {
        // 1) Utamakan objek yang sedang dipilih user.
        if (Selection.activeGameObject != null)
        {
            GameObject picked = Selection.activeGameObject;
            if (IsRobotInstance(picked))
                return picked;

            Transform parent = picked.transform.parent;
            while (parent != null)
            {
                if (IsRobotInstance(parent.gameObject))
                    return parent.gameObject;
                parent = parent.parent;
            }
        }

        // 2) Pindai seluruh scene.
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (IsRobotInstance(root))
                return root;

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (IsRobotInstance(child.gameObject))
                    return child.gameObject;
            }
        }

        // 3) Cadangan: cocokkan nama saja.
        return GameObject.Find(TargetName);
    }

    private static bool IsRobotInstance(GameObject go)
    {
        if (go == null)
            return false;

        string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go);
        if (!string.IsNullOrEmpty(path))
            return path == FbxPath;

        return go.name == TargetName;
    }

    private static void AssignController(GameObject target, AnimatorController controller)
    {
        Animator animator = target.GetComponent<Animator>();
        if (animator == null)
            animator = Undo.AddComponent<Animator>(target);

        Undo.RecordObject(animator, "Setup Robot Idle Loop");
        animator.runtimeAnimatorController = controller;
        // Selalu animasi walau di luar layar, dan jangan biarkan clip menggeser posisi
        // objek (root motion) saat memutar Idle.
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        animator.applyRootMotion = false;
        EditorUtility.SetDirty(animator);

        EditorSceneManager.MarkSceneDirty(target.scene);
    }
}

