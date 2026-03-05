namespace GLGizmos;

using System;
using System.Collections.Generic;
using UnityEngine;

public class GizmoDrawer : MonoSingleton<GizmoDrawer>
{
    public static List<Delegate> RenderQueue = [];

    public static Queue<Delegate> NextFrameRenderQueue = [];

    /// <summary> Render everything in the render queue on this camera. </summary>
    public void OnPostRender()
    {
        GizmoMat.SetPass(0);
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