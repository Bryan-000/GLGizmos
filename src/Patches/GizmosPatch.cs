namespace GLGizmos.Patches;

using HarmonyLib;
using UnityEngine;

/// <summary> Patches <see cref="Gizmos"/> so it actually works ingame :3 </summary>
[HarmonyPatch]
public static class GizmosPatch
{ 
    /// <summary> Our own color field since <see cref="Gizmos.color"/> only returns white. </summary>
    public static Color color;

    /// <summary> Intercept <see cref="Gizmos.color"/>'s set method to also set our own <see cref="color"/> field. </summary>
    [HarmonyPrefix] [HarmonyPatch(typeof(Gizmos), "set_color")]
    public static void InterceptColorSetter(Color value) =>
        color = value;

    [HarmonyPrefix] [HarmonyPatch(typeof(Gizmos), "DrawLine")]
    public static void DrawGizmoLine(Vector3 from, Vector3 to)
    {
        Color col = color; // save the color for later
        GizmoDrawer.NextFrameRenderQueue.Enqueue(delegate ()
        {
            GL.PushMatrix();
            GL.MultMatrix(Gizmos.matrix);

            GL.Begin(GL.LINES);
            GL.Color(col);

            GL.Vertex(from);
            GL.Vertex(to);

            GL.End();
            GL.PopMatrix();
        });
    }

    [HarmonyPrefix] [HarmonyPatch(typeof(Gizmos), "DrawWireCube")]
    public static void DrawGizmoWireCube(Vector3 center, Vector3 size) =>
        DrawGizmoCube(center, size, true);

    [HarmonyPrefix] [HarmonyPatch(typeof(Gizmos), "DrawCube")]
    public static void DrawGizmoCube(Vector3 center, Vector3 size) =>
        DrawGizmoCube(center, size, false);

    public static void DrawGizmoCube(Vector3 center, Vector3 size, bool wireframe)
    {
        Color col = color;
        col.a = 0.5f;
        GizmoDrawer.NextFrameRenderQueue.Enqueue(delegate ()
        {
            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.TRS(center, Quaternion.identity, size / 2));
            GL.wireframe = wireframe;

            GL.Begin(GL.QUADS);
            GL.Color(col);

            // front face
            GL.Vertex3(1f, 1f, 1f);
            GL.Vertex3(-1f, 1f, 1f);
            GL.Vertex3(-1f, -1f, 1f);
            GL.Vertex3(1f, -1f, 1f);

            // back face
            GL.Vertex3(1f, -1f, -1f);
            GL.Vertex3(-1f, -1f, -1f);
            GL.Vertex3(-1f, 1f, -1f);
            GL.Vertex3(1f, 1f, -1f);

            // left face
            GL.Vertex3(-1f, 1f, 1f);
            GL.Vertex3(-1f, 1f, -1f);
            GL.Vertex3(-1f, -1f, -1f);
            GL.Vertex3(-1f, -1f, 1f);

            // right face
            GL.Vertex3(1f, 1f, -1f);
            GL.Vertex3(1f, 1f, 1f);
            GL.Vertex3(1f, -1f, 1f);
            GL.Vertex3(1f, -1f, -1f);

            // top face
            GL.Vertex3(-1f, 1f, 1f);
            GL.Vertex3(1f, 1f, 1f);
            GL.Vertex3(1f, 1f, -1f);
            GL.Vertex3(-1f, 1f, -1f);

            // bottom face
            GL.Vertex3(1f, -1f, 1f);
            GL.Vertex3(-1f, -1f, 1f);
            GL.Vertex3(-1f, -1f, -1f);
            GL.Vertex3(1f, -1f, -1f);

            GL.End();
            GL.PopMatrix();
            GL.wireframe = false;
        });
    }
}