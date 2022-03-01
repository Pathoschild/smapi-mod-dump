/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal class StaticReturnedToTitleEvent : ReturnedToTitleEvent
{
    /// <summary>Construct an instance.</summary>
    internal StaticReturnedToTitleEvent()
    {
        Enable();
    }

    /// <inheritdoc />
    protected override void OnReturnedToTitleImpl(object sender, ReturnedToTitleEventArgs e)
    {
        // disable events
        EventManager.DisableAllForLocalPlayer();

        // reset mod state
        ModEntry.PlayerState.Value = new();
    }
}