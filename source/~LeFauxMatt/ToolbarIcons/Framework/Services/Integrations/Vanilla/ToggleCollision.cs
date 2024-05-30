/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ToolbarIcons.Framework.Services.Integrations.Vanilla;

using StardewMods.ToolbarIcons.Framework.Enums;
using StardewMods.ToolbarIcons.Framework.Interfaces;

/// <inheritdoc />
internal sealed class ToggleCollision : IVanillaIntegration
{
    /// <inheritdoc />
    public string HoverText => I18n.Button_NoClip();

    /// <inheritdoc />
    public string Icon => InternalIcon.ToggleCollision.ToStringFast();

    /// <inheritdoc />
    public void DoAction()
    {
        if (Context.IsPlayerFree || (Context.IsWorldReady && Game1.eventUp))
        {
            Game1.player.ignoreCollisions = !Game1.player.ignoreCollisions;
        }
    }
}