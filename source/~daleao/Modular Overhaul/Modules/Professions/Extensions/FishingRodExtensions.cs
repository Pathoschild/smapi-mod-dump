/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Extensions;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using StardewValley.Buildings;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="FishPond"/> class.</summary>
internal static class FishingRodExtensions
{
    /// <summary>Determines whether the specified bobber's effect should be applied.</summary>
    /// <param name="rod">The <see cref="FishingRod"/>.</param>
    /// <param name="which">The index of the bobber.</param>
    /// <returns><see langword="true"/> if the specified bobber is either directly equipped, or has been recorded in the <paramref name="rod"/>'s "memory".</returns>
    internal static bool HasBobberEffect(this FishingRod rod, int which)
    {
        return rod.getBobberAttachmentIndex() == which ||
               rod.Read<int>(DataKeys.LastTackleUsed) == which;
    }
}
