/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using AtraBase.Caching;
using AtraBase.Collections;
using AtraBase.Toolkit.Reflection;
using CommunityToolkit.Diagnostics;

namespace AtraCore.Framework.ReflectionManager;

/// <summary>
/// A class for cached reflection.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Records break stylecop :(.")]
public static class ReflectionCache
{
    /// <summary>
    /// Convenience flags for BindingFlags.
    /// </summary>
    public enum FlagTypes
    {
        /// <summary>
        /// For instance members, flattens hierarchy.
        /// </summary>
        InstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy,

        /// <summary>
        /// For static members, flattens hierarchy.
        /// </summary>
        StaticFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy,

        /// <summary>
        /// For instance members, do not flatten hierarchy.
        /// </summary>
        UnflattenedInstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,

        /// <summary>
        /// For static members, does not flatten hierarchy.
        /// </summary>
        UnflattenedStaticFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,
    }

    /// <summary>
    /// An entry for the reflection cache.
    /// </summary>
    /// <param name="Type"></param>
    /// <param name="Name"></param>
    /// <param name="FlagTypes"></param>
    /// <param name="MemberType"></param>
    /// <param name="Params"></param>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "This is a record lol.")]
    private readonly record struct ReflectionCacheMember(Type Type, string Name, FlagTypes FlagTypes, MemberTypes MemberType, Type[]? Params)
    {
        public override int GetHashCode()
        {
            unchecked
            {
                const int factor = -0x5AAA_AAD7;

                int ret = ((EqualityComparer<Type>.Default.GetHashCode(this.Type) * factor) + EqualityComparer<string>.Default.GetHashCode(this.Name)) * factor;
                ret += EqualityComparer<FlagTypes>.Default.GetHashCode(this.FlagTypes);
                ret *= factor;
                ret += EqualityComparer<MemberTypes>.Default.GetHashCode(this.MemberType);
                ret *= factor;

                if (this.Params is not null)
                {
                    for (int i = 0; i < this.Params.Length; i++)
                    {
                        Type? param = this.Params[i];
                        ret += EqualityComparer<Type>.Default.GetHashCode(param);
                        ret *= factor;
                    }
                }

                return ret;
            }
        }

        public bool Equals(ReflectionCacheMember other)
        {
            if (EqualityComparer<Type>.Default.Equals(this.Type, other.Type) && EqualityComparer<string>.Default.Equals(this.Name, other.Name) && EqualityComparer<FlagTypes>.Default.Equals(this.FlagTypes, other.FlagTypes) && EqualityComparer<MemberTypes>.Default.Equals(this.MemberType, other.MemberType))
            {
                return this.Params.ArraySequenceEquals(other.Params);
            }
            return false;
        }
    }

    private static readonly SimpleConcurrentCache<ReflectionCacheMember, MemberInfo> Cache = new(2, 50);

    /// <summary>
    /// Gets a constructorinfo, from the cache if possible.
    /// Looks for (and gets) the constructor with zero params.
    /// </summary>
    /// <param name="type">Type to search in.</param>
    /// <param name="flags">Flags to use.</param>
    /// <returns>ConstructorInfo, if it can be found.</returns>
    public static ConstructorInfo GetCachedConstructor(this Type type, FlagTypes flags)
        => type.GetCachedConstructor(flags, Type.EmptyTypes);

    /// <summary>
    /// Gets a constructorinfo, from the cache if possible.
    /// </summary>
    /// <param name="type">Type to search in.</param>
    /// <param name="flags">Flags to use.</param>
    /// <param name="paramsList">Paramater list.</param>
    /// <returns>ConstructorInfo, if it can be found.</returns>
    public static ConstructorInfo GetCachedConstructor(this Type type, FlagTypes flags, Type[] paramsList)
    {
        Guard.IsNotNull(type, nameof(type));

        ReflectionCacheMember cachekey = new(type, "ctor", flags, MemberTypes.Constructor, paramsList);
        if (Cache.TryGetValue(cachekey, out MemberInfo? cached) && cached is ConstructorInfo constructorInfo)
        {
            return constructorInfo;
        }
        ConstructorInfo? constructor = type.GetConstructor((BindingFlags)flags, null, paramsList, null);
        if (constructor is not null)
        {
            Cache[cachekey] = constructor;
            return constructor;
        }
        return ReflectionThrowHelper.ThrowMethodNotFoundException<ConstructorInfo>(type.FullName + "::.ctor" + string.Join(',', paramsList.Select((a) => a.FullName)));
    }

    /// <summary>
    /// Gets a MethodInfo, from the cache if possible.
    /// </summary>
    /// <param name="type">Type to search in.</param>
    /// <param name="methodName">Method name to search for.</param>
    /// <param name="flags">Binding flags to use.</param>
    /// <returns>MethodInfo if possible.</returns>
    public static MethodInfo GetCachedMethod(this Type type, string methodName, FlagTypes flags)
    {
        Guard.IsNotNull(type, nameof(type));
        Guard.IsNotNullOrWhiteSpace(methodName, nameof(methodName));

        ReflectionCacheMember cachekey = new(type, methodName, flags, MemberTypes.Method, null);
        if (Cache.TryGetValue(cachekey, out MemberInfo? cached) && cached is MethodInfo methodInfo)
        {
            return methodInfo;
        }
        MethodInfo? method = type.GetMethod(methodName, (BindingFlags)flags);
        if (method is not null)
        {
            Cache[cachekey] = method;
            return method;
        }
        return ReflectionThrowHelper.ThrowMethodNotFoundException<MethodInfo>(type.FullName + "::" + methodName);
    }

    /// <summary>
    /// Gets a MethodInfo, from the cache if possible.
    /// </summary>
    /// <param name="type">Type to search in.</param>
    /// <param name="methodName">Method name to search for.</param>
    /// <param name="flags">Binding flags to use.</param>
    /// <param name="paramsList">A list of parameters to constrain search.</param>
    /// <returns>Methodinfo.</returns>
    public static MethodInfo GetCachedMethod(this Type type, string methodName, FlagTypes flags, Type[] paramsList)
    {
        Guard.IsNotNull(type, nameof(type));
        Guard.IsNotNullOrWhiteSpace(methodName, nameof(methodName));

        ReflectionCacheMember cachekey = new(type, methodName, flags, MemberTypes.Method, paramsList);
        if (Cache.TryGetValue(cachekey, out MemberInfo? cached) && cached is MethodInfo methodInfo)
        {
            return methodInfo;
        }
        MethodInfo? method = type.GetMethod(methodName, (BindingFlags)flags, null, paramsList, null);
        if (method is not null)
        {
            Cache[cachekey] = method;
            return method;
        }
        return ReflectionThrowHelper.ThrowMethodNotFoundException<MethodInfo>(type.FullName + "::" + methodName + string.Join(',', paramsList.Select((a) => a.FullName)));
    }

    /// <summary>
    /// Gets a PropertyInfo, from the cache if possible.
    /// </summary>
    /// <param name="type">Type to search in.</param>
    /// <param name="propertyName">Property to search for.</param>
    /// <param name="flags">Binding flags to use.</param>
    /// <returns>PropertyInfo.</returns>
    public static PropertyInfo GetCachedProperty(this Type type, string propertyName, FlagTypes flags)
    {
        Guard.IsNotNull(type, nameof(type));
        Guard.IsNotNullOrWhiteSpace(propertyName, nameof(propertyName));

        ReflectionCacheMember cachekey = new(type, propertyName, flags, MemberTypes.Property, Type.EmptyTypes);
        if (Cache.TryGetValue(cachekey, out MemberInfo? cached) && cached is PropertyInfo propertyInfo)
        {
            return propertyInfo;
        }
        PropertyInfo? property = type.GetProperty(propertyName, (BindingFlags)flags);
        if (property is not null)
        {
            Cache[cachekey] = property;
            return property;
        }
        return ReflectionThrowHelper.ThrowMethodNotFoundException<PropertyInfo>(type.FullName + "::" + propertyName);
    }

    /// <summary>
    /// Gets a fieldinfo, from the cache if possible.
    /// </summary>
    /// <param name="type">Type to search in.</param>
    /// <param name="fieldName">Field name to search for.</param>
    /// <param name="flags">Binding flags to use.</param>
    /// <returns>FieldInfo.</returns>
    public static FieldInfo GetCachedField(this Type type, string fieldName, FlagTypes flags)
    {
        Guard.IsNotNull(type, nameof(type));
        Guard.IsNotNullOrWhiteSpace(fieldName, nameof(fieldName));

        ReflectionCacheMember cachekey = new(type, fieldName, flags, MemberTypes.Field, Type.EmptyTypes);
        if (Cache.TryGetValue(cachekey, out MemberInfo? cached) && cached is FieldInfo fieldInfo)
        {
            return fieldInfo;
        }
        FieldInfo? field = type.GetField(fieldName, (BindingFlags)flags);
        if (field is not null)
        {
            Cache[cachekey] = field;
            return field;
        }
        return ReflectionThrowHelper.ThrowMethodNotFoundException<FieldInfo>(type.FullName + "::" + fieldName);
    }

    #region niceties

    /// <inheritdoc cref="GetCachedMethod(Type, string, FlagTypes, Type[])"/>
    /// <remarks>For a method with a signature that takes one param.</remarks>
    /// <typeparam name="T">type signature.</typeparam>
    public static MethodInfo GetCachedMethod<T>(this Type type, string fieldName, FlagTypes flags)
        => type.GetCachedMethod(fieldName, flags, new[] { typeof(T) });

    public static MethodInfo GetCachedMethod<T1, T2>(this Type type, string fieldName, FlagTypes flags)
        => type.GetCachedMethod(fieldName, flags, new[] { typeof(T1), typeof(T2) });

    public static MethodInfo GetCachedMethod<T1, T2, T3>(this Type type, string fieldName, FlagTypes flags)
        => type.GetCachedMethod(fieldName, flags, new[] { typeof(T1), typeof(T2), typeof(T3) });

    public static MethodInfo GetCachedMethod<T1, T2, T3, T4>(this Type type, string fieldName, FlagTypes flags)
        => type.GetCachedMethod(fieldName, flags, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });

    public static ConstructorInfo GetCachedConstructor<T>(this Type type, FlagTypes flags)
        => type.GetCachedConstructor(flags, new[] { typeof(T) } );

    public static ConstructorInfo GetCachedConstructor<T1, T2>(this Type type, FlagTypes flags)
        => type.GetCachedConstructor(flags, new[] { typeof(T1), typeof(T2) });

    public static ConstructorInfo GetCachedConstructor<T1, T2, T3>(this Type type, FlagTypes flags)
        => type.GetCachedConstructor(flags, new[] { typeof(T1), typeof(T2), typeof(T3) });

    public static ConstructorInfo GetCachedConstructor<T1, T2, T3, T4>(this Type type, FlagTypes flags)
        => type.GetCachedConstructor(flags, new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
    #endregion

    /// <summary>
    /// Clears the reflection cache.
    /// </summary>
    internal static void Clear() => Cache.Clear();

    /// <summary>
    /// Swaps the hot cache to stale, clears the stale cache.
    /// </summary>
    internal static void Swap() => Cache.Swap();
}
