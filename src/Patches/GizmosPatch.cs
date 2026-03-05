namespace GLGizmos.Patches;

using HarmonyLib;
using UnityEngine;

/// <summary> Patches <see cref="Gizmos"/> so it actually works ingame :3 </summary>
[HarmonyPatch]
public static class GizmosPatch
{
    [HarmonyPrefix] [HarmonyPatch(typeof(Gizmos), "DrawLine")]
    public static void DrawGizmoLine(Vector3 from, Vector3 to) =>
        GizmoDrawer.NextFrameRenderQueue.Enqueue(delegate ()
        {
            GizmoDrawer.LineMat.SetPass(0);
            GL.PushMatrix();
            GL.MultMatrix(Gizmos.matrix);

            GL.Begin(GL.LINES);

            GL.Color(Gizmos.color);
            GL.Vertex(from);
            GL.Vertex(to);

            GL.End();
            GL.PopMatrix();
        });
}