/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using StardewValley.Monsters;

#endregion using directives

internal static class GreenSlimeExtensions
{
    /// <summary>Whether the Slime instance is currently jumping.</summary>
    public static bool IsJumping(this GreenSlime slime)
    {
        return !string.IsNullOrEmpty(slime.ReadData("Jumping"));
    }
}