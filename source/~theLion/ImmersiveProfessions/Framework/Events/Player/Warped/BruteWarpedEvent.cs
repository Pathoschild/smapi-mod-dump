/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using StardewModdingAPI.Events;

using Extensions;
using GameLoop;

#endregion using directives

internal class BruteWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation)) return;

        if (e.NewLocation.IsDungeon() || e.NewLocation.HasMonsters())
        {
            EventManager.Enable(typeof(BruteUpdateTickedEvent));
        }
        else
        {
            ModEntry.PlayerState.BruteRageCounter = 0;
            EventManager.Disable(typeof(BruteUpdateTickedEvent));
        }
    }
}