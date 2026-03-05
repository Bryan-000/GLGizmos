namespace GLGizmos;

using System;
using System.Collections.Generic;
using UnityEngine;

public class GizmoDrawer : MonoSingleton<GizmoDrawer>
{
    public static List<Delegate> RenderQueue = [];

    public static Queue<Delegate> NextFrameRenderQueue = [];

    public static Material LineMat =>
        Instance.internal_LineMat ??= new(DefaultReferenceManager.Instance.masterShader);

    internal Material internal_LineMat = new(DefaultReferenceManager.Instance.masterShader);

    public static Material LineMatNoDepth =>
        Instance.internal_LineMatNoDepth ??= CreateNoDepth();

    internal Material internal_LineMatNoDepth = CreateNoDepth();

    public static Material CreateNoDepth()
    {
        // Unity has a built-in shader that is useful for drawing
        // simple colored things.
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        Material mat = new(shader) {
            hideFlags = HideFlags.HideAndDontSave
        };

        // Turn on alpha blending
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        // Turn backface culling off
        mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

        // Turn off depth writes
        mat.SetInt("_ZWrite", 0);

        // Always pass depth test (ignore depth)
        mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);

        return mat;
    }

    public void OnPostRender()
    {
        foreach (Delegate render in RenderQueue)
        {
            try
            {
                render.DynamicInvoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        while (NextFrameRenderQueue.TryDequeue(out Delegate render))
        {
            try
            {
                render.DynamicInvoke();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}