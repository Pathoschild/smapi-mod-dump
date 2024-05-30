/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#endregion using directives

/// <summary>Extensions for generic objects.</summary>
public static class GenericExtensions
{
    /// <summary>Determines whether the <paramref name="value"/> is equal to an<paramref name="other"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="other">Some other value with which to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> and <paramref name="other"/> are equal with respect to the default <see cref="EqualityComparer{T}"/>, otherwise <see langword="false"/>.</returns>
    public static bool Compare<T>(this T value, T other)
        where T : IEquatable<T>
    {
        return EqualityComparer<T>.Default.Equals(value, other);
    }

    /// <summary>Determines whether the <paramref name="value"/> is equal to any of the <paramref name="candidates"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="candidates">Some candidates to check.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is equal to at least one of the <paramref name="candidates"/> with respect to the default <see cref="EqualityComparer{T}"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsAnyOf<T>(this T value, params T[] candidates)
    {
        return candidates.Contains(value);
    }

    /// <summary>Determines whether <paramref name="value"/> is equal to any of the enumerated <paramref name="candidates"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="candidates">The candidates to check.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is equal to at least one of the <paramref name="candidates"/> with respect to the default <see cref="EqualityComparer{T}"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsIn<T>(this T value, IEnumerable<T> candidates)
    {
        return candidates.Contains(value);
    }

    /// <summary>Enumerates the <paramref name="value"/> and <paramref name="others"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="others">Some other objects to collect.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> combining <paramref name="value"/> and <paramref name="others"/>.</returns>
    public static IEnumerable<T> Collect<T>(this T value, params T[] others)
    {
        yield return value;
        foreach (var other in others)
        {
            yield return other;
        }
    }

    /// <summary>Enumerates the <paramref name="value"/> along with the enumerated <paramref name="others"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="others">Some other enumeration to collect.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> combining <paramref name="value"/> and <paramref name="others"/>.</returns>
    public static IEnumerable<T> Collect<T>(this T value, IEnumerable<T> others)
    {
        yield return value;
        foreach (var next in others)
        {
            yield return next;
        }
    }

    /// <summary>Determines whether the <paramref name="object"/> is <see langwor="null"/> or equal to the default for type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="object"/>.</typeparam>
    /// <param name="object">The object.</param>
    /// <returns><see langword="true"/> if <paramref name="object"/> is <see langword="null"/> or <see langword="default"/>, otherwise <see langword="false"/>.</returns>
    /// <remarks>Adapted from https://stackoverflow.com/a/6553276/1889720.</remarks>
    public static bool IsNullOrDefault<T>(this T @object)
    {
        // deal with normal scenarios
        if (@object is null || @object.Equals(default(T)))
        {
            return true;
        }

        // deal with non-null nullables
        var declaredType = typeof(T);
        if (Nullable.GetUnderlyingType(declaredType) is not null)
        {
            return false;
        }

        // deal with boxed value types
        var instanceType = @object.GetType();
        if (!instanceType.IsValueType || instanceType == declaredType)
        {
            return false;
        }

        var @default = Activator.CreateInstance(instanceType);
        return @object.Equals(@default);
    }

    /// <summary>Calculates the sum of two generic values.</summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="a">The first summand.</param>
    /// <param name="b">The second summand.</param>
    /// <returns>a+b</returns>
    public static T GenericAdd<T>(this T a, T b)
        where T : struct
    {
        // declare the parameters
        var paramA = Expression.Parameter(typeof(T), "a");
        var paramB = Expression.Parameter(typeof(T), "b");

        // add the parameters together
        var body = Expression.Add(paramA, paramB);

        // compile it
        var add = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();

        // call it
        return add(a, b);
    }
}
