global using static GLGizmos.Materials;

namespace GLGizmos;

using UnityEngine;

/// <summary> Materials class for handling/creating materials for gizmo's. </summary>
public static class Materials
{
    /// <summary> Generic unlit material for gizmo's. </summary>
    public static Material GizmoMat =>
        internal_GizmoMat ??= CreateGizmo();

    /// <summary> Same as <see cref="GizmoMat"/> but without depth and renders ontop of everything else. </summary>
    public static Material GizmoMatNoDepth =>
        internal_GizmoMatNoDepth ??= CreateGizmoNoDepth();

    /// <summary> meowmeowmeow </summary>
    internal static Material internal_GizmoMat = CreateGizmo(),
        internal_GizmoMatNoDepth = CreateGizmoNoDepth();

    /// <summary> Creates a new material designed for gizmo's, without depth. </summary>
    public static Material CreateGizmoNoDepth()
    {
        Material mat = CreateGizmo();

        // Always pass depth test (ignore depth)
        mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);

        return mat;
    }

    /// <summary> Creates a new material designed for gizmo's. </summary>
    public static Material CreateGizmo()
    {
        // Unity has a built-in shader that is useful for drawing
        // simple colored things.
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        Material mat = new(shader)
        {
            hideFlags = HideFlags.HideAndDontSave
        };

        // Turn on alpha blending
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        // Turn backface culling off
        mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

        // Turn off depth writes
        mat.SetInt("_ZWrite", 0);

        return mat;
    }
}