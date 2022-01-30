/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal class DebugCursorMovedEvent : CursorMovedEvent
{
    /// <inheritdoc />
    protected override void OnCursorMovedImpl(object sender, CursorMovedEventArgs e)
    {
        ModEntry.State.Value.CursorPosition = e.NewPosition;
    }
}