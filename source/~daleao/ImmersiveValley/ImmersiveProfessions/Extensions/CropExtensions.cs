/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Framework;
using StardewValley;

#endregion using directives

public static class CropExtensions
{
    /// <summary>Whether the player should track a given crop.</summary>
    public static bool ShouldBeTracked(this Crop crop) =>
        Game1.player.HasProfession(Profession.Scavenger) && crop.forageCrop.Value;
}