/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Exceptions;

/// <summary>Thrown when a given type is not found in any executing assembly at runtime.</summary>
public sealed class MissingTypeException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="MissingTypeException"/> class.</summary>
    /// <param name="name">The name of the expected type.</param>
    public MissingTypeException(string name)
        : base($"A type named {name} could not be found in the executing assemblies.")
    {
    }
}
