namespace GLGizmos.Harmony.Attributes;

using HarmonyLib;
using System;

/// <summary> Our own lil patch attribute so we can handle how patches are done. </summary>
/// <remarks> meow </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class PatchAttribute : Attribute
{
    public HarmonyMethod info = new();

    public PatchAttribute() { }
    
    public PatchAttribute(Type declaringType) =>
        info.declaringType = declaringType;

    public PatchAttribute(Type declaringType, string methodName)
    {
        info.declaringType = declaringType;
        info.methodName = methodName;
    }

    public PatchAttribute(Type declaringType, string methodName, MethodType methodType)
    {
        info.declaringType = declaringType;
        info.methodName = methodName;
        info.methodType = methodType;
    }

    public PatchAttribute(Type declaringType, string methodName, params Type[] argumentTypes)
    {
        info.declaringType = declaringType;
        info.methodName = methodName;
        info.argumentTypes = argumentTypes;
    }

    public PatchAttribute(string methodName) =>
        info.methodName = methodName;

    public PatchAttribute(string methodName, params Type[] argumentTypes)
    {
        info.methodName = methodName;
        info.argumentTypes = argumentTypes;
    }

    public PatchAttribute(string methodName, MethodType methodType)
    {
        info.methodName = methodName;
        info.methodType = methodType;
    }
}