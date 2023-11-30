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
using System.Collections.Generic;

namespace SonoCore.Attributes
{
    /// <summary>Indicates the model will only get added to a <see cref="Repository{T, TIdentifier}"/> if atleast one of the mods in the property are present.</summary>
    /// <remarks>This can only be used on a property of type <see cref="IEnumerable{T}"/> <see langword="where"/> T <see langword="is"/> <see langword="string"/>.</remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IncludeWithModPresenceAttribute : Attribute { }
}
