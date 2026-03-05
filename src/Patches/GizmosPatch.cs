namespace GLGizmos.Patches;

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
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

    #region DrawLines

    [HarmonyPrefix] [HarmonyPatch(typeof(Gizmos), "DrawLine")]
    public static void DrawGizmoLine(Vector3 from, Vector3 to) =>
        DrawGizmoLines(color, GL.LINES, [from, to]);

    [HarmonyPrefix] [HarmonyPatch(typeof(Gizmos), "DrawLineList", [typeof(ReadOnlySpan<Vector3>)])]
    public static void DrawGizmoLineList(ReadOnlySpan<Vector3> points) =>
        DrawGizmoLines(color, GL.LINES, [.. points]);

    [HarmonyPrefix] [HarmonyPatch(typeof(Gizmos), "DrawLineStrip", [typeof(ReadOnlySpan<Vector3>), typeof(bool)])]
    public static void DrawGizmoLineStrip(ReadOnlySpan<Vector3> points, bool looped) =>
        DrawGizmoLines(color, GL.LINES, [.. points.ToArray().Concat(looped ? [points[0]] : [])]);

    public static void DrawGizmoLines(Color col, int mode, List<Vector3> points) =>
        GizmoDrawer.NextFrameRenderQueue.Enqueue(delegate ()
        {
            GL.PushMatrix();
            GL.MultMatrix(Gizmos.matrix);

            GL.Begin(mode);
            GL.Color(col);

            // draw all points
            points.ForEach(GL.Vertex);

            GL.End();
            GL.PopMatrix();
        });

    #endregion
    #region DrawCubes

    [HarmonyPrefix] [HarmonyPatch(typeof(Gizmos), "DrawWireCube")]
    public static void DrawGizmoWireCube(Vector3 center, Vector3 size)
    {
        Color col = color;
        GizmoDrawer.NextFrameRenderQueue.Enqueue(delegate ()
        {
            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.TRS(center, Quaternion.identity, size / 2));

            GL.Begin(GL.LINES);
            GL.Color(col);

            // front face
            GL.Vertex3(-1f, -1f, 1f); GL.Vertex3(1f, -1f, 1f);
            GL.Vertex3(1f, -1f, 1f);  GL.Vertex3(1f, 1f, 1f);
            GL.Vertex3(1f, 1f, 1f);   GL.Vertex3(-1f, 1f, 1f);
            GL.Vertex3(-1f, 1f, 1f);  GL.Vertex3(-1f, -1f, 1f);

            // back face
            GL.Vertex3(-1f, -1f, -1f); GL.Vertex3(1f, -1f, -1f);
            GL.Vertex3(1f, -1f, -1f);  GL.Vertex3(1f, 1f, -1f);
            GL.Vertex3(1f, 1f, -1f);   GL.Vertex3(-1f, 1f, -1f);
            GL.Vertex3(-1f, 1f, -1f);  GL.Vertex3(-1f, -1f, -1f);

            // connecting edges from front->back
            GL.Vertex3(-1f, -1f, -1f); GL.Vertex3(-1f, -1f, 1f);
            GL.Vertex3(1f, -1f, -1f);  GL.Vertex3(1f, -1f, 1f);
            GL.Vertex3(1f, 1f, -1f);   GL.Vertex3(1f, 1f, 1f);
            GL.Vertex3(-1f, 1f, -1f);  GL.Vertex3(-1f, 1f, 1f);

            GL.End();
            GL.PopMatrix();
        });
    }

    [HarmonyPrefix] [HarmonyPatch(typeof(Gizmos), "DrawCube")]
    public static void DrawGizmoCube(Vector3 center, Vector3 size)
    {
        Color col = color;
        GizmoDrawer.NextFrameRenderQueue.Enqueue(delegate ()
        {
            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.TRS(center, Quaternion.identity, size / 2));

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
        });
    }

    #endregion
    #region DrawMesh
    
    [HarmonyPrefix] [HarmonyPatch(typeof(Gizmos), "DrawMesh", [typeof(Mesh), typeof(int), typeof(Vector3), typeof(Quaternion), typeof(Vector3)])]
    public static void DawgGizmoMesh(Mesh mesh, int submeshIndex, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Color col = color;
        GizmoDrawer.NextFrameRenderQueue.Enqueue(delegate ()
        {
            GL.PushMatrix();
            GL.MultMatrix(Gizmos.matrix);

            GL.Begin(GL.TRIANGLES);
            GL.Color(col);
            
            int[] indices = mesh.GetIndices(submeshIndex);
            for (int i = 0; i < indices.Length; i++)
                GL.Vertex(mesh.vertices[i]);

            GL.End();
            GL.PopMatrix();
        });
    }

    #endregion
}