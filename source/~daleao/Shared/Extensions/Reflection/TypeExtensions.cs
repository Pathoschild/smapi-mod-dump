/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Extensions.Reflection;

#region using directives

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;

#endregion using directives

/// <summary>Provides extensions for simplified reflection on C# types.</summary>
/// <remarks>Original code by <see href="https://github.com/pardeike">Pardeike</see>.</remarks>
public static class TypeExtensions
{
    /// <summary>Determines whether the <paramref name="type"/> can be assigned to any of the <paramref name="candidates"/>.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="candidates">The candidate types.</param>
    /// <returns><see langword="true"/> if the <paramref name="type"/> is assignable to at least one of the <paramref name="candidates"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsAssignableToAnyOf(this Type type, params Type[] candidates)
    {
        return candidates.Any(type.IsAssignableTo);
    }

    /// <summary>Determines whether any of the <paramref name="candidates"/> can be assigned to the <paramref name="type"/>.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="candidates">The candidate types.</param>
    /// <returns><see langword="true"/> if the <paramref name="type"/> is assignable from at least one of the <paramref name="candidates"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsAssignableFromAnyOf(this Type type, params Type[] candidates)
    {
        return candidates.Any(type.IsAssignableFrom);
    }

    /// <summary>Enumerates all the types which derive from <paramref name="type"/>.</summary>
    /// <param name="type">The base <see cref="Type"/>.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> containing all types assignable to <paramref name="type"/>.</returns>
    public static IEnumerable<Type> GetDerivedTypes(this Type type)
    {
        return AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(type))
            .Where(t => t.IsAssignableTo(type));
    }

    #region delegate compilation

    /// <summary>Gets the <see cref="MethodInfo"/> for the <paramref name="delegateType"/>.</summary>
    /// <param name="delegateType">The delegate <see cref="Type"/>.</param>
    /// <returns>The corresponding <see cref="MethodInfo"/>.</returns>
    public static MethodInfo GetMethodInfoFromDelegateType(this Type delegateType)
    {
        return delegateType.GetMethod("Invoke")!;
    }

    #endregion delegate compilation

    #region safe reflection

    /// <summary>Gets a constructor and asserts that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <returns>The corresponding <see cref="ConstructorInfo"/>, if found.</returns>
    /// <exception cref="MissingMethodException">If a constructor is not found.</exception>
    [DebuggerStepThrough]
    public static ConstructorInfo RequireConstructor(this Type type)
    {
        return AccessTools.Constructor(type) ??
               ThrowHelper.ThrowMissingMethodException<ConstructorInfo>(
                   $"Couldn't find constructor for type {type.FullName}.");
    }

    /// <summary>Gets a constructor with the specified <paramref name="parameters"/> and asserts that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    /// <returns>The corresponding <see cref="ConstructorInfo"/>, if found.</returns>
    /// <exception cref="MissingMethodException">If a matching constructor is not found.</exception>
    [DebuggerStepThrough]
    public static ConstructorInfo RequireConstructor(this Type type, params Type[]? parameters)
    {
        return AccessTools.Constructor(type, parameters) ??
               ThrowHelper.ThrowMissingMethodException<ConstructorInfo>(
                   $"Couldn't find constructor {parameters.Description()} for type {type.FullName}.");
    }

    /// <summary>Gets a constructor and asserts that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="parameterCount">The the number of parameters in the overload signature.</param>
    /// <returns>The first constructor that matches the specified parameter count.</returns>
    /// <remarks>Useful when there's no compile-time access to one or more parameter types.</remarks>
    [DebuggerStepThrough]
    public static ConstructorInfo RequireConstructor(this Type type, int parameterCount)
    {
        return AccessTools.GetDeclaredConstructors(type).First(c => c.GetParameters().Length == parameterCount);
    }

    /// <summary>Gets a method and asserts that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="name">The method name.</param>
    /// <returns>The corresponding <see cref="MethodInfo"/>, if found.</returns>
    /// <exception cref="MissingMethodException">If a matching method is not found.</exception>
    [DebuggerStepThrough]
    public static MethodInfo RequireMethod(this Type type, string name)
    {
        return AccessTools.Method(type, name) ??
               ThrowHelper.ThrowMissingMethodException<MethodInfo>(
                   $"Couldn't find method {name} in type {type.FullName}.");
    }

    /// <summary>Gets a method and asserts that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="name">The method name.</param>
    /// <param name="parameters">The method parameter types, or <c>null</c> if it's not overloaded.</param>
    /// <returns>The corresponding <see cref="MethodInfo"/>, if found.</returns>
    /// <exception cref="MissingMethodException">If a matching method is not found.</exception>
    [DebuggerStepThrough]
    public static MethodInfo RequireMethod(this Type type, string name, Type[]? parameters)
    {
        return AccessTools.Method(type, name, parameters) ??
               ThrowHelper.ThrowMissingMethodException<MethodInfo>(
                   $"Couldn't find method {name} {parameters.Description()} in type {type.FullName}.");
    }

    /// <summary>Gets a field and asserts that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="name">The field name.</param>
    /// <returns>The corresponding <see cref="FieldInfo"/>, if found.</returns>
    /// <exception cref="MissingFieldException">If a matching field is not found.</exception>
    [DebuggerStepThrough]
    public static FieldInfo RequireField(this Type type, string name)
    {
        return AccessTools.Field(type, name) ??
               ThrowHelper.ThrowMissingFieldException<FieldInfo>(
                   $"Couldnd't find field {name} in type {type.FullName}.");
    }

    /// <summary>Gets a property getter and asserts that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="name">The property name.</param>
    /// <returns>The <see cref="MethodInfo"/> corresponding to the getter of the property, if found.</returns>
    /// <exception cref="MissingMethodException">If a matching property is not found.</exception>
    [DebuggerStepThrough]
    public static MethodInfo RequirePropertyGetter(this Type type, string name)
    {
        return AccessTools.Property(type, name)?.GetGetMethod(true) ??
               ThrowHelper.ThrowMissingMethodException<MethodInfo>(
                   $"Couldn't find property getter {name} in type {type.FullName}.");
    }

    /// <summary>Gets a property setter and asserts that it was found.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="name">The property name.</param>
    /// <returns>The <see cref="MethodInfo"/> corresponding to the setter of the property, if found.</returns>
    /// <exception cref="MissingMethodException">If a matching property is not found.</exception>
    [DebuggerStepThrough]
    public static MethodInfo RequirePropertySetter(this Type type, string name)
    {
        return AccessTools.Property(type, name)?.GetSetMethod(true) ??
               ThrowHelper.ThrowMissingMethodException<MethodInfo>(
                   $"Couldn't find property setter {name} in type {type.FullName}.");
    }

    /// <summary>Gets all inner types of <paramref name="parent"/>.</summary>
    /// <param name="parent">The parent <see cref="Type"/>.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all the inner <see cref="Type"/>s of <paramref name="parent"/>.</returns>
    public static IEnumerable<Type> GetAllInnerTypes(this Type parent)
    {
        yield return parent;

        var nested = parent.GetNestedTypes(AccessTools.all);
        for (var i = 0; i < nested.Length; i++)
        {
            foreach (var inner in GetAllInnerTypes(nested[i]))
            {
                yield return inner;
            }
        }
    }

    /// <summary>Gets all inner methods whose names contain the given <paramref name="substring"/>.</summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="substring">A substring.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all the inner <see cref="MethodInfo"/>s of <paramref name="type"/> containing the <paramref name="substring"/>.</returns>
    /// <exception cref="MissingMethodException">If a matching method is not found.</exception>
    public static IEnumerable<MethodInfo> GetInnerMethodsContaining(this Type type, string substring)
    {
        return type.GetAllInnerTypes()
            .SelectMany(AccessTools.GetDeclaredMethods)
            .Where(m => substring == "*" || m.Name.Contains(substring));
    }

    #endregion safe reflection
}
