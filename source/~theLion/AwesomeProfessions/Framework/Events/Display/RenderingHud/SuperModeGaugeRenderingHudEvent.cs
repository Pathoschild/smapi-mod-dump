/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

internal class SuperModeGaugeRenderingHudEvent : RenderingHudEvent
{
    /// <inheritdoc />
    protected override void OnRenderingHudImpl(object sender, RenderingHudEventArgs e)
    {
        if (ModEntry.PlayerState.Value.SuperMode is null)
        {
            Disable();
            return;
        }

        if (!Game1.eventUp) ModEntry.PlayerState.Value.SuperMode.Gauge.Draw(e.SpriteBatch);
    }
}