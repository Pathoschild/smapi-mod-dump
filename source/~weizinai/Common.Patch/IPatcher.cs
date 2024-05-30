/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using HarmonyLib;

namespace Common.Patch;

internal interface IPatcher
{
    /// <summary>Apply the Harmony patches for this instance.</summary>
    /// <param name="harmony">The Harmony instance.</param>
    public void Apply(Harmony harmony);
}