/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Attributes;

using System;

/// <inheritdoc />
[AttributeUsage(AttributeTargets.Class)]
internal class FuryCoreServiceAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FuryCoreServiceAttribute" /> class.
    /// </summary>
    /// <param name="exportable">Indicates if the service is exportable through the FuryCoreApi.</param>
    public FuryCoreServiceAttribute(bool exportable)
    {
        this.Exportable = exportable;
    }

    /// <summary>
    ///     Gets a value indicating whether the service is exportable.
    /// </summary>
    public bool Exportable { get; }
}