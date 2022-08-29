/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Ultimates;

#region using directives

using System;

#endregion using directives

/// <summary>Specifies that a class is deprecated and should not be available.</summary>
[AttributeUsage(AttributeTargets.Class)]
public class UltimateEventAttribute : Attribute
{
}