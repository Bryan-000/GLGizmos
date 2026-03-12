namespace GLGizmos.Harmony.Attributes;

using System;

[AttributeUsage(AttributeTargets.Method)]
public class PrefixAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class PostfixAttribute : Attribute { }