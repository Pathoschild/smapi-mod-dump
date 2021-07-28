/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using System;
using System.Reflection;

namespace SatoCore.Extensions
{
    /// <summary>Extension methods for the <see cref="MemberInfo"/> class.</summary>
    public static class MemberInfoExtensions
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Retrieves the full name of the member.</summary>
        /// <param name="memberInfo">The member instance to get the full name of.</param>
        /// <returns>The full name of the member.</returns>
        public static string GetFullName(this MemberInfo memberInfo) => $"{memberInfo.DeclaringType.FullName}.{memberInfo.Name}";

        /// <summary>Gets whether the member has an attribute of a specified type.</summary>
        /// <typeparam name="T">The type of attribute to check for.</typeparam>
        /// <param name="memberInfo">The member whose attributes should get checked.</param>
        /// <returns><see langword="true"/>, if the member has an attribute of type <typeparamref name="T"/>; otherwise, <see langword="false"/>.</returns>
        public static bool HasCustomAttribute<T>(this MemberInfo memberInfo) where T : Attribute => memberInfo.GetCustomAttribute<T>() != null;
    }
}
