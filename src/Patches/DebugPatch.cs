namespace GLGizmos.Patches;

using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

/// <summary> Patches <see cref="Debug"/> so <see cref="Debug.DrawLine(Vector3, Vector3, Color, float, bool)"/> and others worl. </summary>
[HarmonyPatch]
public static class DebugPatch
{
    [HarmonyPrefix] [HarmonyPatch(typeof(Debug), "DrawLine", [typeof(Vector3), typeof(Vector3), typeof(Color), typeof(float), typeof(bool)])]
    public static void DrawDebugLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest) =>
        GizmoDrawer.Instance.StartCoroutine(DrawDebugLineAsync(start, end, color, duration, depthTest));

    /// <summary> Draws a line from <paramref name="start"/> to <paramref name="end"/> with the specified color, duration, and whether it should have depth. </summary>
    public static IEnumerator DrawDebugLineAsync(Vector3 start, Vector3 end, Color color, float duration, bool depth)
    {
        Delegate render = delegate ()
        {
            (depth ? GizmoDrawer.LineMat : GizmoDrawer.LineMatNoDepth).SetPass(0);
            GL.Begin(GL.LINES);

            GL.Color(color);
            GL.Vertex(start);
            GL.Vertex(end);

            GL.End();
        };

        GizmoDrawer.RenderQueue.Add(render);
        yield return new WaitForSeconds(duration);
        GizmoDrawer.RenderQueue.Remove(render);
    }
}
