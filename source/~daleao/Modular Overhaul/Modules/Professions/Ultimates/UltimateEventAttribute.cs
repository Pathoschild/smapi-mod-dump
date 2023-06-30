/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Ultimates;

#region using directives

using DaLion.Shared.Events;

#endregion using directives

/// <summary>Qualifies a <see cref="ManagedEvent"/> class related to ultimate abilities / limit breaks.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class UltimateEventAttribute : Attribute
{
}
