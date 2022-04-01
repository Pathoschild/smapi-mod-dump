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

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewModdingAPI.Events;

using Extensions;
using Utility;

#endregion using directives

internal class VerifyHudThemeWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation) || e.NewLocation.GetType() == e.OldLocation.GetType()) return;

        if (e.NewLocation.IsDungeon())
            Textures.UltimateMeterTx = Game1.content.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/UltimateMeter");
    }
}