/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using SonoCore.Attributes;
using SonoCore.Extensions;
using StardewModdingAPI;
using System;
using System.Linq;
using System.Reflection;

namespace SonoCore
{
    /// <summary>A data model validator.</summary>
    internal static class ModelValidator
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Checks whether a model's members are valid.</summary>
        /// <typeparam name="T">The type of the model.</typeparam>
        /// <param name="monitor">The monitor to use for logging.</param>
        /// <returns><see langword="true"/>, if the model type is valid; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="monitor"/> is <see langword="null"/>.</exception>
        public static bool IsModelTypeValid<T>(IMonitor monitor)
        {
            if (monitor == null)
                throw new ArgumentNullException(nameof(monitor));

            var isValid = true;

            // ensure the identifier is valid
            {
                // ensure T has a single identifier
                var identifierProperties = typeof(T).GetIdentifierProperties();
                if (identifierProperties.Count() != 1)
                {
                    monitor.Log($"Type: '{typeof(T).FullName}' doesn't have one member with an '{nameof(IdentifierAttribute)}', instead it has {identifierProperties.Count()}", LogLevel.Error);
                    isValid = false;
                }
                else
                {
                    var identifierProperty = identifierProperties.First();

                    // ensure identifier isn't editable
                    if (identifierProperty.HasCustomAttribute<EditableAttribute>())
                    {
                        monitor.Log($"'{identifierProperty.GetFullName()}' is marked as the identifier by also editable", LogLevel.Error);
                        isValid = false;
                    }

                    // ensure identifier is readable
                    if (!identifierProperty.CanRead)
                    {
                        monitor.Log($"'{identifierProperty.GetFullName()}' is marked as the identifier but not readable", LogLevel.Error);
                        isValid = false;
                    }
                }
            }

            // ensure all editable/defaultable properties are valid
            {
                var editableProperties = typeof(T).GetInstanceProperties().Where(property => property.HasCustomAttribute<EditableAttribute>() || property.HasCustomAttribute<DefaultValueAttribute>());
                foreach (var editableProperty in editableProperties)
                {
                    // ensure property is nullable
                    var type = editableProperty.PropertyType;
                    if (type.IsValueType && (Nullable.GetUnderlyingType(type) == null))
                    {
                        monitor.Log($"'{editableProperty.GetFullName()}' is marked as editable/defaultable but not nullable", LogLevel.Error);
                        isValid = false;
                    }

                    // ensure property is readable & writable
                    if (!editableProperty.CanRead || !editableProperty.CanWrite)
                    {
                        monitor.Log($"'{editableProperty.GetFullName()}' is marked as editable/defaultable but not readable and writable", LogLevel.Error);
                        isValid = false;
                    }
                }
            }

            // ensure output of token properties are valid
            {
                var tokenProperties = typeof(T).GetTokenProperties().ToDictionary(property => property, property => property.GetCustomAttribute<TokenAttribute>().OutputPropertyName);
                foreach (var tokenProperty in tokenProperties)
                {
                    // ensure the output property exists
                    var tokenOutputProperty = typeof(T).GetInstanceProperties().FirstOrDefault(property => property.Name == tokenProperty.Value);
                    if (tokenOutputProperty == null)
                    {
                        monitor.Log($"'{tokenProperty.Key.GetFullName()}' is marked as having '{tokenProperty.Value}' as its token output, but no property with that name could be found", LogLevel.Error);
                        isValid = false;
                    }
                    else
                    {
                        // ensure the output property can be written to
                        if (!tokenOutputProperty.CanWrite)
                        {
                            monitor.Log($"'{tokenProperty.Key.GetFullName()}' is marked as a token output but it's not writable", LogLevel.Error);
                            isValid = false;
                        }

                        // ensure the output property is int
                        if (tokenOutputProperty.PropertyType != typeof(int))
                        {
                            monitor.Log($"'{tokenProperty.Key.GetFullName()}' is marked as a token output but it's not an int", LogLevel.Error);
                        }
                    }
                }
            }

            return isValid;
        }
    }
}
