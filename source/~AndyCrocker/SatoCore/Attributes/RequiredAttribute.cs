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

namespace SatoCore.Attributes
{
    /// <summary>Indicates a member isn't allowed to be <see langword="null"/> (or whitespace if the member is a <see langword="string"/>.)</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RequiredAttribute : Attribute { }
}
