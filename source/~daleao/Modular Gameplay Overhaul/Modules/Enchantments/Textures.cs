/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments;

#region using directives

using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Caches custom mod textures and related functions.</summary>
internal static class Textures
{
    internal static Texture2D ForgeIconTx { get; set; } = ModHelper.ModContent.Load<Texture2D>("assets/menus/ForgeIcon" +
        $"_{EnchantmentsModule.Config.SocketStyle}" +
        (ModHelper.ModRegistry.IsLoaded("ManaKirel.VMI") ||
         ModHelper.ModRegistry.IsLoaded("ManaKirel.VintageInterface2")
            ? "_Vintage"
            : string.Empty));
}
