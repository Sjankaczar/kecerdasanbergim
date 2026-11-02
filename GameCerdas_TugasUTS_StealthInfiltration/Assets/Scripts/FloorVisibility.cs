using System.Collections.Generic;
using UnityEngine;

public class FloorVisibility : MonoBehaviour
{
    [SerializeField] private GameObject floor2;

    private GameObject[] taggedObjects;
    private readonly HashSet<Collider> playerCollidersInside = new HashSet<Collider>();
    private readonly HashSet<Renderer> visibilityTargets = new HashSet<Renderer>();
    private readonly HashSet<GameObject> floor1Exceptions = new HashSet<GameObject>();
    private readonly List<RendererSnapshot> targetSnapshots = new List<RendererSnapshot>();
    private readonly List<RendererState> floorRendererStates = new List<RendererState>();
    private readonly List<ColliderState> floorColliderStates = new List<ColliderState>();
    private bool floor2WasDeactivated;
    private bool effectsApplied;

    private sealed class RendererSnapshot
    {
        public Renderer renderer;
        public bool wasEnabled;
    }

    private sealed class RendererState
    {
        public Renderer component;
        public bool wasEnabled;
    }

    private sealed class ColliderState
    {
        public Collider component;
        public bool wasEnabled;
    }

    private void Start()
    {
        taggedObjects = GameObject.FindGameObjectsWithTag("Floor2Objects");
        CacheVisibilityTargets();
    }

    private void CacheVisibilityTargets()
    {
        visibilityTargets.Clear();
        floor1Exceptions.Clear();
        Transform[] transforms = FindObjectsOfType<Transform>(true);

        foreach (Transform targetRoot in transforms)
        {
            if (IsFloor1ExceptionName(targetRoot.gameObject.name))
                floor1Exceptions.Add(targetRoot.gameObject);

            if (!IsVisibilityTarget(targetRoot.gameObject.name))
                continue;

            foreach (MeshRenderer mesh in targetRoot.GetComponentsInChildren<MeshRenderer>(true))
                visibilityTargets.Add(mesh);
        }

        visibilityTargets.RemoveWhere(mesh => mesh == null || IsUnderFloor1Exception(mesh.transform));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !playerCollidersInside.Add(other))
            return;

        if (playerCollidersInside.Count == 1)
            ApplyFloorVisibility();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || !playerCollidersInside.Remove(other))
            return;

        if (playerCollidersInside.Count == 0)
            RestoreFloorVisibility();
    }

    private void ApplyFloorVisibility()
    {
        if (effectsApplied)
            return;

        effectsApplied = true;
        HideListedMeshes();
        HideFloor2();

        foreach (GameObject obj in taggedObjects)
        {
            if (obj != null && !ContainsVisibilityTarget(obj.transform))
                obj.SetActive(false);
        }
    }

    private void HideListedMeshes()
    {
        targetSnapshots.Clear();

        foreach (Renderer target in visibilityTargets)
        {
            if (target == null || IsUnderFloor1Exception(target.transform))
                continue;

            targetSnapshots.Add(new RendererSnapshot
            {
                renderer = target,
                wasEnabled = target.enabled
            });
            target.enabled = false;
        }
    }

    private void HideFloor2()
    {
        if (floor2 == null)
            return;

        if (!ContainsVisibilityTarget(floor2.transform))
        {
            floor2WasDeactivated = floor2.activeSelf;
            floor2.SetActive(false);
            return;
        }

        // Keep listed meshes active if they are children of floor2; hide the other floor renderers/colliders.
        floorRendererStates.Clear();
        foreach (Renderer renderer in floor2.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer == null || visibilityTargets.Contains(renderer) || IsUnderFloor1Exception(renderer.transform))
                continue;

            floorRendererStates.Add(new RendererState
            {
                component = renderer,
                wasEnabled = renderer.enabled
            });
            renderer.enabled = false;
        }

        floorColliderStates.Clear();
        foreach (Collider collider in floor2.GetComponentsInChildren<Collider>(true))
        {
            if (collider == null || HasVisibilityTarget(collider.transform) || IsUnderFloor1Exception(collider.transform))
                continue;

            floorColliderStates.Add(new ColliderState
            {
                component = collider,
                wasEnabled = collider.enabled
            });
            collider.enabled = false;
        }
    }

    private bool ContainsVisibilityTarget(Transform root)
    {
        foreach (Renderer target in visibilityTargets)
        {
            if (target != null && (target.transform == root || target.transform.IsChildOf(root)))
                return true;
        }

        foreach (GameObject exception in floor1Exceptions)
        {
            if (exception != null && (exception.transform == root || exception.transform.IsChildOf(root)))
                return true;
        }

        return false;
    }

    private bool HasVisibilityTarget(Transform root)
    {
        foreach (Renderer target in visibilityTargets)
        {
            if (target != null && target.transform == root)
                return true;
        }

        return floor1Exceptions.Contains(root.gameObject);
    }

    private bool IsUnderFloor1Exception(Transform root)
    {
        foreach (GameObject exception in floor1Exceptions)
        {
            if (exception != null && (exception.transform == root || root.IsChildOf(exception.transform)))
                return true;
        }

        return false;
    }

    private void RestoreFloorVisibility()
    {
        if (!effectsApplied)
            return;

        foreach (GameObject obj in taggedObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        if (floor2 != null)
        {
            if (floor2WasDeactivated)
            {
                floor2.SetActive(true);
            }
            else
            {
                RestoreRendererStates(floorRendererStates);
                RestoreColliderStates(floorColliderStates);
            }
        }

        foreach (RendererSnapshot snapshot in targetSnapshots)
        {
            if (snapshot.renderer != null)
                snapshot.renderer.enabled = snapshot.wasEnabled;
        }

        targetSnapshots.Clear();
        floorRendererStates.Clear();
        floorColliderStates.Clear();
        floor2WasDeactivated = false;
        effectsApplied = false;
    }

    private static void RestoreRendererStates(List<RendererState> states)
    {
        foreach (RendererState state in states)
        {
            if (state.component != null)
                state.component.enabled = state.wasEnabled;
        }
    }

    private static void RestoreColliderStates(List<ColliderState> states)
    {
        foreach (ColliderState state in states)
        {
            if (state.component != null)
                state.component.enabled = state.wasEnabled;
        }
    }

    private static bool IsFloor1ExceptionName(string objectName)
    {
        const string box008 = "bld_med_lab_mut_box_008";
        const string box009 = "bld_med_lab_mut_box_009";

        if (objectName.StartsWith(box008, System.StringComparison.OrdinalIgnoreCase))
        {
            string suffix = objectName.Substring(box008.Length).Trim().Trim('(', ')');
            return int.TryParse(suffix, out int index) && index >= 2 && index <= 5;
        }

        if (objectName.StartsWith(box009, System.StringComparison.OrdinalIgnoreCase))
        {
            string suffix = objectName.Substring(box009.Length).Trim().Trim('(', ')');
            return int.TryParse(suffix, out int index) && (index == 7 || index == 8);
        }

        return false;
    }

    private static bool IsVisibilityTarget(string objectName)
    {
        if (objectName.StartsWith("stoika", System.StringComparison.OrdinalIgnoreCase) ||
            objectName.StartsWith("glass_32", System.StringComparison.OrdinalIgnoreCase) ||
            objectName.StartsWith("nakladka", System.StringComparison.OrdinalIgnoreCase) ||
            objectName.StartsWith("ramka_monitora", System.StringComparison.OrdinalIgnoreCase) ||
            objectName.StartsWith("derjatel_monitora", System.StringComparison.OrdinalIgnoreCase))
            return true;

        const string stairsPrefix = "props_med_stairs_03 (";
        if (!objectName.StartsWith(stairsPrefix, System.StringComparison.OrdinalIgnoreCase) ||
            !objectName.EndsWith(")", System.StringComparison.Ordinal))
            return false;

        string number = objectName.Substring(stairsPrefix.Length, objectName.Length - stairsPrefix.Length - 1);
        return int.TryParse(number, out int index) && index >= 58 && index <= 66;
    }
}
