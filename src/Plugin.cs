namespace GLGizmos;

using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> Loads all harmony patches and stuff :P </summary>
[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
public class Plugin : BaseUnityPlugin
{
    /// <summary> Load the mod. </summary>
    public void Awake()
    {
        // patch meow
        new Harmony(PluginInfo.GUID).PatchAll();

        // test drawer
        GizmoDrawer.RenderQueue.Add(delegate ()
        {
            GL.Begin(GL.LINES);

            GL.Color(new(0f, 1f, 0.75f));
            GL.Vertex3(0f, 5f, 0f);
            GL.Vertex3(0f, -5f, 0f);

            GL.Color(new(1f, 0.1f, 0.6f));
            GL.Vertex3(5f, 0f, 0f);
            GL.Vertex3(-5f, 0f, 0f);

            GL.Color(new(0.1f, 0.7f, 1f));
            GL.Vertex3(0f, 0f, 5f);
            GL.Vertex3(0f, 0f, -5f);

            GL.End();
        });

        InvokeRepeating("FindAllGizmoDrawers", 0f, 2f);

        SceneManager.sceneLoaded += (_, _) =>
        {
            GizmoDrawers.Clear();
            FindAllGizmoDrawers();
        };
    }

    /// <summary> Every MonoBehaviour/Method that draws gizmos. </summary>
    public static Dictionary<MonoBehaviour, MethodInfo> GizmoDrawers = [];

    /// <summary> Finds all monobehaviours that draw gizmos. </summary>
    public void FindAllGizmoDrawers()
    {
        foreach (MonoBehaviour mono in FindObjectsOfType<MonoBehaviour>())
        {
            if (GizmoDrawers.ContainsKey(mono))
                continue;

            MethodInfo DrawGizmos = mono.GetType().GetMethod("OnDrawGizmos", AccessTools.all);

            if (DrawGizmos != null)
                GizmoDrawers.Add(mono, DrawGizmos);
        }
    }

    /// <summary> Run all OnDrawGizmos. </summary>
    public void Update()
    {
        foreach (KeyValuePair<MonoBehaviour, MethodInfo> drawer in GizmoDrawers)
            drawer.Value.Invoke(drawer.Key, null);
    }
}