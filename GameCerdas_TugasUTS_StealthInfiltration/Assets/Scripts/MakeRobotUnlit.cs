using UnityEngine;

/// <summary>
/// Removes scene-light response from the Player robot while keeping its albedo
/// colors and textures visible.
/// </summary>
[DisallowMultipleComponent]
public class MakeRobotUnlit : MonoBehaviour
{
    private static readonly int BaseMapId = Shader.PropertyToID("_BaseMap");
    private static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private void Start()
    {
        Shader unlitShader = Shader.Find("Universal Render Pipeline/Unlit");
        if (unlitShader == null)
            unlitShader = Shader.Find("Unlit/Texture");

        if (unlitShader == null)
        {
            Debug.LogWarning("Tidak menemukan shader Unlit untuk robot Player.", this);
            return;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer targetRenderer in renderers)
        {
            // Accessing .materials creates per-renderer instances, leaving the FBX source materials intact.
            Material[] materials = targetRenderer.materials;
            foreach (Material material in materials)
            {
                if (material == null)
                    continue;

                Texture texture = GetTexture(material);
                Color color = GetColor(material);

                material.shader = unlitShader;
                if (material.HasProperty(BaseMapId))
                    material.SetTexture(BaseMapId, texture);
                if (material.HasProperty(MainTexId))
                    material.SetTexture(MainTexId, texture);
                if (material.HasProperty(BaseColorId))
                    material.SetColor(BaseColorId, color);
                if (material.HasProperty(ColorId))
                    material.SetColor(ColorId, color);

                material.DisableKeyword("_EMISSION");
            }

            targetRenderer.materials = materials;
        }
    }

    private static Texture GetTexture(Material material)
    {
        if (material.HasProperty(BaseMapId))
            return material.GetTexture(BaseMapId);
        if (material.HasProperty(MainTexId))
            return material.GetTexture(MainTexId);
        return material.mainTexture;
    }

    private static Color GetColor(Material material)
    {
        if (material.HasProperty(BaseColorId))
            return material.GetColor(BaseColorId);
        if (material.HasProperty(ColorId))
            return material.GetColor(ColorId);
        return Color.white;
    }
}
