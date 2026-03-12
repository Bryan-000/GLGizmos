namespace GLGizmos.Harmony;

using GLGizmos.Harmony.Attributes;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

/// <summary> Manages all of our custom harmony patches. </summary>
public static class HarmonyManager
{
    /// <summary> "It's called harmony because it harms your mental health" - Doomah 2025/12/24 </summary>
    public static Harmony harm = new(PluginInfo.GUID);

    /// <summary> safely patches everything in the current assembly or the provided one. </summary>
    public static void SafePatchAll(Assembly asm = null)
    {
        asm ??= new StackTrace().GetFrame(1).GetMethod().ReflectedType.Assembly;

        AccessTools.GetTypesFromAssembly(asm).Do((type) =>
        {
            try
            {
                List<PatchAttribute> patches = [.. type.GetCustomAttributes<PatchAttribute>()];

                if (patches.Any())
                    SafePatchMethodsOfType(type, patches);
            }
            catch { }
        });
    }

    /// <summary> Safely patches all of our patches of a type. </summary>
    public static void SafePatchMethodsOfType(Type t, List<PatchAttribute> classPatches)
    {
        AccessTools.GetDeclaredMethods(t).Do((meth) =>
        {
            try
            {
                List<PatchAttribute> patches = [.. meth.GetCustomAttributes<PatchAttribute>()];
                if (!patches.Any())
                    return;

                HarmonyMethod miaow = HarmonyMethod.Merge([.. patches.Concat(classPatches).Select(p => p.info)]);
                miaow.method = meth; // die

                MethodBase target = ResolveTarget(miaow.declaringType, miaow.methodName, miaow.methodType, miaow.argumentTypes);
                PatchProcessor patchProcessor = harm.CreateProcessor(target);

                if (meth.HasAttribute<PrefixAttribute>()) patchProcessor.AddPrefix(miaow);
                if (meth.HasAttribute<PostfixAttribute>()) patchProcessor.AddPostfix(miaow);

                patchProcessor.Patch();
            }
            catch { }
        });
    }

    /// <summary> uhhmmmm why does harmony not have this 3: </summary>
    public static MethodBase ResolveTarget(Type type, string name, MethodType? methodType, Type[] args) =>
        methodType switch
        {
            MethodType.Constructor or MethodType.StaticConstructor => AccessTools.Constructor(type, args),
            MethodType.Getter => AccessTools.PropertyGetter(type, name),
            MethodType.Setter => AccessTools.PropertySetter(type, name),

            // just blehhh
            MethodType.Normal or _ => AccessTools.Method(type, name, args),
        };

    extension(MethodInfo meth)
    {
        /// <summary> Whether this method has the specified attribute. </summary>
        public bool HasAttribute<T>() where T : Attribute =>
            meth.GetCustomAttribute<T>() != null;
    }
}