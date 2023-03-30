/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class EnchantmentsAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="EnchantmentsAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal EnchantmentsAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("TileSheets/BuffsIcons", new AssetEditor(EditBuffsIconsTileSheet, AssetEditPriority.Default));
        this.Provide(
            $"{Manifest.UniqueID}/QuincyCollisionAnimation",
            new ModTextureProvider(() => "assets/animations/quincy.png", AssetLoadPriority.Medium));
    }

    #region editor callbacks

    /// <summary>Patches buffs icons with energized buff icon.</summary>
    private static void EditBuffsIconsTileSheet(IAssetData asset)
    {
        var editor = asset.AsImage();
        editor.ExtendImage(192, 64);

        var sourceArea = new Rectangle(64, 16, 16, 16);
        var targetArea = new Rectangle(96, 48, 16, 16);
        editor.PatchImage(
            ModHelper.ModContent.Load<Texture2D>("assets/sprites/buffs"),
            sourceArea,
            targetArea);
    }

    #endregion editor callbacks
}
