namespace GLGizmos.Patches;

using HarmonyLib;

/// <summary> Patches <see cref="CameraController"/>'s awake method so we can create the <see cref="GizmoDrawer"/>. </summary>
[HarmonyPatch]
public static class CameraControllerPatch
{
    [HarmonyPostfix] [HarmonyPatch(typeof(CameraController), "Awake")]
    public static void CreateGizmoDrawer(CameraController __instance) =>
        __instance.gameObject.AddComponent<GizmoDrawer>();
}