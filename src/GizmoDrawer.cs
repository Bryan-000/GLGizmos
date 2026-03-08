namespace GLGizmos;

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Static class for drawing all the gizmos :3 </summary>
public static class GizmoDrawer
{
    /// <summary> Stuff to be drawn every OnPostRender. </summary>
    public static List<Delegate> RenderQueue = [];

    /// <summary> Stuff to be drawn in the next OnPostRender frame. </summary>
    public static Queue<Delegate> NextFrameRenderQueue = [];

    /// <summary> Loads the gizmodrawer :3 </summary>
    public static void Load() =>
        Camera.onPostRender += OnPostRender;

    /// <summary> Render everything in the render queue on this camera. </summary>
    public static void OnPostRender(Camera cam)
    {
        try
        {
            // if the main camera is hidden or null then use Camera.current
            Camera main = Camera.main;
            if (!main?.gameObject.activeInHierarchy ?? true)
                main = Camera.current;

            // only render on the main camera as some games have multiple cameras
            if (cam != main)
                return;
        }
        catch
        {
            // older versions of unity dont have Camera.main so uh TRY to use Camera.current
            if (cam != Camera.current)
                return;
        }

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