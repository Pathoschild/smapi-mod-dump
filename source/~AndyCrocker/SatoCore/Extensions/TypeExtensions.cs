/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using SatoCore.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace SatoCore.Extensions
{
    /// <summary>Extension methods for the <see cref="Type"/> class.</summary>
    public static class TypeExtensions
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Retrieves all the instance properties of the type.</summary>
        /// <param name="type">The type whose instance properties should be retrieved.</param>
        /// <returns>All the instance properties of <paramref name="type"/>.</returns>
        public static PropertyInfo[] GetInstanceProperties(this Type type) => type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>Retrieves all the properties that have an <see cref="IdentifierAttribute"/>.</summary>
        /// <param name="type">The type whose identifier properties should be retrieved.</param>
        /// <returns>The properties that have an <see cref="IdentifierAttribute"/>.</returns>
        public static PropertyInfo[] GetIdentifierProperties(this Type type) => type.GetPropertiesWithAttribute<IdentifierAttribute>();

        /// <summary>Retrieves all properties that have an <see cref="EditableAttribute"/>.</summary>
        /// <param name="type">The type whose editable properties should be retrieved.</param>
        /// <returns>The properties that have an <see cref="EditableAttribute"/>.</returns>
        public static PropertyInfo[] GetEditableProperties(this Type type) => type.GetPropertiesWithAttribute<EditableAttribute>();

        /// <summary>Retrieves all properties that have a <see cref="DefaultValueAttribute"/>.</summary>
        /// <param name="type">The type whose defaultable properties should be retrieved.</param>
        /// <returns>The properties that have a <see cref="DefaultValueAttribute"/>.</returns>
        public static PropertyInfo[] GetDefaultableProperties(this Type type) => type.GetPropertiesWithAttribute<DefaultValueAttribute>();

        /// <summary>Retrieves all properties that have a <see cref="DefaultValueAttribute"/>.</summary>
        /// <param name="type">The type whose token properties should be retrieved.</param>
        /// <returns>The properties that have a <see cref="DefaultValueAttribute"/>.</returns>
        public static PropertyInfo[] GetTokenProperties(this Type type) => type.GetPropertiesWithAttribute<TokenAttribute>();

        /// <summary>Retrieves all properties that have a <see cref="RequiredAttribute"/>.</summary>
        /// <param name="type">The type whose token properties should be retrieved.</param>
        /// <returns>The properties that have a <see cref="RequiredAttribute"/>.</returns>
        public static PropertyInfo[] GetRequiredProperties(this Type type) => type.GetPropertiesWithAttribute<RequiredAttribute>();

        /// <summary>Retrieves all properties with a specified attribute.</summary>
        /// <typeparam name="T">The type of attribute to get the properties which have it.</typeparam>
        /// <returns>The properties which have an attribute of type <typeparamref name="T"/>.</returns>
        public static PropertyInfo[] GetPropertiesWithAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            return type.GetInstanceProperties()
                .Where(property => property.HasCustomAttribute<TAttribute>())
                .ToArray();
        }
    }
}
