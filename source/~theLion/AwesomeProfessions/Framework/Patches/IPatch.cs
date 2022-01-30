/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches;

#region using directives

using HarmonyLib;

#endregion using directives

/// <summary>Interface for Harmony patch classes.</summary>
internal interface IPatch
{
    /// <summary>Apply internally-defined Harmony patches.</summary>
    /// <param name="harmony">The Harmony instance for this mod.</param>
    public void Apply(Harmony harmony);
}