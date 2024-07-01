/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using System.Reflection;
using HarmonyLib;

namespace weizinai.StardewValleyMod.Common.Patcher;

/// <summary>Provides base implementation logic for <see cref="IPatcher"/> instances.</summary>
public abstract class BasePatcher : IPatcher
{
    /// <inheritdoc />
    public abstract void Apply(Harmony harmony);

    /// <summary>Get a constructor and assert that it was found.</summary>
    /// <typeparam name="T">The type containing the method.</typeparam>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    /// <exception cref="InvalidOperationException">The type has no matching constructor.</exception>
    protected ConstructorInfo RequireConstructor<T>(Type[]? parameters = null)
    {
        return AccessTools.Constructor(typeof(T), parameters) ??
               throw new InvalidOperationException($"Can't find constructor {GetMethodString(typeof(T), null, parameters)} to patch.");
    }

    /// <summary>Get a method and assert that it was found.</summary>
    /// <typeparam name="T">The type containing the method.</typeparam>
    /// <param name="name">The method name.</param>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    /// <exception cref="InvalidOperationException">The type has no matching method.</exception>
    protected MethodInfo RequireMethod<T>(string name, Type[]? parameters = null)
    {
        return AccessTools.Method(typeof(T), name, parameters) ??
               throw new InvalidOperationException($"Can't find method {GetMethodString(typeof(T), name, parameters)} to patch.");
    }

    /// <summary>Get a Harmony patch method on the current patcher instance.</summary>
    /// <param name="name">The method name.</param>
    protected HarmonyMethod GetHarmonyMethod(string name)
    {
        return new HarmonyMethod(AccessTools.Method(this.GetType(), name)) ??
               throw new InvalidOperationException($"Can't find patcher method {GetMethodString(this.GetType(), name)}.");
    }

    /// <summary>Get a human-readable representation of a method target.</summary>
    /// <param name="type">The type containing the method.</param>
    /// <param name="name">The method name, or <c>null</c> for a constructor.</param>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    protected static string GetMethodString(Type type, string? name, Type[]? parameters = null)
    {
        var paramString = parameters?.Any() == true
            ? $"({string.Join(", ", parameters.Select(p => p.FullName))})"
            : string.Empty;

        return $"{type.FullName}{(name != null ? "." + name : string.Empty)}{paramString}";
    }
}