/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Slingshots.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class SlingshotAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("TileSheets/weapons", new AssetEditor(EditWeaponsTileSheet, AssetEditPriority.Early));
        this.Provide(
            $"{Manifest.UniqueID}/SnowballCollisionAnimation",
            new ModTextureProvider(() => "assets/animations/snowball.png", AssetLoadPriority.Medium));
    }

    #region editor callbacks

    /// <summary>Edits weapons tilesheet with touched up textures.</summary>
    private static void EditWeaponsTileSheet(IAssetData asset)
    {
        if (!SlingshotsModule.Config.EnableInfinitySlingshot)
        {
            return;
        }

        var editor = asset.AsImage();
        var area = new Rectangle(16, 128, 16, 16);
        editor.PatchImage(ModHelper.ModContent.Load<Texture2D>("assets/sprites/InfinitySlingshot"), targetArea: area);
    }

    #endregion editor callbacks
}
