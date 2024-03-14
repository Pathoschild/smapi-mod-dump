/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace SonoCore;

/// <summary>A data model validator.</summary>
internal static class ModelValidator
{
    /*********
    ** Public Methods
    *********/
    /// <summary>Checks whether a model's members are valid.</summary>
    /// <typeparam name="T">The type of the model.</typeparam>
    /// <typeparam name="TIdentifier">The type the model identifier should be.</typeparam>
    /// <param name="monitor">The monitor to use for logging.</param>
    /// <returns><see langword="true"/>, if the model type is valid; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="monitor"/> is <see langword="null"/>.</exception>
    public static bool IsModelTypeValid<T, TIdentifier>(IMonitor monitor)
        where T : ModelBase
    {
        ArgumentNullException.ThrowIfNull(monitor, nameof(monitor));

        var isValid = true;

        ValidateIdentifier<T, TIdentifier>(monitor, ref isValid);
        ValidateDefaultableEditableProperties<T>(monitor, ref isValid);
        ValidateTokenPropertyOutputs<T>(monitor, ref isValid);

        return isValid;
    }


    /*********
    ** Private Methods
    *********/
    /// <summary>Validates the identifier property.</summary>
    /// <typeparam name="T">The type of the model.</typeparam>
    /// <typeparam name="TIdentifier">The type the model identifier should be.</typeparam>
    /// <param name="monitor">The monitor to use for logging.</param>
    /// <param name="isValid">Whether the model is valid due to the identifier.</param>
    private static void ValidateIdentifier<T, TIdentifier>(IMonitor monitor, ref bool isValid)
        where T : ModelBase
    {
        // ensure T has a single identifier
        var identifierProperties = typeof(T).GetIdentifierProperties();
        if (identifierProperties.Length != 1)
        {
            monitor.Log($"Type: '{typeof(T).FullName}' doesn't have one member with an '{nameof(IdentifierAttribute)}', instead it has {identifierProperties.Length}", LogLevel.Error);
            isValid = false;
        }
        else
        {
            var identifierProperty = identifierProperties.First();

            // ensure identifier is the expected type
            if (identifierProperty.PropertyType != typeof(TIdentifier))
            {
                monitor.Log($"{nameof(TIdentifier)} ({typeof(TIdentifier).FullName}) doesn't match the identifier member ({identifierProperty.GetFullName()})", LogLevel.Error);
                isValid = false;
            }

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

    /// <summary>Validates the defaultable/editable properties.</summary>
    /// <typeparam name="T">The type of the model.</typeparam>
    /// <param name="monitor">The monitor to use for logging.</param>
    /// <param name="isValid">Whether the model is valid due to the defaultable/editable properties.</param>
    private static void ValidateDefaultableEditableProperties<T>(IMonitor monitor, ref bool isValid)
        where T : ModelBase
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

    /// <summary>Validates the output of token properties.</summary>
    /// <typeparam name="T">The type of the model.</typeparam>
    /// <param name="monitor">The monitor to use for logging.</param>
    /// <param name="isValid">Whether the model is valid due to the output of token properties.</param>
    private static void ValidateTokenPropertyOutputs<T>(IMonitor monitor, ref bool isValid)
        where T : ModelBase
    {
        var tokenProperties = typeof(T).GetTokenProperties().ToDictionary(property => property, property => property.GetCustomAttribute<TokenAttribute>()!.OutputPropertyName);
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
}
