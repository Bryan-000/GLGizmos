global using static GLGizmos.Materials;

namespace GLGizmos;

using UnityEngine;

/// <summary> Materials class for handling/creating materials for gizmo's. </summary>
public static class Materials
{
    /// <summary> Generic unlit colored material for gizmo's. </summary>
    public static Material GizmoMat
    { 
        get 
        {
            if (!field)
                field = CreateGizmo();

            return field;
        } 
    }

    /// <summary> Same as <see cref="GizmoMat"/> but without depth and renders ontop of everything else. </summary>
    public static Material GizmoMatNoDepth
    { 
        get 
        {
            if (!field)
                field = CreateGizmoNoDepth();

            return field;
        } 
    }

    /// <summary> Generic unlit textured material for gizmo's. </summary>
    public static Material GizmoTexMat
    {
        get
        {
            if (!field)
                field = CreateTexGizmo();

            return field;
        }
    }

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

        // Turn off depth writes
        mat.SetInt("_ZWrite", 0);

        // Make backface culling work the way we want :3
        mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Back);

        return mat;
    }


    /// <summary> Creates a new textured material designed for gizmo's. </summary>
    public static Material CreateTexGizmo()
    {
        Shader shader = Shader.Find("Unlit/Texture");
        Material mat = new(shader)
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        
        mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

        return mat;
    }
}