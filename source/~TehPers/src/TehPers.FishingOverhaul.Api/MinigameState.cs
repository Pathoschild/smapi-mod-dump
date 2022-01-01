/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// The state of the fishing minigame.
    /// </summary>
    /// <param name="IsPerfect">Whether the current catch is still perfect.</param>
    /// <param name="Treasure">The state of the treasure in the minigame.</param>
    public record MinigameState(bool IsPerfect, TreasureState Treasure);
}