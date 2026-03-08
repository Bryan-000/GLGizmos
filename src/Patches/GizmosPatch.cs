namespace GLGizmos.Patches;

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary> Patches <see cref="Gizmos"/> so it actually works ingame :3 </summary>
[HarmonyPatch(typeof(Gizmos))]
public static class GizmosPatch
{
    /// <summary> Our own color field since <see cref="Gizmos.color"/> only returns white. </summary>
    public static Color? color;

    /// <summary> Our own matrix field since <see cref="matrix"/> only returns default. </summary>
    public static Matrix4x4? matrix;

    /// <summary> Intercept <see cref="Gizmos.color"/>'s set method to also set our own <see cref="color"/> field. </summary>
    [HarmonyPrefix] [HarmonyPatch("color", MethodType.Setter)]
    public static void InterceptColorSetter(Color value) =>
        color = value;

    /// <summary> Intercept <see cref="Gizmos.matrix"/>'s set method to also set our own <see cref="color"/> field. </summary>
    [HarmonyPrefix] [HarmonyPatch("matrix", MethodType.Setter)]
    public static void InterceptMatrixSetter(Matrix4x4 value) =>
        matrix = value;

    /// <summary> Intercept <see cref="Gizmos.color"/>'s so it gets the true value. </summary>
    [HarmonyPostfix] [HarmonyPatch("color", MethodType.Getter)]
    public static void InterceptColorGetter(ref Color __result) =>
        __result = color ?? __result;

    /// <summary> Intercept <see cref="Gizmos.matrix"/>'s so it gets the true value. </summary>
    [HarmonyPostfix] [HarmonyPatch("matrix", MethodType.Getter)]
    public static void InterceptMatrixGetter(ref Matrix4x4 __result) =>
        __result = matrix ?? __result;

    #region DrawLines

    [HarmonyPrefix] [HarmonyPatch("DrawLine")]
    public static void DrawGizmoLine(Vector3 from, Vector3 to) =>
        DrawGizmoLines(Gizmos.color, Gizmos.matrix, GL.LINES, [from, to]);

    [HarmonyPrefix] [HarmonyPatch("DrawLineList", [typeof(ReadOnlySpan<Vector3>)])]
    public static void DrawGizmoLineList(ReadOnlySpan<Vector3> points) =>
        DrawGizmoLines(Gizmos.color, Gizmos.matrix, GL.LINES, [.. points]);

    [HarmonyPrefix] [HarmonyPatch("DrawLineStrip", [typeof(ReadOnlySpan<Vector3>), typeof(bool)])]
    public static void DrawGizmoLineStrip(ReadOnlySpan<Vector3> points, bool looped) =>
        DrawGizmoLines(Gizmos.color, Gizmos.matrix, GL.LINES, [.. points.ToArray().Concat(looped ? [points[0]] : [])]);

    public static void DrawGizmoLines(Color col, Matrix4x4 matrix4X4, int mode, List<Vector3> points) =>
        GizmoDrawer.NextFrameRenderQueue.Enqueue(delegate ()
        {
            GL.PushMatrix();
            GL.MultMatrix(matrix4X4);

            GL.Begin(mode);
            GL.Color(col);

            // draw all points
            points.ForEach(GL.Vertex);

            GL.End();
            GL.PopMatrix();
        });

    #endregion
    #region DrawCubes

    [HarmonyPrefix] [HarmonyPatch("DrawWireCube")]
    public static void DrawGizmoWireCube(Vector3 center, Vector3 size)
    {
        Color col = Gizmos.color;
        Matrix4x4 matrix4X4 = Gizmos.matrix;
        GizmoDrawer.NextFrameRenderQueue.Enqueue(delegate ()
        {
            GL.PushMatrix();
            GL.MultMatrix(matrix4X4*Matrix4x4.TRS(center, Quaternion.identity, size / 2));

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

    [HarmonyPrefix] [HarmonyPatch("DrawCube")]
    public static void DrawGizmoCube(Vector3 center, Vector3 size)
    {
        Color col = Gizmos.color;
        Matrix4x4 matrix4X4 = Gizmos.matrix;
        GizmoDrawer.NextFrameRenderQueue.Enqueue(delegate ()
        {
            GL.PushMatrix();
            GL.MultMatrix(matrix4X4*Matrix4x4.TRS(center, Quaternion.identity, size / 2));

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
    #region DrawSphere

    public static Mesh SphereMesh
    {
        get
        {
            if (field)
                return field;

            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            field = temp.GetComponent<MeshFilter>().sharedMesh;
            UnityEngine.Object.Destroy(temp);

            return field;
        }
        set;
    }

    /// <summary> <see cref="Mathf.Deg2Rad"/> * 16. </summary>
    public const float Deg2RadX16 = Mathf.Deg2Rad * 16;

    [HarmonyPrefix]
    [HarmonyPatch("DrawSphere")]
    public static void DrawGizmoSphere(Vector3 center, float radius) =>
        DawgGizmoMesh(SphereMesh, 0, center, Quaternion.identity, Vector3.one * radius, false);

    [HarmonyPrefix]
    [HarmonyPatch("DrawWireSphere")]
    public static void DrawGizmowireSphere(Vector3 center, float radius)
    {
        Color col = Gizmos.color;
        Matrix4x4 matrix4X4 = Gizmos.matrix;
        GizmoDrawer.NextFrameRenderQueue.Enqueue(delegate ()
        {
            GL.PushMatrix();
            GL.MultMatrix(matrix4X4 * Matrix4x4.TRS(center, Quaternion.identity, Vector3.one));

            // draw y
            GL.Begin(GL.LINE_STRIP);
            GL.Color(new(0f, 1f, 0.75f));
            for (int i = 0; i < 23; i++)
                GL.Vertex3(Mathf.Cos(i * Deg2RadX16) * 10f, 0f, Mathf.Sin(i * Deg2RadX16) * 10f);
            GL.Vertex3(10f, 0f, 0f);
            GL.End();

            // draw x
            GL.Begin(GL.LINE_STRIP);
            GL.Color(new(0f, 1f, 0.75f));
            for (int i = 0; i < 23; i++)
                GL.Vertex3(0f, Mathf.Cos(i * Deg2RadX16) * 10f, Mathf.Sin(i * Deg2RadX16) * 10f);
            GL.Vertex3(0f, 10f, 0f);
            GL.End();

            // draw z
            GL.Begin(GL.LINE_STRIP);
            GL.Color(new(0f, 1f, 0.75f));
            for (int i = 0; i < 23; i++)
                GL.Vertex3(Mathf.Cos(i * Deg2RadX16) * 10f, Mathf.Sin(i * Deg2RadX16) * 10f, 0f);
            GL.Vertex3(10f, 0f, 0f);
            GL.End();

            GL.PopMatrix();
        });
    }

    #endregion
    #region DrawFrustum

    [HarmonyPrefix] [HarmonyPatch("DrawFrustum")]
    public static void DrawGizmosFrustum(Vector3 center, float fov, float maxRange, float minRange, float aspect)
    {
        Color col = Gizmos.color;
        Matrix4x4 matrix4X4 = Gizmos.matrix;
        GizmoDrawer.NextFrameRenderQueue.Enqueue(delegate ()
        {
            GL.PushMatrix();
            GL.MultMatrix(matrix4X4 * Matrix4x4.TRS(center, Quaternion.identity, Vector3.one));

            // draw main frustum
            GL.Begin(GL.LINE_STRIP);
            GL.Color(col);

            float height = Mathf.Tan(fov/2f * Mathf.Deg2Rad);
            float max = height * maxRange;
            float min = height * minRange;

            GL.Vertex3(-max * aspect, max,  maxRange);
            GL.Vertex3(max  * aspect, max,  maxRange);
            GL.Vertex3(max  * aspect, -max, maxRange);
            GL.Vertex3(-max * aspect, -max, maxRange);
            GL.Vertex3(-max * aspect, max,  maxRange);

            GL.Vertex3(-min * aspect, min,  minRange);
            GL.Vertex3(min  * aspect, min,  minRange);
            GL.Vertex3(min  * aspect, -min, minRange);
            GL.Vertex3(-min * aspect, -min, minRange);
            GL.Vertex3(-min * aspect, min,  minRange);

            GL.End();

            // draw connecting lines :3333 miaow meow mrrrp rawr
            GL.Begin(GL.LINES);
            GL.Color(col);

            GL.Vertex3(max * aspect, max, maxRange);
            GL.Vertex3(min * aspect, min, minRange);

            GL.Vertex3(max * aspect, -max, maxRange);
            GL.Vertex3(min * aspect, -min, minRange);

            GL.Vertex3(-max * aspect, -max, maxRange);
            GL.Vertex3(-min * aspect, -min, minRange);

            GL.End();

            GL.PopMatrix();
        });
    }

    #endregion
    #region DrawMesh

    [HarmonyPrefix] [HarmonyPatch("DrawMesh", [typeof(Mesh), typeof(int), typeof(Vector3), typeof(Quaternion), typeof(Vector3)])]
    public static void DrawGizmoMesh(Mesh mesh, int submeshIndex, Vector3 position, Quaternion rotation, Vector3 scale) =>
        DawgGizmoMesh(mesh, submeshIndex, position, rotation, scale, false);

    [HarmonyPrefix] [HarmonyPatch("DrawWireMesh", [typeof(Mesh), typeof(int), typeof(Vector3), typeof(Quaternion), typeof(Vector3)])]
    public static void DrawGizmoWireMesh(Mesh mesh, int submeshIndex, Vector3 position, Quaternion rotation, Vector3 scale) =>
        DawgGizmoMesh(mesh, submeshIndex, position, rotation, scale, true);

    public static void DawgGizmoMesh(Mesh mesh, int submeshIndex, Vector3 position, Quaternion rotation, Vector3 scale, bool wireframe)
    {
        if (!mesh.canAccess || !mesh.isReadable)
            return;

        Color col = Gizmos.color;
        Matrix4x4 matrix4X4 = Gizmos.matrix;
        GizmoDrawer.NextFrameRenderQueue.Enqueue(delegate ()
        {
            GL.PushMatrix();
            GL.MultMatrix(matrix4X4*Matrix4x4.TRS(position, rotation, scale));
            GL.wireframe = wireframe;

            GL.Begin(GL.TRIANGLES);
            GL.Color(col);

            int[] indices = mesh.GetIndices(submeshIndex);
            for (int i = 0; i < indices.Length; i++)
                GL.Vertex(mesh.vertices[indices[i]]);

            GL.End();
            GL.PopMatrix();
            GL.wireframe = false;
        });
    }

    #endregion
    #region DrawIcon

    [HarmonyPrefix] [HarmonyPatch("DrawIcon", [typeof(Vector3), typeof(string), typeof(bool), typeof(Color)])]
    public static void DrawGizmoIconDrawIcon(Vector3 center, string name, bool allowScaling, Color tint)
    {
        Color col = Gizmos.color;
        Matrix4x4 matrix4X4 = Gizmos.matrix;
        Texture icon = Resources.Load<Texture>(name);
        GizmoDrawer.NextFrameRenderQueue.Enqueue(delegate ()
        {
            // calculate where to look as to look at the camera :3
            Quaternion rotation = Quaternion.LookRotation(matrix4X4.MultiplyPoint(center) - Camera.current.transform.position);

            GL.PushMatrix();
            GL.MultMatrix(matrix4X4 * Matrix4x4.TRS(center, rotation, Vector3.one));
            GizmoTexMat.mainTexture = icon;
            GizmoTexMat.SetPass(0);

            GL.Begin(GL.QUADS);
            GL.Color(col*tint);

            GL.TexCoord2(0f, 1f); GL.Vertex3(-10f, -10f, 0f);
            GL.TexCoord2(0f, 0f); GL.Vertex3(-10f, 10f, 0f);
            GL.TexCoord2(1f, 0f); GL.Vertex3(10f, 10f, 0f);
            GL.TexCoord2(1f, 1f); GL.Vertex3(10f, -10f, 0f);

            GL.End();
            GizmoMat.SetPass(0);
            GL.PopMatrix();
        });
    }

    #endregion
}