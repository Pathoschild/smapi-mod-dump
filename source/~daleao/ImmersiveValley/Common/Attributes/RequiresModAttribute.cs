/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Common.Attributes;

#region using directives

using System;

#endregion using directives

/// <summary>Specifies that a class should only be available if a certain mod is installed.</summary>
[AttributeUsage(AttributeTargets.Class)]
public class RequiresModAttribute : Attribute
{
    /// <summary>The mod unique ID.</summary>
    public string UniqueID { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="uniqueID">The mod unique ID.</param>
    public RequiresModAttribute(string uniqueID)
    {
        UniqueID = uniqueID;
    }
}