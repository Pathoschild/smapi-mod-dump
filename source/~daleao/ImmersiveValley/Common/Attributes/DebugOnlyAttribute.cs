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

/// <summary>Specifies that a class should only be available in debug mode.</summary>
[AttributeUsage(AttributeTargets.Class)]
public class DebugOnlyAttribute : Attribute
{
}