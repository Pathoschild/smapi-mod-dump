/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Attributes;

/// <summary>Indicates that an implicitly-used marked symbol should be ignored unless explicitly instantiated.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ImplicitIgnoreAttribute : Attribute
{
}
